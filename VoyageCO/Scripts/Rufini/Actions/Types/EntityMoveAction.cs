using SFML.System;
using System;
using System.Collections.Generic;
using VCO.Actors.NPCs;
using VCO.Actors.NPCs.Movement;
using VCO.Scripts;
using VCO.Scripts.Actions;
using Violet.Actors;

namespace Rufini.Actions.Types
{
    internal class EntityMoveAction : RufiniAction
    {
        public override string Code => "EMOV";
        public EntityMoveAction()
        {
            this.paramList = new List<ActionParam>
            {
                new ActionParam
                {
                    Name = "name",
                    Type = typeof(string)
                },
                new ActionParam
                {
                    Name = "x",
                    Type = typeof(int)
                },
                new ActionParam
                {
                    Name = "y",
                    Type = typeof(int)
                },
                new ActionParam
                {
                    Name = "rel",
                    Type = typeof(bool)
                },
                new ActionParam
                {
                    Name = "spd",
                    Type = typeof(int)
                },
                new ActionParam
                {
                    Name = "sub",
                    Type = typeof(string)
                },
                new ActionParam
                {
                    Name = "blk",
                    Type = typeof(bool)
                }
            };
        }

        public override ActionReturnContext Execute(ExecutionContext context)
        {
            ActionReturnContext result = default(ActionReturnContext);
            string name = base.GetValue<string>("name");
            int value = base.GetValue<int>("x");
            int value2 = base.GetValue<int>("y");
            bool value3 = base.GetValue<bool>("rel");
            int value4 = base.GetValue<int>("spd");
            string value5 = base.GetValue<string>("sub");
            this.blocking = base.GetValue<bool>("blk");
            NPC npc = (NPC)context.ActorManager.Find((Actor n) => n is NPC && ((NPC)n).Name == name);
            if (npc != null)
            {
                Vector2f vector2f = new Vector2f(value, value2);
                if (value3)
                {
                    vector2f += npc.Position;
                }
                if (value4 > 0)
                {
                    PointMover pointMover = new PointMover(vector2f, value4);
                    pointMover.OnMoveComplete += this.OnMoveComplete;
                    npc.SetMover(pointMover);
                    this.context = context;
                    if (this.blocking)
                    {
                        result.Wait = ScriptExecutor.WaitType.Event;
                    }
                }
                else
                {
                    npc.Position = vector2f;
                }
                if (value5.Length > 0)
                {
                    npc.OverrideSubsprite(value5);
                }
                else
                {
                    npc.ClearOverrideSubsprite();
                }
            }
            else
            {
                Console.WriteLine("Count not find NPC named \"{0}\"", name);
            }
            return result;
        }

        private void OnMoveComplete(PointMover sender)
        {
            sender.OnMoveComplete -= this.OnMoveComplete;
            if (this.blocking)
            {
                this.context.Executor.Continue();
            }
        }

        private ExecutionContext context;

        private bool blocking;
    }
}

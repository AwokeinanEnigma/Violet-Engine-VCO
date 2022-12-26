using System.Collections.Generic;
using VCO.Actors.NPCs;
using VCO.Scripts;
using VCO.Scripts.Actions;
using Violet.Actors;
using Violet.Graphics;

namespace Rufini.Actions.Types
{
    internal class CameraNPCAction : RufiniAction
    {
        public override string Code => "CFLN";
        public CameraNPCAction()
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
                    Name = "spd",
                    Type = typeof(float)
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
            float value = base.GetValue<float>("spd");
            bool value2 = base.GetValue<bool>("blk");
            if (!string.IsNullOrWhiteSpace(name))
            {
                NPC npc = (NPC)context.ActorManager.Find((Actor x) => x is NPC && ((NPC)x).Name == name);
                if (npc != null)
                {
                    ViewManager.Instance.MoveTo(npc, value);
                    if (value2)
                    {
                        this.context = context;
                        ViewManager.Instance.OnMoveToComplete += this.OnMoveToComplete;
                        result.Wait = ScriptExecutor.WaitType.Event;
                    }
                }
            }
            else
            {
                ViewManager.Instance.FollowActor = null;
            }
            return result;
        }

        private void OnMoveToComplete(ViewManager sender)
        {
            ViewManager.Instance.OnMoveToComplete -= this.OnMoveToComplete;
            this.context.Executor.Continue();
        }

        private ExecutionContext context;
    }
}

using SFML.System;
using System;
using System.Collections.Generic;
using VCO.Actors.NPCs;
using VCO.Scripts;
using VCO.Scripts.Actions;
using VCO.Scripts.Actions.ParamTypes;
using Violet.Actors;
using Violet.Utility;

namespace Rufini.Actions.Types
{
    internal class EntityDirectionAction : RufiniAction
    {
        public EntityDirectionAction()
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
                    Name = "dir",
                    Type = typeof(RufiniOption)
                },
                new ActionParam
                {
                    Name = "spd",
                    Type = typeof(float)
                }
            };
        }

        public override ActionReturnContext Execute(ExecutionContext context)
        {
            ActionReturnContext result = default(ActionReturnContext);
            this.context = context;
            string name = base.GetValue<string>("name");
            this.directionTo = base.GetValue<RufiniOption>("dir").Option;
            this.speed = base.GetValue<float>("spd");
            this.npc = (NPC)context.ActorManager.Find((Actor n) => n is NPC && ((NPC)n).Name == name);
            if (this.npc != null)
            {
                this.npc.MovementLocked = true;
                if (this.directionTo >= 8)
                {
                    Vector2f v = VectorMath.Normalize(context.Player.Position - this.npc.Position);
                    this.directionTo = VectorMath.VectorToDirection(v);
                }
                int num = Math.Abs(this.directionTo - this.npc.Direction);
                int num2 = Math.Abs(this.directionTo + 8 - this.npc.Direction) % 8;
                this.increment = ((num < num2) ? -1 : 1);
                if (this.speed > 0f)
                {
                    this.TurnStep(true);
                    FrameTimerManager.Instance.OnTimerEnd += this.OnTimer;
                    result.Wait = ScriptExecutor.WaitType.Event;
                }
                else
                {
                    this.npc.Direction = this.directionTo;
                }
            }
            return result;
        }

        private void TurnStep(bool isFirst)
        {
            int num = this.npc.Direction;
            if (!isFirst)
            {
                num += this.increment;
                if (num < 0)
                {
                    num = 7;
                }
                if (num > 7)
                {
                    num = 0;
                }
                this.npc.Direction = num;
            }
            if (num != this.directionTo)
            {
                this.timerId = FrameTimerManager.Instance.StartTimer((int)(1f / this.speed));
                return;
            }
            FrameTimerManager.Instance.OnTimerEnd -= this.OnTimer;
            this.npc.MovementLocked = false;
            this.context.Executor.Continue();
        }

        private void OnTimer(int timerIndex)
        {
            if (this.timerId == timerIndex)
            {
                this.TurnStep(false);
            }
        }

        private ExecutionContext context;

        private int timerId;

        private int directionTo;

        private int increment;

        private float speed;

        private NPC npc;
    }
}

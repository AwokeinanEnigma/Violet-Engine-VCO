using System.Collections.Generic;
using VCO.Scripts;
using VCO.Scripts.Actions;
using Violet.Utility;

namespace Rufini.Actions.Types
{
    internal class WaitAction : RufiniAction
    {
        public WaitAction()
        {
            this.paramList = new List<ActionParam>
            {
                new ActionParam
                {
                    Name = "wait",
                    Type = typeof(int)
                }
            };
        }

        public override ActionReturnContext Execute(ExecutionContext context)
        {
            this.context = context;
            int value = base.GetValue<int>("wait");
            this.timerIndex = FrameTimerManager.Instance.StartTimer(value);
            FrameTimerManager.Instance.OnTimerEnd += this.TimerEnd;
            return new ActionReturnContext
            {
                Wait = ScriptExecutor.WaitType.Event
            };
        }

        private void TimerEnd(int timerIndex)
        {
            if (this.timerIndex == timerIndex)
            {
                FrameTimerManager.Instance.OnTimerEnd -= this.TimerEnd;
                this.context.Executor.Continue();
            }
        }

        private ExecutionContext context;

        private int timerIndex;
    }
}

using System.Collections.Generic;
using VCO.Scripts;
using VCO.Scripts.Actions;
using VCO.Scripts.Actions.ParamTypes;
using Violet.Graphics;

namespace Rufini.Actions.Types
{
    internal class CameraMoveAction : RufiniAction
    {
        public CameraMoveAction()
        {
            this.paramList = new List<ActionParam>
            {
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
                    Name = "spd",
                    Type = typeof(float)
                },
                new ActionParam
                {
                    Name = "mod",
                    Type = typeof(RufiniOption)
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
            int value = base.GetValue<int>("x");
            int value2 = base.GetValue<int>("y");
            float value3 = base.GetValue<float>("spd");
            int option = base.GetValue<RufiniOption>("mod").Option;
            bool value4 = base.GetValue<bool>("blk");
            ViewManager.MoveMode moveToMode;
            switch (option)
            {
                case 1:
                    moveToMode = ViewManager.MoveMode.Smoothed;
                    break;
                case 2:
                    moveToMode = ViewManager.MoveMode.ExpIn;
                    break;
                case 3:
                    moveToMode = ViewManager.MoveMode.ExpOut;
                    break;
                default:
                    moveToMode = ViewManager.MoveMode.Linear;
                    break;
            }
            ViewManager.Instance.MoveToMode = moveToMode;
            ViewManager.Instance.FollowActor = null;
            ViewManager.Instance.MoveTo(value, value2, value3);
            if (value4)
            {
                this.context = context;
                ViewManager.Instance.OnMoveToComplete += this.OnMoveToComplete;
                result.Wait = ScriptExecutor.WaitType.Event;
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

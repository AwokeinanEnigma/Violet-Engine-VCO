using SFML.System;
using System.Collections.Generic;
using VCO.Scripts;
using VCO.Scripts.Actions;
using Violet.Graphics;
using Violet.Utility;

namespace Rufini.Actions.Types
{
    internal class AnimationAction : RufiniAction
    {
        public AnimationAction()
        {
            this.paramList = new List<ActionParam>
            {
                new ActionParam
                {
                    Name = "spr",
                    Type = typeof(string)
                },
                new ActionParam
                {
                    Name = "sub",
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
                    Name = "depth",
                    Type = typeof(int)
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
            this.context = context;
            string value = base.GetValue<string>("spr");
            string value2 = base.GetValue<string>("sub");
            int value3 = base.GetValue<int>("x");
            int value4 = base.GetValue<int>("y");
            int value5 = base.GetValue<int>("depth");
            this.blocking = base.GetValue<bool>("blk");
            this.graphic = new IndexedColorGraphic(DataHandler.instance.Load(value + ".dat"), value2, new Vector2f(value3, value4), (value5 < 0) ? value4 : value5);
            this.graphic.OnAnimationComplete += this.OnAnimationComplete;
            this.context.Pipeline.Add(this.graphic);
            if (this.blocking)
            {
                result.Wait = ScriptExecutor.WaitType.Event;
            }
            return result;
        }
        private void OnAnimationComplete(AnimatedRenderable graphic)
        {
            this.context.Pipeline.Remove(graphic);
            this.graphic.OnAnimationComplete -= this.OnAnimationComplete;
            this.timerId = FrameTimerManager.Instance.StartTimer(1);
            FrameTimerManager.Instance.OnTimerEnd += this.OnTimerEnd;
            if (this.blocking)
            {
                this.context.Executor.Continue();
            }
        }
        private void OnTimerEnd(int timerIndex)
        {
            if (this.timerId == timerIndex)
            {
                this.graphic.Dispose();
                FrameTimerManager.Instance.OnTimerEnd -= this.OnTimerEnd;
            }
        }
        private Graphic graphic;
        private ExecutionContext context;
        private bool blocking;
        private int timerId;
    }
}

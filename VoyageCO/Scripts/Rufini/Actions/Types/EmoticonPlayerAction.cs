using System.Collections.Generic;
using VCO.Scripts;
using VCO.Scripts.Actions;
using VCO.Scripts.Actions.ParamTypes;
using Violet.Graphics;

namespace Rufini.Actions.Types
{
    internal class EmoticonPlayerAction : RufiniAction
    {
        public override string Code => "EMPL";
        public EmoticonPlayerAction()
        {
            this.paramList = new List<ActionParam>
            {
                new ActionParam
                {
                    Name = "emt",
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
            RufiniOption value = base.GetValue<RufiniOption>("emt");
            this.isBlocking = base.GetValue<bool>("blk");
            if (context.Player != null)
            {
                string spriteName = EmoticonPlayerAction.EMOTE_TYPE_SUBSPRITE_MAP[0];
                EmoticonPlayerAction.EMOTE_TYPE_SUBSPRITE_MAP.TryGetValue(value.Option, out spriteName);
                IndexedColorGraphic indexedColorGraphic = new IndexedColorGraphic(DataHandler.instance.Load("emote.dat"), spriteName, context.Player.EmoticonPoint, context.Player.Depth);
                indexedColorGraphic.OnAnimationComplete += this.OnAnimationComplete;
                context.Pipeline.Add(indexedColorGraphic);
                this.context = context;
            }
            else
            {
                this.isBlocking = false;
            }
            return new ActionReturnContext
            {
                Wait = (this.isBlocking ? ScriptExecutor.WaitType.Event : ScriptExecutor.WaitType.None)
            };
        }
        private void OnAnimationComplete(AnimatedRenderable graphic)
        {
            this.context.Pipeline.Remove(graphic);
            graphic.Dispose();
            graphic = null;
            if (this.isBlocking)
            {
                this.context.Executor.Continue();
            }
        }
        private static readonly Dictionary<int, string> EMOTE_TYPE_SUBSPRITE_MAP = new Dictionary<int, string>
        {
            {
                0,
                "surprise"
            },
            {
                1,
                "question"
            },
            {
                2,
                "ellipses"
            },
            {
                3,
                "frustration"
            },
            {
                4,
                "vein"
            },
            {
                5,
                "idea"
            },
            {
                6,
                "music"
            }
        };
        private ExecutionContext context;
        private bool isBlocking;
    }
}

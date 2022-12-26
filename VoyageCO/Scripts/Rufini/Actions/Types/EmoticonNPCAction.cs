using System.Collections.Generic;
using VCO.Actors.NPCs;
using VCO.Scripts;
using VCO.Scripts.Actions;
using VCO.Scripts.Actions.ParamTypes;
using Violet.Graphics;

namespace Rufini.Actions.Types
{
    internal class EmoticonNPCAction : RufiniAction
    {
        public override string Code => "EMNP";
        public EmoticonNPCAction()
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
            string value = base.GetValue<string>("name");
            RufiniOption value2 = base.GetValue<RufiniOption>("emt");
            this.isBlocking = base.GetValue<bool>("blk");
            NPC npcByName = context.GetNpcByName(value);
            if (npcByName != null)
            {
                string spriteName = EmoticonNPCAction.EMOTE_TYPE_SUBSPRITE_MAP[0];
                EmoticonNPCAction.EMOTE_TYPE_SUBSPRITE_MAP.TryGetValue(value2.Option, out spriteName);
                IndexedColorGraphic indexedColorGraphic = new IndexedColorGraphic(DataHandler.instance.Load("emote.dat"), spriteName, npcByName.EmoticonPoint, npcByName.Depth);
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

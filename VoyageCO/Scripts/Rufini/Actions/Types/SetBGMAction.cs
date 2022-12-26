using System.Collections.Generic;
using VCO.Scripts;
using VCO.Scripts.Actions;
using Violet.Audio;

namespace Rufini.Actions.Types
{
    internal class SetBGMAction : RufiniAction
    {
        public override string Code => "SBGM";
        public SetBGMAction()
        {
            this.paramList = new List<ActionParam>
            {
                new ActionParam
                {
                    Name = "bgm",
                    Type = typeof(string)
                },
                new ActionParam
                {
                    Name = "loop",
                    Type = typeof(bool)
                }
            };
        }

        public override ActionReturnContext Execute(ExecutionContext context)
        {
            string value = base.GetValue<string>("bgm");
            bool value2 = base.GetValue<bool>("loop");
            AudioManager.Instance.SetBGM(value);
            AudioManager.Instance.BGM.LoopCount = (value2 ? -1 : 0);
            AudioManager.Instance.BGM.Play();
            return default(ActionReturnContext);
        }
    }
}

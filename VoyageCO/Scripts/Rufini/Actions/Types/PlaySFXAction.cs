using System.Collections.Generic;
using VCO.Scripts;
using VCO.Scripts.Actions;
using Violet.Audio;

namespace Rufini.Actions.Types
{
    internal class PlaySFXAction : RufiniAction
    {
        public override string Code => "PSFX";
        public PlaySFXAction()
        {
            this.paramList = new List<ActionParam>
            {
                new ActionParam
                {
                    Name = "sfx",
                    Type = typeof(string)
                },
                new ActionParam
                {
                    Name = "loop",
                    Type = typeof(int)
                },
                new ActionParam
                {
                    Name = "bal",
                    Type = typeof(float)
                }
            };
        }

        public override ActionReturnContext Execute(ExecutionContext context)
        {
            string value = base.GetValue<string>("sfx");
            bool value2 = base.GetValue<bool>("loop");
            base.GetValue<float>("bal");
            VioletSound VioletSound = AudioManager.Instance.Use(value, AudioType.Sound);
            VioletSound.LoopCount = (value2 ? -1 : 0);
            VioletSound.OnComplete += this.SoundComplete;
            VioletSound.Play();
            return default(ActionReturnContext);
        }

        private void SoundComplete(VioletSound sender)
        {
            AudioManager.Instance.Unuse(sender);
        }
    }
}

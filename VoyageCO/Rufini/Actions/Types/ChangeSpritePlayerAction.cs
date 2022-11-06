using System;
using System.Collections.Generic;
using System.IO;
using VCO.Scripts;
using VCO.Scripts.Actions;

namespace Rufini.Actions.Types
{
    internal class ChangeSpritePlayerAction : RufiniAction
    {
        public ChangeSpritePlayerAction()
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
                }
            };
        }

        public override ActionReturnContext Execute(ExecutionContext context)
        {
            string value = base.GetValue<string>("spr");
            string value2 = base.GetValue<string>("sub");
            if (context.Player != null && value.Length > 0)
            {
                string text = Paths.GRAPHICS + value + ".dat";
                if (File.Exists(text))
                {
                    context.Player.ChangeSprite(text, value2);
                }
                else
                {
                    Console.WriteLine("Sprite file \"{0}\" does not exist.", text);
                }
            }
            return default(ActionReturnContext);
        }
    }
}

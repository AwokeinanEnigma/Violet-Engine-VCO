using System;
using System.Collections.Generic;
using VCO.Scripts;
using VCO.Scripts.Actions;
using VCO.Scripts.Actions.ParamTypes;

namespace Rufini.Actions.Types
{
    internal class ScreenEffectAction : RufiniAction
    {
        public ScreenEffectAction()
        {
            this.paramList = new List<ActionParam>
            {
                new ActionParam
                {
                    Name = "eff",
                    Type = typeof(RufiniOption)
                }
            };
        }

        public override ActionReturnContext Execute(ExecutionContext context)
        {
            Console.WriteLine("NOT IMPLEMENTED - BUG DAVE");
            return default(ActionReturnContext);
        }
    }
}

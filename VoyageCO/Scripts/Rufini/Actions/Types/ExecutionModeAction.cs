using System;
using System.Collections.Generic;
using VCO.Scripts;
using VCO.Scripts.Actions;
using VCO.Scripts.Actions.ParamTypes;

namespace Rufini.Actions.Types
{
    internal class ExecutionModeAction : RufiniAction
    {
        public override string Code => "EXMD";
        public ExecutionModeAction()
        {
            this.paramList = new List<ActionParam>
            {
                new ActionParam
                {
                    Name = "mode",
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

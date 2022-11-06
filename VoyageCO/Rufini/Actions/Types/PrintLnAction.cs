using System;
using System.Collections.Generic;
using VCO.Scripts;
using VCO.Scripts.Actions;

namespace Rufini.Actions.Types
{
    internal class PrintLnAction : RufiniAction
    {
        public PrintLnAction()
        {
            this.paramList = new List<ActionParam>
            {
                new ActionParam
                {
                    Name = "text",
                    Type = typeof(string)
                }
            };
        }

        public override ActionReturnContext Execute(ExecutionContext context)
        {
            string value = base.GetValue<string>("text");
            Console.WriteLine(value);
            return default(ActionReturnContext);
        }
    }
}

using System.Collections.Generic;
using VCO.Scripts;
using VCO.Scripts.Actions;

namespace Rufini.Actions.Types
{
    internal class EndElseAction : RufiniAction
    {
        public override string Code => "ELEN";
        public EndElseAction()
        {
            this.paramList = new List<ActionParam>();
        }

        public override ActionReturnContext Execute(ExecutionContext context)
        {
            return default(ActionReturnContext);
        }
    }
}

using System.Collections.Generic;
using VCO.Scripts;
using VCO.Scripts.Actions;
using Violet.Flags;

namespace Rufini.Actions.Types
{
    internal class SetMoneyAction : RufiniAction
    {
        public SetMoneyAction()
        {
            this.paramList = new List<ActionParam>
            {
                new ActionParam
                {
                    Name = "val",
                    Type = typeof(int)
                }
            };
        }

        public override ActionReturnContext Execute(ExecutionContext context)
        {
            int value = base.GetValue<int>("val");
            ValueManager.Instance[1] = value;
            return default(ActionReturnContext);
        }
    }
}

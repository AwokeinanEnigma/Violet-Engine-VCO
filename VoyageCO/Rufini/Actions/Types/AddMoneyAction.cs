using System.Collections.Generic;
using VCO.Scripts;
using VCO.Scripts.Actions;
using Violet.Flags;

namespace Rufini.Actions.Types
{
    internal class AddMoneyAction : RufiniAction
    {
        public AddMoneyAction()
        {
            this.paramList = new List<ActionParam>
            {
                new ActionParam
                {
                    Name = "val",
                    Type = typeof(int)
                },
                new ActionParam
                {
                    Name = "msg",
                    Type = typeof(bool)
                }
            };
        }

        public override ActionReturnContext Execute(ExecutionContext context)
        {
            int value = base.GetValue<int>("val");
            base.GetValue<bool>("msg");
            ValueManager instance;
            (instance = ValueManager.Instance)[1] = instance[1] + value;
            return default(ActionReturnContext);
        }
    }
}

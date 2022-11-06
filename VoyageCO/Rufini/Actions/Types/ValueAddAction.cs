using System.Collections.Generic;
using VCO.Scripts;
using VCO.Scripts.Actions;
using Violet.Flags;

namespace Rufini.Actions.Types
{
    internal class ValueAddAction : RufiniAction
    {
        public ValueAddAction()
        {
            this.paramList = new List<ActionParam>
            {
                new ActionParam
                {
                    Name = "id",
                    Type = typeof(int)
                },
                new ActionParam
                {
                    Name = "val",
                    Type = typeof(int)
                }
            };
        }

        public override ActionReturnContext Execute(ExecutionContext context)
        {
            int value = base.GetValue<int>("id");
            int value2 = base.GetValue<int>("val");
            ValueManager instance;
            int index;
            (instance = ValueManager.Instance)[index = value] = instance[index] + value2;
            return default(ActionReturnContext);
        }
    }
}

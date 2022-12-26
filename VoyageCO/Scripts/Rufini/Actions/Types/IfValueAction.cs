using System.Collections.Generic;
using VCO.Scripts;
using VCO.Scripts.Actions;
using VCO.Scripts.Actions.ParamTypes;
using Violet.Flags;

namespace Rufini.Actions.Types
{
    internal class IfValueAction : RufiniAction
    {
        public override string Code => "IFVL";
        public IfValueAction()
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
                    Name = "op",
                    Type = typeof(RufiniOption)
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
            int value2 = base.GetValue<int>("op");
            int value3 = base.GetValue<int>("val");
            int num = ValueManager.Instance[value];
            bool[] array = new bool[]
            {
                value3 == num,
                value3 != num,
                value3 <= num,
                value3 >= num,
                value3 < num,
                value3 > num
            };
            if (!array[value2])
            {
                context.Executor.JumpToElseOrEndIf();
            }
            return default(ActionReturnContext);
        }
    }
}

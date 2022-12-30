using System.Collections.Generic;
using VCO.Scripts;
using VCO.Scripts.Actions;
using Violet.Flags;

namespace Rufini.Actions.Types
{
    internal class IfFlagAction : RufiniAction
    {
        public override string Code => "IFFL";
        public bool satisfied;
        public IfFlagAction()
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
                    Type = typeof(bool)
                }
            };
        }

        public override ActionReturnContext Execute(ExecutionContext context)
        {
            int value = base.GetValue<int>("id");
            bool value2 = base.GetValue<bool>("val");
            bool flag = FlagManager.Instance[value];
            if (value2 != flag)
            {
                context.Executor.JumpToElseOrEndIf();
                satisfied = false;
            }
            else {
                satisfied = true;
            }
            return default(ActionReturnContext);
        }
    }
}

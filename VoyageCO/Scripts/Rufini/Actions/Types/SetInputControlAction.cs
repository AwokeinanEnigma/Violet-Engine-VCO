using System.Collections.Generic;
using VCO.Scripts;
using VCO.Scripts.Actions;
using VCO.Scripts.Actions.ParamTypes;
using Violet;

namespace Rufini.Actions.Types
{
    internal class SetInputControlAction : RufiniAction
    {
        public override string Code => "CTRL";

        public SetInputControlAction()
        {
            this.paramList = new List<ActionParam>
            {
                new ActionParam
                {
                    Name = "mode",
                    Type = typeof(RufiniOption)
                },
            };
        }
        public override ActionReturnContext Execute(ExecutionContext context)
        {
            RufiniOption value = base.GetValue<RufiniOption>("mode");
            Debug.Log("ohhh!");
            if (context.Player != null)
            {
                switch (value.Option) {
                    case 0:
                        context.Player.InputLocked = false;
                        break;
                    case 1:
                        context.Player.InputLocked = true;
                        break;
                    case 2:
                        context.Player.InputLocked = true;
                        break;
                }
            }
            else
            {
                Debug.LogWarning("Tried to use SetInputControlAction in place where the player was null! ");
            }
            return default(ActionReturnContext);
        }
    }
}

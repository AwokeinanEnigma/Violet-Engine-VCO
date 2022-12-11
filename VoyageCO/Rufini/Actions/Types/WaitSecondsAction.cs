using SFML.System;
using System.Collections.Generic;
using VCO.Scripts;
using VCO.Scripts.Actions;
using Violet.Utility;

namespace Rufini.Actions.Types
{
    internal class WaitSecondsAction : RufiniAction
    {
        public WaitSecondsAction()
        {
            this.paramList = new List<ActionParam>
            {
                new ActionParam
                {
                    Name = "wait",
                    Type = typeof(int)
                }
            };
        }

        public override ActionReturnContext Execute(ExecutionContext context)
        {
            Clock clock = new Clock();
            clock.Restart();
            int waitTime = base.GetValue<int>("wait");

            while (clock.ElapsedTime.AsSeconds() < waitTime) {
                
                if (clock.ElapsedTime.AsSeconds() >= waitTime) {
                    
                    goto I_CANGO;
                }
            }

        I_CANGO:
            clock.Dispose();
            return default;
        }
    }
}

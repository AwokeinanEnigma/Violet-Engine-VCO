using SFML.System;
using System.Collections.Generic;
using VCO.Scripts;
using VCO.Scripts.Actions;
using Violet.Utility;

namespace Rufini.Actions.Types
{
    internal class WaitSecondsAction : RufiniAction
    {
        public override string Code => "WSEC";
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
            int waitTime = base.GetValue<int>("wait");

            Clock clock = new Clock();
            clock.Restart();

            while (clock.ElapsedTime.AsSeconds() < waitTime)
            {
                // do nothing;
            }
            return default;
        }

    }
}

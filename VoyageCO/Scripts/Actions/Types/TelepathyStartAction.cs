using System;
using System.Collections.Generic;
using VCO.Actors;
using VCO.GUI;
using Violet.Flags;

namespace VCO.Scripts.Actions.Types
{
    internal class TelepathyStartAction : RufiniAction
    {
        public override string Code => "TESA";

        public TelepathyStartAction()
        {
            this.paramList = new List<ActionParam>();
        }

        public override ActionReturnContext Execute(ExecutionContext context)
        {
            Console.WriteLine("It's telepathy time!");
            FlagManager.Instance[4] = true;
            this.context = context;
            if (this.context.Player != null)
            {
                this.context.Player.Telepathize();
                this.context.Player.OnTelepathyAnimationComplete += this.player_OnTelepathyAnimationComplete;
            }
            return new ActionReturnContext
            {
                Wait = ScriptExecutor.WaitType.Event
            };
        }

        private void player_OnTelepathyAnimationComplete(Player player)
        {
            player.OnTelepathyAnimationComplete -= this.player_OnTelepathyAnimationComplete;
            if (this.context.CheckedNPC != null)
            {
                this.context.CheckedNPC.Telepathize();
                if (this.context.TextBox is OverworldTextBox)
                {
                    ((OverworldTextBox)this.context.TextBox).SetDimmer(0.5f);
                }
            }
            this.context.Executor.Continue();
            this.context = null;
        }

        private ExecutionContext context;
    }
}

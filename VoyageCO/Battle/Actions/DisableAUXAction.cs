using SFML.Graphics;
using System;
using VCO.Battle.Combatants;
using VCO.Data;

namespace VCO.Battle.Actions
{
    internal class DisableAUXAction : BattleAction
    {
        public DisableAUXAction(ActionParams aparams) : base(aparams)
        {
            Console.WriteLine($"Current Target: {targets[0] } ");
            this.state = DisableAUXAction.State.Initialize;
        }

        protected override void UpdateAction()
        {
            base.UpdateAction();
            switch (this.state)
            {
                case DisableAUXAction.State.Initialize:

                    this.controller.InterfaceController.OnTextboxComplete += this.InteractionComplete;
                    targets[0].AddStatusEffect(StatusEffect.DisableAUX, 3);

                    EnemyCombatant cE = this.sender as EnemyCombatant;
                    PlayerCombatant pC = targets[0] as PlayerCombatant;

                    this.controller.InterfaceController.ShowMessage($"{  cE.Enemy.PlayerFriendlyName } disabled { CharacterNames.GetName(pC.Character) }'s AUX!", true);
                    this.controller.InterfaceController.FlashEnemy(this.sender as EnemyCombatant, Color.Black, 8, 2);
                    this.controller.InterfaceController.PreEnemyAttack.Play();
                    this.state = DisableAUXAction.State.WaitForUI;
                    return;
                case DisableAUXAction.State.WaitForUI:
                    break;
                case DisableAUXAction.State.Finish:
                    this.controller.InterfaceController.OnTextboxComplete -= this.InteractionComplete;
                    this.complete = true;
                    break;
                default:
                    return;
            }
        }

        public void InteractionComplete()
        {
            this.state = DisableAUXAction.State.Finish;
        }

        private const int MESSAGE_INDEX = 0;

        private const int USE_BUTTON_INDEX = 1;

        private const int BLINK_DURATION = 8;

        private const int BLINK_COUNT = 2;

        private DisableAUXAction.State state;

        private enum State
        {
            Initialize,
            WaitForUI,
            Finish
        }
    }
}

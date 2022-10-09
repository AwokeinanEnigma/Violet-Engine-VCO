using System;
using SunsetRhapsody.Battle.Combatants;
using SunsetRhapsody.Data;
using SFML.Graphics;
using SunsetRhapsody.Utility;

namespace SunsetRhapsody.Battle.Actions
{
	internal class InflictInfectionAction : BattleAction
	{
		public InflictInfectionAction(ActionParams aparams) : base(aparams)
		{
			Console.WriteLine($"Current Target for infliction: {targets[0] } ");
			this.state = InflictInfectionAction.State.Initialize;
		}

		protected override void UpdateAction()
		{
			base.UpdateAction();
			switch (this.state)
			{
				case InflictInfectionAction.State.Initialize:

					this.controller.InterfaceController.OnTextboxComplete += this.InteractionComplete;
					targets[0].AddStatusEffect(StatusEffect.Infection, 3);

					EnemyCombatant cE = this.sender as EnemyCombatant;
					PlayerCombatant pC = targets[0] as PlayerCombatant;

					this.controller.InterfaceController.ShowMessage($"{Capitalizer.Capitalize(cE.Enemy.Article)}{  cE.Enemy.PlayerFriendlyName }inflicted { CharacterNames.GetName(pC.Character) } with a deadly infection!", true);
					this.controller.InterfaceController.FlashEnemy(this.sender as EnemyCombatant, Color.Black, 8, 2);
					this.controller.InterfaceController.PreEnemyAttack.Play();
					this.state = InflictInfectionAction.State.WaitForUI;
					return;
				case InflictInfectionAction.State.WaitForUI:
					break;
				case InflictInfectionAction.State.Finish:
					this.controller.InterfaceController.OnTextboxComplete -= this.InteractionComplete;
					this.complete = true;
					break;
				default:
					return;
			}
		}

		public void InteractionComplete()
		{
			this.state = InflictInfectionAction.State.Finish;
		}

		private const int MESSAGE_INDEX = 0;

		private const int USE_BUTTON_INDEX = 1;

		private const int BLINK_DURATION = 8;

		private const int BLINK_COUNT = 2;

		private InflictInfectionAction.State state;

		private enum State
		{
			Initialize,
			WaitForUI,
			Finish
		}
	}
}

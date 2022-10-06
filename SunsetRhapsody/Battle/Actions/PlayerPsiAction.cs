using System;
using System.Linq;
using SunsetRhapsody.Battle.Combatants;
using SunsetRhapsody.Battle.PsiAnimation;
using SunsetRhapsody.Battle.UI;
using SunsetRhapsody.Data;
using SunsetRhapsody.Psi;

namespace SunsetRhapsody.Battle.Actions
{
	internal class PlayerPsiAction : BattleAction
	{
		public PlayerPsiAction(ActionParams aparams) : base(aparams)
		{
			this.combatant = (this.sender as PlayerCombatant);
			IPsi psi = (aparams.data.Length > 0) ? ((IPsi)aparams.data[0]) : null;
			this.psi = ((psi is OffensePsi) ? ((OffensePsi)psi) : default(OffensePsi));
			this.psiLevel = ((aparams.data.Length > 1) ? ((int)aparams.data[1]) : 0);
			this.state = PlayerPsiAction.State.Initialize;
		}

		private void RemoveInvalidTargets()
		{
			Combatant[] factionCombatants = this.controller.CombatantController.GetFactionCombatants(BattleFaction.EnemyTeam, true);
			bool[] array = new bool[this.targets.Length];
			int num = this.targets.Length;
			for (int i = 0; i < this.targets.Length; i++)
			{
				Combatant combatant = this.targets[i];
				if (!this.controller.CombatantController.IsIdValid(combatant.ID))
				{
					array[i] = true;
					num--;
					foreach (Combatant combatant2 in factionCombatants)
					{
						if (!this.targets.Contains(combatant2))
						{
							this.targets[i] = combatant2;
							array[i] = false;
							num++;
							break;
						}
					}
				}
			}
			Combatant[] array3 = new Combatant[num];
			int k = 0;
			int num2 = 0;
			while (k < this.targets.Length)
			{
				if (!array[k])
				{
					array3[num2] = this.targets[k];
					num2++;
				}
				k++;
			}
			this.targets = array3;
		}

		protected override void UpdateAction()
		{
			base.UpdateAction();
			if (this.state == PlayerPsiAction.State.Initialize)
			{
				foreach (StatusEffectInstance statusEffectInstance in combatant.GetStatusEffects())
				{
					if (statusEffectInstance.Type == StatusEffect.DisablePSI)
					{
						string msg = string.Format("{0} tried {1} {2}!", CharacterNames.GetName(this.combatant.Character), this.psi.Name, PsiLetters.Get(this.psiLevel));
						this.controller.InterfaceController.ShowMessage(msg, false);
						this.controller.InterfaceController.ShowMessage("...But it failed!", false);
						this.complete = true;

						return;
					}
				}
					
				string message = string.Format("{0} tried {1} {2}!", CharacterNames.GetName(this.combatant.Character), this.psi.Name, PsiLetters.Get(this.psiLevel));
				this.RemoveInvalidTargets();

				

				this.controller.InterfaceController.OnTextboxComplete += this.OnTextboxComplete;
				this.controller.InterfaceController.ShowMessage(message, false);
				this.controller.InterfaceController.PrePsiSound.Play();
				this.controller.InterfaceController.PopCard(this.combatant.ID, 12);
				this.state = PlayerPsiAction.State.WaitForUI;
				return;
			}
			if (this.state == PlayerPsiAction.State.Animate)
			{
				PsiElementList animation = PsiAnimations.Get(this.psi);
				PsiAnimator psiAnimator = this.controller.InterfaceController.AddPsiAnimation(animation, this.sender, this.targets);
				psiAnimator.OnAnimationComplete += this.OnAnimationComplete;
				this.state = PlayerPsiAction.State.WaitForUI;
				return;
			}
			if (this.state == PlayerPsiAction.State.DamageNumbers)
			{
				foreach (Combatant combatant in this.targets)
				{
					//todo:
					//lifeup breaks the game because its effect is null
					int num = BattleCalculator.CalculatePsiDamage(this.psi.Effect[this.psiLevel][0], this.psi.Effect[this.psiLevel][1], this.sender, combatant);
					DamageNumber damageNumber = this.controller.InterfaceController.AddDamageNumber(combatant, num);
					damageNumber.OnComplete += this.OnDamageNumberComplete;
					StatSet statChange = new StatSet
					{
						HP = -num
					};
					combatant.AlterStats(statChange);
					if (combatant as EnemyCombatant != null)
					{
						this.controller.InterfaceController.BlinkEnemy(combatant as EnemyCombatant, 3, 2);
					}
				}
				StatSet statChange2 = new StatSet
				{
					PP = -this.psi.PP[this.psiLevel],
					Meter = 0.026666667f
				};
				this.sender.AlterStats(statChange2);
				this.state = PlayerPsiAction.State.WaitForUI;
				return;
			}
			if (this.state == PlayerPsiAction.State.Finish)
			{
				this.controller.InterfaceController.PopCard(this.combatant.ID, 0);
				this.complete = true;
			}
		}

		private void OnDamageNumberComplete(DamageNumber sender)
		{
			sender.OnComplete -= this.OnDamageNumberComplete;
			this.state = PlayerPsiAction.State.Finish;
		}

		private void OnTextboxComplete()
		{
			this.controller.InterfaceController.OnTextboxComplete -= this.OnTextboxComplete;
			this.state = PlayerPsiAction.State.Animate;
		}

		private void OnAnimationComplete(PsiAnimator anim)
		{
			anim.OnAnimationComplete -= this.OnAnimationComplete;
			this.state = PlayerPsiAction.State.DamageNumbers;
		}

		private const float ONE_GP = 0.013333334f;

		private const int CARD_POP_HEIGHT = 12;

		private const int DAMAGE_NUMBER_WAIT = 70;

		private const int PSI_INDEX = 0;

		private const int PSI_LEVEL_INDEX = 1;

		private PlayerPsiAction.State state;

		private PlayerCombatant combatant;

		private OffensePsi psi;

		private int psiLevel;

		private enum State
		{
			Initialize,
			Animate,
			WaitForUI,
			DamageNumbers,
			Finish
		}
	}
}

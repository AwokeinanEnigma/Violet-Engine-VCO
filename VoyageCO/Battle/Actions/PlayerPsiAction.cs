using System;
using System.Linq;
using VCO.AUX;
//using VCO.AUX;
using VCO.Battle.Combatants;
using VCO.Battle.UI;
using VCO.Data;
using VCO.SOMETHING;

namespace VCO.Battle.Actions
{
    internal class PlayerAUXAction : BattleAction
    {
        public AUXBase aux;
        public PlayerAUXAction(ActionParams aparams) : base(aparams)
        {
            Console.WriteLine("CONSTRUCTOR");
            this.combatant = (this.sender as PlayerCombatant);
            IAUX AUX = (aparams.data.Length > 0) ? ((IAUX)aparams.data[0]) : null;
            this.AUX = AUX;
            this.AUXLevel = ((aparams.data.Length > 1) ? ((int)aparams.data[1]) : 0);
            aux = ((aparams.data.Length > 1) ? ((AUXBase)aparams.data[2]) : null);

            this.state = PlayerAUXAction.State.Initialize;
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
            if (this.state == PlayerAUXAction.State.Initialize)
            {
                foreach (StatusEffectInstance statusEffectInstance in combatant.GetStatusEffects())
                {
                    if (statusEffectInstance.Type == StatusEffect.DisableAUX)
                    {
                        string msg = string.Format("{0} tried {1} {2}!", CharacterNames.GetName(this.combatant.Character), this.AUX.aux.QualifiedName, AUXLetters.Get(this.AUXLevel));
                        this.controller.InterfaceController.ShowMessage(msg, false);
                        this.controller.InterfaceController.ShowMessage("...But it failed!", false);
                        this.complete = true;

                        return;
                    }
                }

                this.RemoveInvalidTargets();
                aux.Initialize(combatant, controller.InterfaceController, this, targets, AUXLevel);
                return;
            }
            aux.Update(combatant, controller.InterfaceController, this, targets, AUXLevel);
            if (this.state == PlayerAUXAction.State.Animate)
            {
                aux.Animate(combatant, controller.InterfaceController, this, targets, AUXLevel);


                return;
            }
            if (this.state == PlayerAUXAction.State.DamageNumbers)
            {
                aux.Act(targets, combatant, controller.InterfaceController, this, AUXLevel);
                /*
				foreach (Combatant combatant in this.targets)
				{
					//todo:
					//lifeup breaks the game because its effect is null
					//int num = BattleCalculator.CalculateAUXDamage(this.AUX.Effect[this.AUXLevel][0], this.AUX.Effect[this.AUXLevel][1], this.sender, combatant);
					DamageNumber damageNumber = this.controller.InterfaceController.AddDamageNumber(combatant, 32);
					damageNumber.OnComplete += this.OnDamageNumberComplete;
					StatSet statChange = new StatSet
					{
						HP = -32
					};
					combatant.AlterStats(statChange);
					if (combatant as EnemyCombatant != null)
					{
						this.controller.InterfaceController.BlinkEnemy(combatant as EnemyCombatant, 3, 2);
					}
				}
				StatSet statChange2 = new StatSet
				{
					PP = -this.AUX.aux.AUCost,
					Meter = 0.026666667f
				};
				this.sender.AlterStats(statChange2);*/
                //this.state = PlayerAUXAction.State.WaitForUI;
                return;
            }
            if (this.state == PlayerAUXAction.State.Finish)
            {
                aux.Finish(targets, combatant, controller.InterfaceController, this, AUXLevel);
            }
        }

        private void OnDamageNumberComplete(DamageNumber sender)
        {
            sender.OnComplete -= this.OnDamageNumberComplete;
            this.state = PlayerAUXAction.State.Finish;
        }

        public void Finish()
        {
            this.complete = true;

        }



        private const float ONE_GP = 0.013333334f;

        private const int CARD_POP_HEIGHT = 12;

        private const int DAMAGE_NUMBER_WAIT = 70;

        private const int AUX_INDEX = 0;

        private const int AUX_LEVEL_INDEX = 1;

        public PlayerAUXAction.State state;

        private readonly PlayerCombatant combatant;

        public IAUX AUX;

        public int AUXLevel;

        public enum State
        {
            Initialize,
            Animate,
            WaitForUI,
            DamageNumbers,
            Finish
        }
    }
}

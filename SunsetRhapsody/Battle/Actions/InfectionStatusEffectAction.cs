using System;
using SunsetRhapsody.Battle.Combatants;
using SunsetRhapsody.Battle.UI;
using SunsetRhapsody.Data;
using SunsetRhapsody.Utility;

namespace SunsetRhapsody.Battle.Actions
{
	internal class InfectionStatusEffectAction : StatusEffectAction
	{
        public InfectionStatusEffectAction(ActionParams aparams) : base(aparams)
        {
            if (this.effect.TurnsRemaining > 1)
            {

                this.actionStartSound = this.controller.InterfaceController.EnemyDeathSound;
                actionStartSound.OnComplete += ActionStartSound_OnComplete;

                PlayerCombatant pCombat = sender as PlayerCombatant;
                if (pCombat != null)
                {
                    this.message = string.Format("@{0} is hurt by an infection!", new object[]
{
                            CharacterNames.GetName(pCombat.Character),
});


                }
                else
                {
                    foreach (Combatant combat in targets)
                    {
                        PlayerCombatant pCombat1 = combat as PlayerCombatant;
                        if (pCombat1 != null)
                        {
                            this.message = string.Format("@{0} felt their infection go away!", new object[]
                            {
                            CharacterNames.GetName(pCombat1.Character),
                            });
                        }
                    }
                }
            }
        }

        private void ActionStartSound_OnComplete(Violet.Audio.VioletSound not)
        {
            StatSet set = new StatSet()
            {
                HP = -10,
            };

            PlayerCombatant pCombat = sender as PlayerCombatant;
            if (pCombat != null)
            {
                DamageNumber ui = controller.InterfaceController.AddDamageNumber(pCombat, 10);
                ui.OnComplete += Ui_OnComplete;

                void Ui_OnComplete(DamageNumber sender)
                {
                    Console.Write("number");
                    pCombat.AlterStats(set);
                    Console.Write("alter numbers");
                    this.message = string.Format("@{0} is hurt by an infection!", new object[]
                    {
                            CharacterNames.GetName(pCombat.Character),
                    });
                }

            }
            else
            {
                foreach (Combatant combat in targets)
                {
                    PlayerCombatant pCombat1 = combat as PlayerCombatant;
                    if (pCombat1 != null)
                    {
                        this.message = string.Format("@{0} felt their infection go away!", new object[]
                        {
                            CharacterNames.GetName(pCombat1.Character),
                        });
                    }
                }
            }
        }
    }
}

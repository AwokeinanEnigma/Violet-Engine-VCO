using System;
using VCO.AUX;
using VCO.Battle;
using VCO.Battle.Actions;
using VCO.Battle.AUXAnimation;
using VCO.Battle.Combatants;
using VCO.Battle.UI;
using VCO.Data;

namespace VCO.SOMETHING
{
    public class Fire : AUXBase
    {
        public override int AUCost => 12; //{ get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public override TargetingMode TargetMode => TargetingMode.AllEnemies;//; set => throw new NotImplementedException(); }
        public override int[] Symbols => new int[2]; //{ get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public override string QualifiedName => "AUX Desync";//{ get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public override string Key => "1"; //{ get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        internal override IAUX identifier => new DefensiveAUX(); //{ get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public Fire()
        {
            Console.WriteLine("THE PURPOSE OF MAN IS TO BURN");
        }



        internal override void Initialize(PlayerCombatant combantant, BattleInterfaceController interfaceController, PlayerAUXAction action, Combatant[] targets, int level)
        {
            string message = string.Format("{0} tried {1} {2}!", CharacterNames.GetName(combantant.Character), QualifiedName, AUXLetters.Get(action.AUXLevel));

            action.state = PlayerAUXAction.State.WaitForUI;
            interfaceController.OnTextboxComplete += OnTextboxComplete;
            interfaceController.ShowMessage(message, false);
            interfaceController.PopCard(combantant.ID, 23);

            void OnTextboxComplete()
            {
                interfaceController.OnTextboxComplete -= OnTextboxComplete;
                action.state = PlayerAUXAction.State.Animate;
            }
            Console.WriteLine("initialize");
        }

        internal override void Animate(PlayerCombatant combantant, BattleInterfaceController interfaceController, PlayerAUXAction action, Combatant[] targets, int level)
        {
            AUXElementList animation = AUXAnimations.Get(7);
            AUXAnimator AUXAnimator = interfaceController.AddAUXAnimation(animation, combantant, targets);
            AUXAnimator.OnAnimationComplete += OnAnimationComplete;
            action.state = PlayerAUXAction.State.WaitForUI;

            void OnAnimationComplete(AUXAnimator anim)
            {
                anim.OnAnimationComplete -= OnAnimationComplete;
                action.state = PlayerAUXAction.State.DamageNumbers;
            }
        }

        internal override void Act(Combatant[] combatants, PlayerCombatant combantant, BattleInterfaceController interfaceController, PlayerAUXAction action, int level)
        {
            Console.WriteLine("act");
            foreach (Combatant combatant in combatants)
            {

                DamageNumber damageNumber = interfaceController.AddDamageNumber(combatant, 23);
                damageNumber.OnComplete += DamageNumber_OnComplete; ;
                StatSet statChange = new StatSet
                {
                    HP = -23
                };
                combatant.AlterStats(statChange);
                if (combatant as EnemyCombatant != null)
                {
                    interfaceController.BlinkEnemy(combatant as EnemyCombatant, 3, 2);
                }
            }
            StatSet statChange2 = new StatSet
            {
                PP = -this.AUCost,
                Meter = 0.026666667f
            };
            combantant.AlterStats(statChange2);
            action.state = PlayerAUXAction.State.WaitForUI;
            void DamageNumber_OnComplete(DamageNumber sender)
            {
                action.state = PlayerAUXAction.State.Finish;
            }
        }

        internal override void ScaleToLevel(PlayerCombatant combatant, int level)
        {
        }

        internal override void ShowUnavaliableMessage(PlayerCombatant combatant, BattleInterfaceController interfaceController)
        {
            interfaceController.ShowMessage("Not enough AU!", false);
        }

        internal override void Finish(Combatant[] combatants, PlayerCombatant combantant, BattleInterfaceController interfaceController, PlayerAUXAction action, int level)
        {
            interfaceController.PopCard(combantant.ID, 0);
            action.Finish();
        }
    }
}

using System;
using VCO.AUX;
using VCO.Battle;
using VCO.Battle.Actions;
using VCO.Battle.Combatants;
using VCO.Battle.UI;
using VCO.Data;
using Violet.Graphics;

namespace VCO.SOMETHING
{
    public class Test : AUXBase
    {
        public override int AUCost => 1; //{ get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public override TargetingMode TargetMode => TargetingMode.Enemy;//; set => throw new NotImplementedException(); }
        public override int[] Symbols => new int[2]; //{ get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public override string QualifiedName => "AUX Spark";//{ get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public override string Key => "1"; //{ get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        internal override IAUX identifier => new AssistiveAUX(); //{ get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public Test()
        {
            Console.WriteLine("THE PURPOSE OF MAN IS TO TEST FEATURES IN THEIR VIDEO GAMES");
        }



        internal override void Initialize(PlayerCombatant combantant, BattleInterfaceController interfaceController, PlayerAUXAction action, Combatant[] targets, int level)
        {
            string message = string.Format("{0} tried {1} {2}!", CharacterNames.GetName(combantant.Character), QualifiedName, AUXLetters.Get(action.AUXLevel));

            action.state = PlayerAUXAction.State.WaitForUI;
            interfaceController.OnTextboxComplete += OnTextboxComplete;
            interfaceController.ShowMessage(message, false);
            interfaceController.PopCard(combantant.ID, 20);

            void OnTextboxComplete()
            {
                interfaceController.OnTextboxComplete -= OnTextboxComplete;
                action.state = PlayerAUXAction.State.Animate;
            }
            Console.WriteLine("initialize");
        }

        internal override void Animate(PlayerCombatant combantant, BattleInterfaceController interfaceController, PlayerAUXAction action, Combatant[] targets, int level)
        {
            level += 1;
            string thing = "whoknowsssss.dat";
            switch (level)
            {

                case 1:
                    //do nothing
                    break;
                case 2:
                    thing = "other.dat";
                    break;



            }
            IndexedColorGraphic graphic = new IndexedColorGraphic(Paths.GRAPHICS + thing, "spark", interfaceController.GetEnemyGraphic((targets[0] as EnemyCombatant).ID).Position, 579600);

            interfaceController.pipeline.Add(graphic);
            graphic.OnAnimationComplete += Graphic_OnAnimationComplete;
            action.state = PlayerAUXAction.State.WaitForUI;

            void Graphic_OnAnimationComplete(AnimatedRenderable renderable)
            {
                interfaceController.pipeline.Remove(renderable);
                graphic.Visible = false;
                graphic.OnAnimationComplete -= Graphic_OnAnimationComplete;
                action.state = PlayerAUXAction.State.DamageNumbers;
            };
        }

        internal override void Act(Combatant[] combatants, PlayerCombatant combantant, BattleInterfaceController interfaceController, PlayerAUXAction action, int level)
        {
            Console.WriteLine("act");
            foreach (Combatant combatant in combatants)
            {


                level += 1;
                int dmg = -32 * level;
                DamageNumber damageNumber = interfaceController.AddDamageNumber(combatant, dmg);
                damageNumber.OnComplete += DamageNumber_OnComplete; ; Console.WriteLine("level is " + level + "");
                StatSet statChange = new StatSet
                {
                    HP = dmg
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

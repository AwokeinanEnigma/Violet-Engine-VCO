using SFML.System;
using SunsetRhapsody.Battle;
using SunsetRhapsody.Battle.Actions;
using SunsetRhapsody.Battle.Combatants;
using SunsetRhapsody.Battle.AUXAnimation;
using SunsetRhapsody.Battle.UI;
using SunsetRhapsody.Data;
using SunsetRhapsody.AUX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Violet.Graphics;

namespace SunsetRhapsody.SOMETHING
{
    public class LifeUp : AUXBase
    {
        public override int AUCost => 1; //{ get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public override TargetingMode TargetMode => TargetingMode.AllPartyMembers;//; set => throw new NotImplementedException(); }
        public override int[] Symbols => new int[2]; //{ get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public override string QualifiedName => "AUX Heal";//{ get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public override string Key => "1"; //{ get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        internal override IAUX identifier => new AssistiveAUX(); //{ get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public LifeUp()
        {
            Console.WriteLine("THE PURPOSE OF MAN IS TO HEAL");
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
            IndexedColorGraphic graphic = new IndexedColorGraphic(Paths.GRAPHICS + "AUXlifeup1.dat", "lifeupalpha", new Vector2f(160, 90), 579600);
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

                DamageNumber damageNumber = interfaceController.AddDamageNumber(combatant, 32, Paths.GRAPHICS + "numbersethealing.dat");
                damageNumber.OnComplete += DamageNumber_OnComplete; ;
                StatSet statChange = new StatSet
                {
                    HP = +32
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

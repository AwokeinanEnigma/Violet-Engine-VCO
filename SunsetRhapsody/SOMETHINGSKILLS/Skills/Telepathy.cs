using SunsetRhapsody.Battle;
using SunsetRhapsody.Battle.Actions;
using SunsetRhapsody.Battle.Combatants;
using SunsetRhapsody.Battle.PsiAnimation;
using SunsetRhapsody.Battle.UI;
using SunsetRhapsody.Data;
using SunsetRhapsody.Psi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SunsetRhapsody.SOMETHING
{
    public class Telepathy : AUXBase
    {
        public override int AUCost => 0; //{ get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public override TargetingMode TargetMode => TargetingMode.Enemy;//; set => throw new NotImplementedException(); }
        public override int[] Symbols => new int[2]; //{ get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public override string QualifiedName => "Telapathy";//{ get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public override string Key => "1"; //{ get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        internal override IPsi identifier => new AssistivePsi(); //{ get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public Telepathy()
        {
            Console.WriteLine("THE PURPOSE OF MAN IS TO PERFORM TELEPATHY");
        }



        internal override void Initialize(PlayerCombatant combantant, BattleInterfaceController interfaceController, PlayerPsiAction action, Combatant[] targets)
        {
            string message = string.Format("{0} tried {1} {2}!", CharacterNames.GetName(combantant.Character), QualifiedName, PsiLetters.Get(action.psiLevel));

            action.state = PlayerPsiAction.State.WaitForUI;
            interfaceController.OnTextboxComplete += OnTextboxComplete;
            interfaceController.ShowMessage(message, false);
            interfaceController.PopCard(combantant.ID, 23);

            void OnTextboxComplete()
            {
                interfaceController.OnTextboxComplete -= OnTextboxComplete;
                action.state = PlayerPsiAction.State.Animate;
            }
            Console.WriteLine("initialize");
        }

        internal override void Animate(PlayerCombatant combantant, BattleInterfaceController interfaceController, PlayerPsiAction action, Combatant[] targets)
        {
            // skip animation -- it's a textbox
            action.state = PlayerPsiAction.State.DamageNumbers;

        }

        internal override void Act(Combatant[] combatants, PlayerCombatant combantant, BattleInterfaceController interfaceController, PlayerPsiAction action)
        {
            Console.WriteLine("act");
            foreach (Combatant combatant in combatants)
            {
                EnemyCombatant enemy = combatant as EnemyCombatant;
                if (enemy == null)
                {
                    throw new Exception("Enemy don't work!");
                    return;
                }
                interfaceController.ShowStyledMessage($"{enemy.Enemy.GetStringQualifiedName("telepathy")}", true, Violet.GUI.WindowBox.Style.Telepathy);
                interfaceController.OnTextboxComplete += InterfaceController_OnTextboxComplete;
                void InterfaceController_OnTextboxComplete()
                {
                    interfaceController.OnTextboxComplete -= InterfaceController_OnTextboxComplete;
                    interfaceController.ResetTextboxStyle();
                    action.state = PlayerPsiAction.State.Finish;
                }
            }
            StatSet statChange2 = new StatSet
            {
                PP = -this.AUCost,
                Meter = 0.026666667f
            };
            combantant.AlterStats(statChange2);
            action.state = PlayerPsiAction.State.WaitForUI;

        }

        

        internal override void ScaleToLevel(PlayerCombatant combatant)
        {
        }

        internal override void ShowUnavaliableMessage(PlayerCombatant combatant, BattleInterfaceController interfaceController)
        {
            interfaceController.ShowMessage("Not enough AU!", false);
        }

        internal override void Finish(Combatant[] combatants, PlayerCombatant combantant, BattleInterfaceController interfaceController, PlayerPsiAction action)
        {
            interfaceController.PopCard(combantant.ID, 0);
            action.Finish();
        }
    }
}

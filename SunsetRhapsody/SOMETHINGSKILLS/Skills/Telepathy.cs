using SFML.Graphics;
using SFML.System;
using SunsetRhapsody.Battle;
using SunsetRhapsody.Battle.Actions;
using SunsetRhapsody.Battle.Combatants;
using SunsetRhapsody.Battle.AUXAnimation;
using SunsetRhapsody.Battle.UI;
using SunsetRhapsody.Data;
using SunsetRhapsody.GUI.Modifiers;
using SunsetRhapsody.AUX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Violet.Graphics;

namespace SunsetRhapsody.SOMETHING
{
    public class Telepathy : AUXBase
    {
        public override int AUCost => 0; 
        public override TargetingMode TargetMode => TargetingMode.Enemy;
        public override int[] Symbols => new int[2];
        public override string QualifiedName => "Telapathy";
        public override string Key => "1"; 
        internal override IAUX identifier => new AssistiveAUX(); 

        public Telepathy()
        {
            Console.WriteLine("THE PURPOSE OF MAN IS TO PERFORM TELEPATHY");
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
            action.state = PlayerAUXAction.State.DamageNumbers;
        }
        internal override void Act(Combatant[] combatants, PlayerCombatant combantant, BattleInterfaceController interfaceController, PlayerAUXAction action, int level)
        {
            Console.WriteLine("act");
            foreach (Combatant combatant in combatants)
            {
                EnemyCombatant enemy = combatant as EnemyCombatant;
                if (enemy == null)
                {               
                    throw new Exception("Combatant is not an enemy or somehow has null data!");
                    return;
                }
                interfaceController.ShowStyledMessage($"{enemy.Enemy.GetStringQualifiedName("telepathy")}", true, Violet.GUI.WindowBox.Style.Telepathy);
                interfaceController.OnTextboxComplete += InterfaceController_OnTextboxComplete;
                void InterfaceController_OnTextboxComplete()
                {
                    interfaceController.OnTextboxComplete -= InterfaceController_OnTextboxComplete;
                    interfaceController.ResetTextboxStyle();
                    action.state = PlayerAUXAction.State.Finish;
                }
            }
            StatSet statChange2 = new StatSet
            {
                PP = -this.AUCost,
                Meter = 0.026666667f
            };
            combantant.AlterStats(statChange2);
            action.state = PlayerAUXAction.State.WaitForUI;

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

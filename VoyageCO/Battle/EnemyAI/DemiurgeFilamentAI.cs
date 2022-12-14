using System.Collections.Generic;
using VCO.Battle.Actions;
using VCO.Battle.Combatants;
using VCO.Data.Enemies;
using Violet;

namespace VCO.Battle.EnemyAI
{
    internal class DemiurgeFilamentAI : IEnemyAI
    {
        public DemiurgeFilamentAI(BattleController controller, Combatant sender, EnemyData data)
        {
            this.controller = controller;
            this.sender = sender;
            //this.battleActionParams = EnemyBattleActions.GetBattleActionParams((sender as EnemyCombatant).Enemy);
        }

        public BattleAction GetAction(int priority, Combatant[] potentialTargets)
        {

            ActionParams aparams = this.battleActionParams[Engine.Random.Next(this.battleActionParams.Count)];
            aparams.controller = this.controller;
            aparams.sender = this.sender;
            aparams.priority = this.sender.Stats.Speed;
            //List<Combatant> targets = new List<Combatant>();

            aparams.targets = new Combatant[] { potentialTargets[Engine.Random.Next(potentialTargets.Length)] };

            return BattleAction.GetInstance(aparams);
        }

        private readonly List<ActionParams> battleActionParams = new List<ActionParams>() {
                        new ActionParams
                        {
                            actionType = typeof(EnemyTurnWasteAction),
                            data = new object[]
                            {
                                "The Demiurge Filament is glaring at you.",
                                true
                            }
                        },
                        new ActionParams
                        {
                            actionType = typeof(EnemyBashAction),
                            data = new object[]
                            {
                                5f,
                                true,
                                "glares at",
                            }
                        },
						//this is broken

		};

        private readonly BattleController controller;

        private readonly Combatant sender;
    }
}

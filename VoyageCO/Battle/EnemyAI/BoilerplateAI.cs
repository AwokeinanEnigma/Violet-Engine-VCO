using System;
using System.Collections.Generic;
using VCO.Battle.Actions;
using VCO.Battle.Combatants;
using VCO.Data;
using VCO.Data.Enemies;
using VCO.Utility;
using Violet;

namespace VCO.Battle.EnemyAI
{
    internal class BoilerplateAI : IEnemyAI
    {
        public BoilerplateAI(BattleController controller, Combatant sender, EnemyData data)
        {
            this.controller = controller;
            this.sender = sender;

            this.battleActionParams = new List<ActionParams>
            {
                new ActionParams
                {
                    actionType = typeof(EnemyTurnWasteAction),
                    data = new object[]
                    {
                        Capitalizer.Capitalize( data.Article) + data.PlayerFriendlyName + data.GetStringQualifiedName("wasteaction"),
                        false
                    }
                }
            };
        }

        public BattleAction GetAction(int priority, Combatant[] potentialTargets)
        {

            ActionParams aparams = this.battleActionParams[Engine.Random.Next(this.battleActionParams.Count)];
            aparams.controller = this.controller;
            aparams.sender = this.sender;
            aparams.priority = this.sender.Stats.Speed;

            Combatant combatant = null;
            List<Combatant> possibleTargets = null;


            if (!(aparams.actionType == typeof(DisableAUX)))
            {
                Console.WriteLine("Choosing to fuck up TRAVIS.");
                foreach (Combatant combatant2 in potentialTargets)
                {
                    if (combatant2.Faction == BattleFaction.PlayerTeam)
                    {
                        PlayerCombatant playerCombatant = combatant2 as PlayerCombatant;
                        if (playerCombatant.Character == CharacterType.Travis)
                        {
                            combatant = playerCombatant;
                            break;
                        }
                    }
                }
            }
            else
            {
                Console.WriteLine("Choosing to fuck up AUX.");
                possibleTargets = new List<Combatant>();
                foreach (Combatant combatant2 in potentialTargets)
                {
                    if (combatant2.Faction == BattleFaction.PlayerTeam)
                    {
                        PlayerCombatant playerCombatant = combatant2 as PlayerCombatant;
                        if (playerCombatant.Character != CharacterType.Travis)
                        {
                            possibleTargets.Add(playerCombatant);
                        }
                    }
                }
            }

            Combatant[] targets = new Combatant[]
            {
                (combatant != null) ? combatant : possibleTargets[Engine.Random.Next(possibleTargets.Count)]
            };
            aparams.targets = targets;

            return BattleAction.GetInstance(aparams);
        }

        private readonly List<ActionParams> battleActionParams;

        private readonly BattleController controller;

        private readonly Combatant sender;
    }
}

using System.Collections.Generic;
using VCO.Battle.Actions;
using VCO.Battle.Combatants;
using Violet;

namespace VCO.Battle.EnemyAI
{
    internal class RandomAI : IEnemyAI
    {
        public RandomAI(BattleController controller, Combatant sender)
        {
            this.controller = controller;
            this.sender = sender;
            //this.battleActionParams = EnemyBattleActions.GetBattleActionParams((sender as EnemyCombatant).Enemy);
        }

        public BattleAction GetAction(int priority, Combatant[] potentialTargets)
        {
            Combatant[] targets = new Combatant[]
            {
                potentialTargets[Engine.Random.Next(potentialTargets.Length)]
            };
            ActionParams aparams = this.battleActionParams[Engine.Random.Next(this.battleActionParams.Count)];
            aparams.controller = this.controller;
            aparams.sender = this.sender;
            aparams.priority = this.sender.Stats.Speed;
            aparams.targets = targets;
            return BattleAction.GetInstance(aparams);
        }

        private readonly List<ActionParams> battleActionParams;

        private readonly BattleController controller;

        private readonly Combatant sender;
    }
}

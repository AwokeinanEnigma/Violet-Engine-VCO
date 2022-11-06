using VCO.Battle.Actions;
using VCO.Battle.Combatants;

namespace VCO.Battle.EnemyAI
{
    internal interface IEnemyAI
    {
        BattleAction GetAction(int priority, Combatant[] potentialTargets);
    }
}

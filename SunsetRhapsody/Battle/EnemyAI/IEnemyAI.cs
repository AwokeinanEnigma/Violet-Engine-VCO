using System;
using SunsetRhapsody.Battle.Actions;
using SunsetRhapsody.Battle.Combatants;

namespace SunsetRhapsody.Battle.EnemyAI
{
	internal interface IEnemyAI
	{
		BattleAction GetAction(int priority, Combatant[] potentialTargets);
	}
}

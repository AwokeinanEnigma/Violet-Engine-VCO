using System;
using SunsetRhapsody.Battle.Actions;
using SunsetRhapsody.Data;
using SunsetRhapsody.Data.Enemies;

namespace SunsetRhapsody.Battle.Combatants
{
	internal class EnemyCombatant : Combatant
	{
		public EnemyData Enemy
		{
			get
			{
				return this.enemy;
			}
		}

		private EnemyDecisionAction action;


		public EnemyCombatant(EnemyData enemy) : base(BattleFaction.EnemyTeam)
		{
			this.enemy = enemy;
			this.stats = enemy.GetStatSet();
			action = new EnemyDecisionAction(new ActionParams
			{
				actionType = typeof(EnemyDecisionAction),
				controller = null,
				sender = this,
				priority = 0
			},
			enemy);
		}

		public override DecisionAction GetDecisionAction(BattleController controller, int priority, bool isFromUndo)
		{
			
			return new EnemyDecisionAction(new ActionParams
			{
				actionType = typeof(EnemyDecisionAction),
				controller = controller,
				sender = this,
				priority = priority
			},
		enemy);
		//	return action;
		}

		private EnemyData enemy;
	}
}

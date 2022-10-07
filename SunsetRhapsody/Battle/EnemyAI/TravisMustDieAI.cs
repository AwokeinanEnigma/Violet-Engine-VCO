using System;
using System.Collections.Generic;
using Violet;
using SunsetRhapsody.Battle.Actions;
using SunsetRhapsody.Battle.Combatants;
using SunsetRhapsody.Data;
using SunsetRhapsody.Data.Enemies;

namespace SunsetRhapsody.Battle.EnemyAI
{
	internal class TravisMustDieAI : IEnemyAI
	{
		public TravisMustDieAI(BattleController controller, Combatant sender, EnemyData data)
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

			Combatant combatant = null;
			List<Combatant> possibleTargets = null;


			if (!(aparams.actionType == typeof(DisablePSI)))
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
				Console.WriteLine("Choosing to fuck up PSI.");
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

		private List<ActionParams> battleActionParams = new List<ActionParams>() {
						new ActionParams
						{
							actionType = typeof(DisablePSIAction),
							data = new object[]
							{
								"a comet",
								23
							}
						},
						new ActionParams
						{
							actionType = typeof(EnemyProjectileAction),
							data = new object[]
							{
								"a comet",
								5
							}
						},
						new ActionParams
						{
							actionType = typeof(EnemyTurnWasteAction),
							data = new object[]
							{
								"The Modern Mind is having trouble thinking!",
								true
							}
						},
						new ActionParams
						{
							actionType = typeof(EnemyBashAction),
							data = new object[]
							{
								12f
							}
						}

		};

		private BattleController controller;

		private Combatant sender;
	}
}

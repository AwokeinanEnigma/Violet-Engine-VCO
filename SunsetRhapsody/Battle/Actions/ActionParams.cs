using System;
using SunsetRhapsody.Battle.Combatants;

namespace SunsetRhapsody.Battle.Actions
{
	internal struct ActionParams
	{
		public Type actionType;

		public BattleController controller;

		public Combatant sender;

		public Combatant[] targets;

		public int priority;

		public object[] data;
	}
}

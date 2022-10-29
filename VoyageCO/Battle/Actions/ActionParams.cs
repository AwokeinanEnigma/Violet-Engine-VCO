using System;
using VCO.Battle.Combatants;

namespace VCO.Battle.Actions
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

﻿using System;

namespace SunsetRhapsody.Battle.Actions
{
	internal abstract class DecisionAction : BattleAction
	{
		public DecisionAction(ActionParams aparams) : base(aparams)
		{
		}

		protected override void UpdateAction()
		{
			base.UpdateAction();
		}

		private const int DECIDER_INDEX = 0;
	}
}

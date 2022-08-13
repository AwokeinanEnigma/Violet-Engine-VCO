﻿using System;
using System.Collections.Generic;
using SunsetRhapsody.Scripts;
using SunsetRhapsody.Scripts.Actions;

namespace Rufini.Actions.Types
{
	internal class HopPlayerAction : RufiniAction
	{
		public HopPlayerAction()
		{
			this.paramList = new List<ActionParam>
			{
				new ActionParam
				{
					Name = "h",
					Type = typeof(int)
				},
				new ActionParam
				{
					Name = "col",
					Type = typeof(bool)
				}
			};
		}

		public override ActionReturnContext Execute(ExecutionContext context)
		{
			base.GetValue<string>("name");
			int value = base.GetValue<int>("h");
			base.GetValue<bool>("col");
			context.Player.HopFactor = (float)value;
			return default(ActionReturnContext);
		}
	}
}

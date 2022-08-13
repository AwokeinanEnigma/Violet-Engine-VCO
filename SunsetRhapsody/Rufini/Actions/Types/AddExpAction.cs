﻿using System;
using System.Collections.Generic;
using SunsetRhapsody.Battle;
using SunsetRhapsody.Data;
using SunsetRhapsody.Scripts;
using SunsetRhapsody.Scripts.Actions;
using SunsetRhapsody.Scripts.Actions.ParamTypes;

namespace Rufini.Actions.Types
{
	internal class AddExpAction : RufiniAction
	{
		public AddExpAction()
		{
			this.paramList = new List<ActionParam>
			{
				new ActionParam
				{
					Name = "char",
					Type = typeof(RufiniOption)
				},
				new ActionParam
				{
					Name = "val",
					Type = typeof(int)
				},
				new ActionParam
				{
					Name = "msg",
					Type = typeof(bool)
				},
				new ActionParam
				{
					Name = "sup",
					Type = typeof(bool)
				}
			};
		}

		public override ActionReturnContext Execute(ExecutionContext context)
		{
			CharacterType option = (CharacterType)base.GetValue<RufiniOption>("char").Option;
			int value = base.GetValue<int>("val");
			base.GetValue<bool>("msg");
			base.GetValue<bool>("sup");
			StatSet stats = CharacterStats.GetStats(option);
			stats.Experience += value;
			CharacterStats.SetStats(option, stats);
			Console.WriteLine("SORT OF IMPLEMENTED - BUG DAVE");
			return default(ActionReturnContext);
		}
	}
}

using System;
using System.Collections.Generic;
using Violet.Flags;
using SunsetRhapsody.Scripts;
using SunsetRhapsody.Scripts.Actions;
using SunsetRhapsody.Scripts.Actions.ParamTypes;

namespace Rufini.Actions.Types
{
	internal class TimeAction : RufiniAction
	{
		public TimeAction()
		{
			this.paramList = new List<ActionParam>
			{
				new ActionParam
				{
					Name = "time",
					Type = typeof(RufiniOption)
				}
			};
		}

		public override ActionReturnContext Execute(ExecutionContext context)
		{
			RufiniOption value = base.GetValue<RufiniOption>("time");
			FlagManager.Instance[1] = (value.Option != 0);
			return default(ActionReturnContext);
		}
	}
}

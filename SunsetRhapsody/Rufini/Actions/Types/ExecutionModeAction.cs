using System;
using System.Collections.Generic;
using SunsetRhapsody.Scripts;
using SunsetRhapsody.Scripts.Actions;
using SunsetRhapsody.Scripts.Actions.ParamTypes;

namespace Rufini.Actions.Types
{
	internal class ExecutionModeAction : RufiniAction
	{
		public ExecutionModeAction()
		{
			this.paramList = new List<ActionParam>
			{
				new ActionParam
				{
					Name = "mode",
					Type = typeof(RufiniOption)
				}
			};
		}

		public override ActionReturnContext Execute(ExecutionContext context)
		{
			Console.WriteLine("NOT IMPLEMENTED - BUG DAVE");
			return default(ActionReturnContext);
		}
	}
}

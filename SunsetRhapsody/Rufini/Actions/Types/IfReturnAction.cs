using System;
using System.Collections.Generic;
using SunsetRhapsody.Scripts;
using SunsetRhapsody.Scripts.Actions;
using SunsetRhapsody.Scripts.Actions.ParamTypes;

namespace Rufini.Actions.Types
{
	internal class IfReturnAction : RufiniAction
	{
		public IfReturnAction()
		{
			this.paramList = new List<ActionParam>
			{
				new ActionParam
				{
					Name = "op",
					Type = typeof(RufiniOption)
				},
				new ActionParam
				{
					Name = "val",
					Type = typeof(int)
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

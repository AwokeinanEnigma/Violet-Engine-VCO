using System;
using System.Collections.Generic;
using SunsetRhapsody.Scripts;
using SunsetRhapsody.Scripts.Actions;

namespace Rufini.Actions.Types
{
	internal class MapMarkClearAction : RufiniAction
	{
		public MapMarkClearAction()
		{
			this.paramList = new List<ActionParam>
			{
				new ActionParam
				{
					Name = "map",
					Type = typeof(string)
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

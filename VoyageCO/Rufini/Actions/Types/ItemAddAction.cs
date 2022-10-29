using System;
using System.Collections.Generic;
using VCO.Scripts;
using VCO.Scripts.Actions;
using VCO.Scripts.Actions.ParamTypes;

namespace Rufini.Actions.Types
{
	internal class ItemAddAction : RufiniAction
	{
		public ItemAddAction()
		{
			this.paramList = new List<ActionParam>
			{
				new ActionParam
				{
					Name = "inv",
					Type = typeof(RufiniOption)
				},
				new ActionParam
				{
					Name = "item",
					Type = typeof(int)
				},
				new ActionParam
				{
					Name = "msg",
					Type = typeof(bool)
				},
				new ActionParam
				{
					Name = "sfx",
					Type = typeof(bool)
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

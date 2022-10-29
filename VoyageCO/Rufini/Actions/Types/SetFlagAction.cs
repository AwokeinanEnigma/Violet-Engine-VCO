using System;
using System.Collections.Generic;
using Violet.Flags;
using VCO.Scripts;
using VCO.Scripts.Actions;

namespace Rufini.Actions.Types
{
	internal class SetFlagAction : RufiniAction
	{
		public SetFlagAction()
		{
			this.paramList = new List<ActionParam>
			{
				new ActionParam
				{
					Name = "flg",
					Type = typeof(int)
				},
				new ActionParam
				{
					Name = "val",
					Type = typeof(bool)
				}
			};
		}

		public override ActionReturnContext Execute(ExecutionContext context)
		{
			int value = base.GetValue<int>("flg");
			bool value2 = base.GetValue<bool>("val");
			FlagManager.Instance[value] = value2;
			return default(ActionReturnContext);
		}
	}
}

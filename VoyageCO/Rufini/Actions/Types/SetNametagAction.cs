using System;
using System.Collections.Generic;
using VCO.Scripts;
using VCO.Scripts.Actions;
using VCO.Scripts.Actions.ParamTypes;
using Rufini.Strings;

namespace Rufini.Actions.Types
{
	internal class SetNametagAction : RufiniAction
	{
		public SetNametagAction()
		{
			this.paramList = new List<ActionParam>
			{
				new ActionParam
				{
					Name = "text",
					Type = typeof(RufiniString)
				}
			};
		}

		public override ActionReturnContext Execute(ExecutionContext context)
		{
			RufiniString value = base.GetValue<RufiniString>("text");
			context.Nametag = StringFile.Instance.Get(value.Names).Value;
			return default(ActionReturnContext);
		}
	}
}

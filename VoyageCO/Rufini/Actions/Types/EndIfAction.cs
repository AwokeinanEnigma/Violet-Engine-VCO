using System;
using System.Collections.Generic;
using VCO.Scripts;
using VCO.Scripts.Actions;

namespace Rufini.Actions.Types
{
	internal class EndIfAction : RufiniAction
	{
		public EndIfAction()
		{
			this.paramList = new List<ActionParam>();
		}

		public override ActionReturnContext Execute(ExecutionContext context)
		{
			return default(ActionReturnContext);
		}
	}
}

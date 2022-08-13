using System;
using System.Collections.Generic;
using SunsetRhapsody.Scripts;
using SunsetRhapsody.Scripts.Actions;

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

using System;
using System.Collections.Generic;
using SunsetRhapsody.Scripts;
using SunsetRhapsody.Scripts.Actions;

namespace Rufini.Actions.Types
{
	internal class ElseAction : RufiniAction
	{
		public ElseAction()
		{
			this.paramList = new List<ActionParam>();
		}

		public override ActionReturnContext Execute(ExecutionContext context)
		{
			context.Executor.JumpToElseOrEndIf();
			return default(ActionReturnContext);
		}
	}
}

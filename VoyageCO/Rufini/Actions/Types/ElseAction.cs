using System;
using System.Collections.Generic;
using VCO.Scripts;
using VCO.Scripts.Actions;

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

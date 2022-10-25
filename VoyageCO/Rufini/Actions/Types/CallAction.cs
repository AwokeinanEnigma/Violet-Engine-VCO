using System;
using System.Collections.Generic;
using VCO.Scripts;
using VCO.Scripts.Actions;

namespace Rufini.Actions.Types
{
	internal class CallAction : RufiniAction
	{
		public CallAction()
		{
			this.paramList = new List<ActionParam>
			{
				new ActionParam
				{
					Name = "scr",
					Type = typeof(string)
				}
			};
		}

		public override ActionReturnContext Execute(ExecutionContext context)
		{
			string value = base.GetValue<string>("scr");
			Script? script = ScriptLoader.Load(value);
			if (script != null)
			{
				context.Executor.PushScript(script.Value);
			}
			return default(ActionReturnContext);
		}
	}
}

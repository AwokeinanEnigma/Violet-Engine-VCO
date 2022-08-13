using System;
using System.Collections.Generic;

namespace SunsetRhapsody.Scripts.Actions
{
	internal struct ActionReturnContext
	{
		public ScriptExecutor.WaitType Wait { get; set; }

		public Dictionary<string, object> Data { get; set; }
	}
}

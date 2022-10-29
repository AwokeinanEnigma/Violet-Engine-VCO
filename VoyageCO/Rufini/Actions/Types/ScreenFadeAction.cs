using System;
using System.Collections.Generic;
using Violet.Scenes;
using VCO.Scenes;
using VCO.Scripts;
using VCO.Scripts.Actions;
using VCO.Utility;
using SFML.Graphics;

namespace Rufini.Actions.Types
{
	internal class ScreenFadeAction : RufiniAction
	{
		public ScreenFadeAction()
		{
			this.paramList = new List<ActionParam>
			{
				new ActionParam
				{
					Name = "col",
					Type = typeof(Color)
				},
				new ActionParam
				{
					Name = "dur",
					Type = typeof(int)
				},
				new ActionParam
				{
					Name = "blk",
					Type = typeof(bool)
				}
			};
		}

		public override ActionReturnContext Execute(ExecutionContext context)
		{
			ActionReturnContext result = default(ActionReturnContext);
			Color value = base.GetValue<Color>("col");
			int value2 = base.GetValue<int>("dur");
			bool value3 = base.GetValue<bool>("blk");
			Scene scene = SceneManager.Instance.Peek();
			if (scene is OverworldScene)
			{
				this.dimmer = ((OverworldScene)scene).Dimmer;
				this.dimmer.ChangeColor(value, value2);
				if (value3)
				{
					this.dimmer.OnFadeComplete += this.OnFadeComplete;
					result.Wait = ScriptExecutor.WaitType.Event;
					this.context = context;
				}
			}
			return result;
		}

		private void OnFadeComplete(ScreenDimmer sender)
		{
			this.dimmer.OnFadeComplete -= this.OnFadeComplete;
			this.context.Executor.Continue();
		}

		private ExecutionContext context;

		private ScreenDimmer dimmer;
	}
}

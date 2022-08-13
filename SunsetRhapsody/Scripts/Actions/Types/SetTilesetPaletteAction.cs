using System;
using System.Collections.Generic;
using Violet.Scenes;
using SunsetRhapsody.Scenes;

namespace SunsetRhapsody.Scripts.Actions.Types
{
	internal class SetTilesetPaletteAction : RufiniAction
	{
		public SetTilesetPaletteAction()
		{
			this.paramList = new List<ActionParam>
			{
				new ActionParam
				{
					Name = "pal",
					Type = typeof(int)
				}
			};
		}

		public override ActionReturnContext Execute(ExecutionContext context)
		{
			int value = base.GetValue<int>("pal");
			Scene scene = SceneManager.Instance.Peek();
			if (scene is OverworldScene)
			{
				OverworldScene overworldScene = (OverworldScene)scene;
				overworldScene.SetTilesetPalette(value);
			}
			return default(ActionReturnContext);
		}
	}
}

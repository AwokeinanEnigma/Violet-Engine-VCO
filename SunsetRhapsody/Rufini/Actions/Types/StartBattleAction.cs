using System;
using System.Collections.Generic;
using Violet.Scenes;
using Violet.Scenes.Transitions;
using SunsetRhapsody.Data;
using SunsetRhapsody.Data.Enemies;
using SunsetRhapsody.Scenes;
using SunsetRhapsody.Scenes.Transitions;
using SunsetRhapsody.Scripts;
using SunsetRhapsody.Scripts.Actions;
using SunsetRhapsody.Scripts.Actions.ParamTypes;
using SFML.Graphics;

namespace Rufini.Actions.Types
{
	internal class StartBattleAction : RufiniAction
	{
		public StartBattleAction()
		{
			this.paramList = new List<ActionParam>
			{
				new ActionParam
				{
					Name = "type",
					Type = typeof(RufiniOption)
				},
				new ActionParam
				{
					Name = "enm",
					Type = typeof(int[])
				},
				new ActionParam
				{
					Name = "bgm",
					Type = typeof(int)
				},
				new ActionParam
				{
					Name = "bbg",
					Type = typeof(int)
				}
			};
		}

		public override ActionReturnContext Execute(ExecutionContext context)
		{
			RufiniOption value = base.GetValue<RufiniOption>("type");
			bool letterboxing = value.Option != 3;
			int value2 = base.GetValue<int>("bgm");
			int value3 = base.GetValue<int>("bbg");
			string[] value4 = base.GetValue<string[]>("enm");
			if (value4 != null && value4.Length > 0)
			{
                EnemyData[] array = new EnemyData[value4.Length];
				for (int i = 0; i < value4.Length; i++)
				{
					array[i] = EnemyFile.Instance.GetEnemyData(value4[i]);
				}
				BattleScene newScene = new BattleScene(array, letterboxing, value2, value3);
				ITransition transition;
				switch (value.Option)
				{
				case 0:
					transition = new BattleFadeTransition(1f, Color.Blue);
					break;
				case 1:
					transition = new BattleFadeTransition(1f, Color.Green);
					break;
				case 2:
					transition = new BattleFadeTransition(1f, Color.Red);
					break;
				default:
					transition = new BattleFadeTransition(1f, Color.Black);
					break;
				}
				transition.Blocking = true;
				SceneManager.Instance.Transition = transition;
				SceneManager.Instance.Push(newScene);
			}
			else
			{
				Console.WriteLine("There were no enemies specified.");
			}
			return default(ActionReturnContext);
		}

		private const int NORMAL_MODE = 0;

		private const int PLAYER_ADV_MODE = 1;

		private const int ENEMY_ADV_MODE = 2;

		private const int BOSS_MODE = 3;
	}
}

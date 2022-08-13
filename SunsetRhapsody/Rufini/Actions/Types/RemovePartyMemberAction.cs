using System;
using System.Collections.Generic;
using Violet.Maps;
using Violet.Scenes;
using SunsetRhapsody.Actors;
using SunsetRhapsody.Actors.NPCs;
using SunsetRhapsody.Data;
using SunsetRhapsody.Overworld;
using SunsetRhapsody.Scenes;
using SunsetRhapsody.Scripts;
using SunsetRhapsody.Scripts.Actions;
using SunsetRhapsody.Scripts.Actions.ParamTypes;

namespace Rufini.Actions.Types
{
	internal class RemovePartyMemberAction : RufiniAction
	{
		public RemovePartyMemberAction()
		{
			this.paramList = new List<ActionParam>
			{
				new ActionParam
				{
					Name = "char",
					Type = typeof(RufiniOption)
				},
				new ActionParam
				{
					Name = "name",
					Type = typeof(string)
				}
			};
		}

		public override ActionReturnContext Execute(ExecutionContext context)
		{
			CharacterType option = (CharacterType)base.GetValue<RufiniOption>("char").Option;
			string value = base.GetValue<string>("name");
			Scene scene = SceneManager.Instance.Peek();
			if (scene is OverworldScene)
			{
				PartyTrain partyTrain = ((OverworldScene)scene).PartyTrain;
				IList<PartyFollower> list = partyTrain.Remove(option);
				if (list.Count > 0)
				{
					PartyFollower partyFollower = list[0];
					Map.NPC npcData = new Map.NPC
					{
						Name = value,
						Solid = true,
						Shadow = true,
						X = (int)partyFollower.Position.X,
						Y = (int)partyFollower.Position.Y,
						DepthOverride = int.MinValue,
						Direction = (short)partyFollower.Direction,
						Sprite = CharacterGraphics.GetFile(partyFollower.Character, false)
					};
					NPC actor = new NPC(context.Pipeline, context.CollisionManager, npcData, null);
					context.ActorManager.Add(actor);
					partyFollower.Dispose();
				}
			}
			PartyManager.Instance.Remove(option);
			return default(ActionReturnContext);
		}
	}
}

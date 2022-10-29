using System;
using System.Collections.Generic;
using Violet.Maps;
using Violet.Scenes;
using VCO.Actors;
using VCO.Actors.NPCs;
using VCO.Data;
using VCO.Overworld;
using VCO.Scenes;
using VCO.Scripts;
using VCO.Scripts.Actions;
using VCO.Scripts.Actions.ParamTypes;

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

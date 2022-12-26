using Rufini.Strings;
using SFML.System;
using System.Collections.Generic;
using VCO.Actors;
using VCO.Actors.NPCs;
using VCO.Data;
using VCO.GUI.Text.PrintActions;
using VCO.Overworld;
using VCO.Scenes;
using VCO.Scripts;
using VCO.Scripts.Actions;
using VCO.Scripts.Actions.ParamTypes;
using Violet.Actors;
using Violet.Scenes;

namespace Rufini.Actions.Types
{
    internal class AddPartyMemberAction : RufiniAction
    {
        public override string Code => "APRT";
        public AddPartyMemberAction()
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
                },
                new ActionParam
                {
                    Name = "sup",
                    Type = typeof(bool)
                }
            };
        }

        public override ActionReturnContext Execute(ExecutionContext context)
        {
            ActionReturnContext result = default(ActionReturnContext);
            CharacterType option = (CharacterType)base.GetValue<RufiniOption>("char").Option;
            string npcName = base.GetValue<string>("name");
            bool value = base.GetValue<bool>("sup");
            PartyManager.Instance.Add(option);
            Scene scene = SceneManager.Instance.Peek();
            if (scene is OverworldScene)
            {
                NPC npc = (NPC)context.ActorManager.Find((Actor x) => x is NPC && ((NPC)x).Name == npcName);
                Vector2f position;
                int direction;
                if (npc != null)
                {
                    position = npc.Position;
                    direction = npc.Direction;
                    context.ActorManager.Remove(npc);
                    context.CollisionManager.Remove(npc);
                }
                else
                {
                    position = context.Player.Position;
                    direction = context.Player.Direction;
                }
                PartyTrain partyTrain = ((OverworldScene)scene).PartyTrain;
                PartyFollower partyFollower = new PartyFollower(context.Pipeline, context.CollisionManager, partyTrain, option, position, direction, true);
                partyTrain.Add(partyFollower);
            }
            if (!value)
            {
                this.context = context;
                string text = StringFile.Instance.Get("system.partyJoin").Value;
                text = text.Replace("$newMember", CharacterNames.GetName(option));
                this.context.TextBox.OnTextboxComplete += this.ContinueAfterTextbox;
                this.context.TextBox.Enqueue(new PrintAction(PrintActionType.PrintText, text));
                this.context.TextBox.Enqueue(new PrintAction(PrintActionType.Prompt, new object[0]));
                this.context.TextBox.Show();
                result = new ActionReturnContext
                {
                    Wait = ScriptExecutor.WaitType.Event
                };
            }
            return result;
        }

        private void ContinueAfterTextbox()
        {
            this.context.TextBox.Hide();
            this.context.TextBox.OnTextboxComplete -= this.ContinueAfterTextbox;
            this.context.Player.MovementLocked = false;
            this.context.Executor.Continue();
        }

        private const string MSG_KEY = "system.partyJoin";

        private ExecutionContext context;
    }
}

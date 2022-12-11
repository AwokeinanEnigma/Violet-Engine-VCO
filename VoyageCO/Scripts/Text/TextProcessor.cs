using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Violet.Flags;
using Violet.Utility;
using Rufini.Strings;
using SFML.Graphics;
using VCO.Data;
using VCO.Scripts.Actions.ParamTypes;
using VCO.Utility;
using VCO.GUI.Text.PrintActions;

namespace VCO.GUI.Text
{
    internal class TextProcessor
	{
		public IList<PrintAction> Actions
		{
			get
			{
				return this.Process();
			}
		}

		public TextProcessor(string text, Dictionary<string, string> contextualReplacements) : this(text)
		{
			this.contextualDict = contextualReplacements;
		}

		public TextProcessor(string text)
		{
			this.text = text;
			this.state = TextProcessor.ReadState.Text;
			this.actions = new List<PrintAction>();
			this.readIndex = 0;
		}

		public static string ProcessReplacements(string text)
		{
			return TextProcessor.ProcessReplacements(text, null);
		}

		public static string ProcessReplacements(string text, Dictionary<string, string> contextualReplacements)
		{
			StringBuilder stringBuilder = new StringBuilder(text ?? string.Empty);
			stringBuilder = stringBuilder.Replace("\r", "");
			int num = 0;
			MatchCollection matchCollection = Regex.Matches(stringBuilder.ToString(), "\\[([a-zA-Z][a-zA-Z0-9]*):?(\\b[^\\]]*)\\]");
			foreach (object obj in matchCollection)
			{
				Match match = (Match)obj;
				string value = match.Groups[1].Value;
				string value2 = match.Groups[2].Value;
				string[] array = value2.Split(new char[]
				{
					','
				});
				for (int i = 0; i < array.Length; i++)
				{
					array[i] = array[i].Trim();
				}
				string text2 = null;
				string key;
				switch (key = value)
				{
					case "cn":
						{
							CharacterType character;
							Enum.TryParse<CharacterType>(value2, out character);
							text2 = CharacterNames.GetName(character);
							break;
						}
					case "travis":
						text2 = CharacterNames.GetName(CharacterType.Travis);
						break;
					case "floyd":
						text2 = CharacterNames.GetName(CharacterType.Floyd);
						break;
					case "meryl":
						text2 = CharacterNames.GetName(CharacterType.Meryl);
						break;
					case "leo":
						text2 = CharacterNames.GetName(CharacterType.Leo);
						break;
					case "zack":
						text2 = CharacterNames.GetName(CharacterType.Zack);
						break;
					case "renee":
						text2 = CharacterNames.GetName(CharacterType.Renee);
						break;
					case "leader":
						{
							CharacterType character2 = PartyManager.Instance[0];
							text2 = CharacterNames.GetName(character2);
							break;
						}
					case "party":
						text2 = "unimpl"; // CharacterNames.GetGroup(PartyManager.Instance.ToArray());
						break;
					case "money":
						{
							RufiniString rufiniString = StringFile.Instance.Get("system.currency");
							int num3 = ValueManager.Instance[1];
							text2 = string.Format("{0}{1}", rufiniString.Value ?? string.Empty, num3);
							break;
						}
					case "str":
						{
							text2 = string.Empty;
							RufiniString rufiniString2 = StringFile.Instance.Get(array[0]);
							if (rufiniString2.Value != null)
							{
								text2 = rufiniString2.Value;
							}
							break;
						}
					case "ctx":
						{
							text2 = string.Empty;
							string text3;
							if (contextualReplacements != null && array.Length > 0 && contextualReplacements.TryGetValue(array[0], out text3))
							{
								RufiniString rufiniString3 = StringFile.Instance.Get(text3);
								if (rufiniString3.Value != null)
								{
									if (array.Length > 1 && array[1] == "true")
									{
										text2 = Capitalizer.Capitalize(rufiniString3.Value);
									}
									else
									{
										text2 = rufiniString3.Value;
									}
								}
								else
								{
									text2 = text3;
								}
							}
							break;
						}
				}
				if (text2 != null)
				{
					int num4 = match.Index - num;
					stringBuilder = stringBuilder.Remove(num4, match.Length);
					stringBuilder = stringBuilder.Insert(num4, text2);
					num += match.Length - text2.Length;
				}
			}
			return stringBuilder.ToString();
		}

		private void AddAction(PrintAction action)
		{
			this.actions.Add(action);
			string text;
			if (action.Data is object[])
			{
				object[] array = (object[])action.Data;
				text = string.Empty;
				for (int i = 0; i < array.Length; i++)
				{
					text = text + "\"" + array[i].ToString() + "\", ";
				}
			}
			else
			{
				text = action.Data.ToString();
			}
			Console.WriteLine("Added print action:\n\tType:\t{0}\n\tData:\t{1}", Enum.GetName(typeof(PrintActionType), action.Type), text);
		}

		private void ProcessText()
		{
			int num = this.text.IndexOf('[', this.readIndex);
			if (num == -1)
			{
				num = this.text.Length;
			}
			while (this.readIndex < num)
			{
				int num2 = this.text.IndexOf('\n', this.readIndex);
				bool flag = true;
				if (num2 == -1 || num2 > num)
				{
					num2 = num;
					flag = false;
				}
				if (num2 - this.readIndex > 0)
				{
					PrintAction action = new PrintAction(PrintActionType.PrintText, this.text.Substring(this.readIndex, num2 - this.readIndex));
					this.AddAction(action);
				}
				if (flag)
				{
					PrintAction action2 = new PrintAction(PrintActionType.LineBreak, new object[0]);
					this.AddAction(action2);
				}
				this.readIndex = num2 + 1;
			}
			this.readIndex = num;
			this.state = TextProcessor.ReadState.Tag;
		}

		private void AddActionByTagName(string tagName, string[] args)
		{
			PrintAction? printAction = null;
			switch (tagName)
			{
				case "p":
					{
						int num2 = 0;
						int.TryParse(args[0], out num2);
						printAction = new PrintAction?(new PrintAction(PrintActionType.Pause, num2));
						goto IL_1FD;
					}
				case "t":
					printAction = new PrintAction?(new PrintAction(PrintActionType.Trigger, args));
					goto IL_1FD;
				case "g":
					printAction = new PrintAction?(new PrintAction(PrintActionType.PrintGraphic, args[0]));
					goto IL_1FD;
				case "c":
					{
						Color color = ColorHelper.FromHexString(args[0]);
						printAction = new PrintAction?(new PrintAction(PrintActionType.Color, color));
						goto IL_1FD;
					}
				case "b":
					printAction = new PrintAction?(new PrintAction(PrintActionType.Prompt, new object[0]));
					goto IL_1FD;
				case "q":
					printAction = new PrintAction?(new PrintAction(PrintActionType.PromptQuestion, args));
					goto IL_1FD;
				case "i":
					{
						int num3 = 0;
						int.TryParse(args[0], out num3);
						int num4 = 0;
						int.TryParse(args[1], out num4);
						printAction = new PrintAction?(new PrintAction(PrintActionType.PromptNumeric, new object[]
						{
					num3,
					num4
						}));
						goto IL_1FD;
					}
				case "l":
					printAction = new PrintAction?(new PrintAction(PrintActionType.PromptList, args));
					goto IL_1FD;
				case "ts":
					{
						int num5 = 0;
						int.TryParse(args[0], out num5);
						printAction = new PrintAction?(new PrintAction(PrintActionType.Sound, num5));
						goto IL_1FD;
					}
			}
			Console.WriteLine("UNKNOWN TAG: {0}", tagName);
		IL_1FD:
			if (printAction != null)
			{
				this.AddAction(printAction.Value);
			}
		}

		private void ProcessTag()
		{
			int num = this.text.IndexOf(']', this.readIndex);
			if (num == -1)
			{
				num = this.text.Length;
			}
			this.readIndex++;
			int num2 = this.text.IndexOf(':', this.readIndex);
			string tagName;
			string[] args;
			if (num2 != -1)
			{
				tagName = this.text.Substring(this.readIndex, num2 - this.readIndex);
				string text = this.text.Substring(num2 + 1, num - (num2 + 1));
				args = (text.Contains(',') ? text.Split(new char[]
				{
					','
				}) : new string[]
				{
					text
				});
			}
			else
			{
				tagName = this.text.Substring(this.readIndex, num - this.readIndex);
				args = null;
			}
			this.AddActionByTagName(tagName, args);
			this.readIndex = num + 1;
			this.state = TextProcessor.ReadState.Text;
		}

		public IList<PrintAction> Process()
		{
			if (this.state != TextProcessor.ReadState.Done)
			{
				this.text = TextProcessor.ProcessReplacements(this.text, this.contextualDict);
				while (this.readIndex < this.text.Length)
				{
					switch (this.state)
					{
						case TextProcessor.ReadState.Text:
							this.ProcessText();
							break;
						case TextProcessor.ReadState.Tag:
							this.ProcessTag();
							break;
					}
				}
				this.state = TextProcessor.ReadState.Done;
			}
			return this.actions;
		}

		private const char NEWLINE_CHAR = '\n';

		private const char BULLET_CHAR = '@';

		private const char SPACE_CHAR = ' ';

		private const int PAUSE_CHAR_DURATION = 10;

		private const int BULLET_PAUSE_DURATION = 30;

		private const string SINGLE_CMD_REGEX = "\\[([a-zA-Z][a-zA-Z0-9]*):?(\\b[^\\]]*)\\]";

		private const string TRUE_STRING = "true";

		private const char TAG_START = '[';

		private const char TAG_END = ']';

		private const char TAG_COLON = ':';

		private const char TAG_SEPARATOR_CHAR = ',';

		private const string CMD_PAUSE = "p";

		private const string CMD_TRIGGER = "t";

		private const string CMD_GRAPHIC = "g";

		private const string CMD_COLOR = "c";

		private const string CMD_PROMPT = "b";

		private const string CMD_QUESTION = "q";

		private const string CMD_INPUT = "i";

		private const string CMD_LIST = "l";

		private const string CMD_SOUND = "ts";

		private const string CMD_CHARNAME = "cn";

		private const string CMD_TRAVIS = "travis";

		private const string CMD_FLOYD = "floyd";

		private const string CMD_MERYL = "meryl";

		private const string CMD_LEO = "leo";

		private const string CMD_ZACK = "zack";

		private const string CMD_RENEE = "renee";

		private const string CMD_PARTY_LEAD = "leader";

		private const string CMD_PARTY_GROUP = "party";

		private const string CMD_MONEY = "money";

		private const string CMD_STRING = "str";

		private const string CMD_CONTEXTUAL = "ctx";

		private string text;

		private TextProcessor.ReadState state;

		private IList<PrintAction> actions;

		private int readIndex;

		private Dictionary<string, string> contextualDict;

		private enum ReadState
		{
			Text,
			Tag,
			Done
		}
	}
}

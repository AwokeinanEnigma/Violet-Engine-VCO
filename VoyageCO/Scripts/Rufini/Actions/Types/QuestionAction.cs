using Rufini.Strings;
using System.Collections.Generic;
using VCO.Actors.NPCs;
using VCO.GUI.Text.PrintActions;
using VCO.Scripts;
using VCO.Scripts.Actions;
using VCO.Scripts.Actions.ParamTypes;
using Violet.Flags;

namespace Rufini.Actions.Types
{
	internal class QuestionAction : RufiniAction
	{
		public override string Code => "QSTN";
		public QuestionAction()
		{
			this.paramList = new List<ActionParam>
			{
				new ActionParam
				{
					Name = QuestionAction.OPT_NAMES[0],
					Type = typeof(RufiniString)
				},
				new ActionParam
				{
					Name = QuestionAction.OPT_NAMES[1],
					Type = typeof(RufiniString)
				},
				new ActionParam
				{
					Name = QuestionAction.OPT_NAMES[2],
					Type = typeof(RufiniString)
				},
				new ActionParam
				{
					Name = QuestionAction.OPT_NAMES[3],
					Type = typeof(RufiniString)
				},
				new ActionParam
				{
					Name = QuestionAction.OPT_NAMES[4],
					Type = typeof(RufiniString)
				}
			};
		}

		public override ActionReturnContext Execute(ExecutionContext context)
		{
			this.context = context;
			int num = 0;
			for (int i = 0; i < QuestionAction.OPT_NAMES.Length; i++)
			{
				if (!base.HasValue(QuestionAction.OPT_NAMES[i]))
				{
					num = i;
					break;
				}
			}
			string[] array = new string[num];
			for (int j = 0; j < array.Length; j++)
			{
				array[j] = base.GetValue<RufiniString>(QuestionAction.OPT_NAMES[j]).Value;
			}
			this.context.TextBox.OnTextboxComplete += this.ContinueAfterTextbox;
			this.context.TextBox.Enqueue(new PrintAction(PrintActionType.PromptQuestion, array));
			this.context.TextBox.Show();
			return new ActionReturnContext
			{
				Wait = ScriptExecutor.WaitType.Event
			};
		}

		private void ContinueAfterTextbox()
		{
			this.context.TextBox.OnTextboxComplete -= this.ContinueAfterTextbox;
			this.context.Executor.Continue();
		}

		private static readonly string[] OPT_NAMES = new string[]
		{
			"opt1",
			"opt2",
			"opt3",
			"opt4",
			"opt5"
		};

		private ExecutionContext context;

		private NPC activeNpc;
	}
}
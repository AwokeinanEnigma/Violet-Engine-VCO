using System;
using System.Collections.Generic;
using VCO.Actors.NPCs;
using VCO.GUI.Text;
using VCO.GUI.Text.PrintActions;
using VCO.Scripts;
using VCO.Scripts.Actions;
using Rufini.Strings;

namespace Rufini.Actions.Types
{
	internal class TextboxAction : RufiniAction
	{
		public override string Code => "TXBX";
		public TextboxAction()
		{
			this.paramList = new List<ActionParam>
			{
				new ActionParam
				{
					Name = "text",
					Type = typeof(string)
				}
			};
		}

		public override ActionReturnContext Execute(ExecutionContext context)
		{
			this.context = context;
			string value = base.GetValue<string>("text");
			string value2 = StringFile.Instance.Get(value).Value;
			this.context.TextBox.OnTextboxComplete += this.ContinueAfterTextbox;
			TextProcessor textProcessor = new TextProcessor(value2);
			if (this.context.TextBox.HasPrinted)
			{
				this.context.TextBox.Enqueue(new PrintAction(PrintActionType.LineBreak, new object[0]));
			}
			this.context.TextBox.EnqueueAll(textProcessor.Actions);
			this.context.TextBox.Enqueue(new PrintAction(PrintActionType.Prompt, new object[0]));
			this.context.TextBox.Show();
			if (this.context.ActiveNPC != null)
			{
				this.activeNpc = this.context.ActiveNPC;
				this.activeNpc.StartTalking();
			}
			return new ActionReturnContext
			{
				Wait = ScriptExecutor.WaitType.Event
			};
		}

		private void StopTalking()
		{
			if (this.context.ActiveNPC != null)
			{
				this.activeNpc.StopTalking();
				this.activeNpc = null;
			}
		}

		private void ContinueAfterTextbox()
		{
			this.StopTalking();
			this.context.TextBox.OnTextboxComplete -= this.ContinueAfterTextbox;
			this.context.Executor.Continue();
		}

		private ExecutionContext context;

		private NPC activeNpc;
	}
}

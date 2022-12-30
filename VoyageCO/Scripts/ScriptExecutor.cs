using Rufini.Actions.Types;
using System;
using System.Collections.Generic;
using VCO.Actors.NPCs;
using VCO.Scripts.Actions;
using Violet;
using Violet.Input;

namespace VCO.Scripts
{
	internal class ScriptExecutor
	{
		public bool Running
		{
			get => running;

		}

		public int ProgramCounter
		{
			get => programCounter;
			
		}

		public ScriptExecutor(ExecutionContext context)
		{
			this.contextStack = new Stack<ScriptExecutor.ScriptContext>();
			this.context = context;
			this.context.Executor = this;
			this.waitMode = ScriptExecutor.WaitType.None;
			this.pausedInstruction = 0;
		}

		public void PushScript(RufiniScript script)
		{
			if (this.waitMode != ScriptExecutor.WaitType.None)
			{
				throw new InvalidOperationException("Cannot push a new script while waiting for an action to complete.");
			}
			if (this.contextStack.Count < 32)
			{
				if (this.script != null)
				{
					this.contextStack.Push(new ScriptExecutor.ScriptContext(this.context, this.script.Value, this.programCounter));
					this.Reset();
					this.context = new ExecutionContext(this.context);
				}
				this.script = new RufiniScript?(script);
				this.actions = this.script.Value.Actions;
				this.programCounter = 0;
				this.pushedScript = true;
				return;
			}
			throw new StackOverflowException("Script Executor stack cannot exceed 32 levels.");
		}

		public void SetCheckedNPC(NPC npc)
		{
			this.context.CheckedNPC = npc;
		}

		private void Reset()
		{
			this.pausedInstruction = 0;
			this.running = false;
			this.script = null;
			if (this.context.ActiveNPC != null)
			{
				this.context.ActiveNPC.StopTalking();
			}
			this.context.ActiveNPC = null;
			this.context.CheckedNPC = null;
		}

		public void Continue()
		{
			this.waitMode = ScriptExecutor.WaitType.None;
			Debug.Log($"EX: {this.programCounter} Continued");
		}

		public void JumpToElseOrEndIf()
		{
			if (this.script != null)
			{
				RufiniScript script = this.script.Value;

				int actionsAmount = script.Actions.Length;
				int controlIndex = 0;
				
				for (int i = this.programCounter; i < actionsAmount; i++)
				{
					RufiniAction rufiniAction = script.Actions[i];
					Debug.Log($"index {rufiniAction} and int is {i} control is {controlIndex} ");
					if (rufiniAction is IfFlagAction || rufiniAction is IfValueAction || rufiniAction is IfReturnAction)
					{
						Debug.Log($"found if action at {i}, control is {controlIndex}");
						controlIndex++;
					}
					else if (rufiniAction is EndIfAction)
					{
						controlIndex--;
						Debug.Log($"found end if action at {i}, control is {controlIndex}");
						if (controlIndex == 0)
						{
							Debug.Log($"control index is zero, resetting in end if block");
							this.programCounter = i;
							return;
						}
					}
					else if (rufiniAction is ElseAction)
					{
						Debug.Log($"found else action at {i}, control is {controlIndex}");
						if (i == this.programCounter)
						{
							controlIndex++;
						}
						else if (controlIndex - 1 == 0)
						{
							this.programCounter = i;
							return;
						}
					}

				}
			}
		}

		private bool PopScript()
		{
			bool result = true;
			this.Reset();
			if (this.contextStack.Count > 0)
			{
				ScriptExecutor.ScriptContext scriptContext = this.contextStack.Pop();
				this.context = scriptContext.ExecutionContext;
				this.script = new RufiniScript?(scriptContext.Script);
				this.actions = this.script.Value.Actions;
				this.pausedInstruction = scriptContext.ProgramCounter + 1;
			}
			else
			{
				Debug.Log("EX: End of execution");
				result = false;
				if (this.context.Player != null)
				{
					this.context.Player.InputLocked = false;
				}
				InputManager.Instance.Enabled = true;
				this.context.TextBox.Reset();
				if (this.context.TextBox.Visible)
				{
					this.context.TextBox.Hide();
				}
			}
			return result;
		}

		public void Execute()
		{
			if (this.script != null)
			{
				this.running = true;
				if (this.waitMode != ScriptExecutor.WaitType.Event)
				{
					bool flag = true;
					while (flag)
					{
						flag = false;
						this.programCounter = 0;
						this.programCounter = this.pausedInstruction;
						while (this.programCounter < this.actions.Length)
						{
							if (this.pushedScript)
							{
								this.pushedScript = false;
								this.programCounter = 0;
							}
							RufiniAction rufiniAction = this.actions[this.programCounter];
							Debug.Log($"EX: {this.programCounter} Execute {rufiniAction.GetType().Name}");
							this.waitMode = rufiniAction.Execute(this.context).Wait;
							if (this.waitMode != ScriptExecutor.WaitType.None)
							{
								this.pausedInstruction = this.programCounter + 1;
							//	Debug.Log($"EX: {this.programCounter} Paused (Next: {this.pausedInstruction})");
								break;
							}
							this.programCounter++;
						}
						if (this.waitMode == ScriptExecutor.WaitType.None && this.programCounter >= this.actions.Length)
						{
						//	Debug.Log("EX: End of script; popping");
							flag = this.PopScript();
						}
					}
				}
			}
		}

		private const int STACK_MAX_SIZE = 32;

		private Stack<ScriptExecutor.ScriptContext> contextStack;

		private ExecutionContext context;

		private RufiniScript? script;

		private RufiniAction[] actions;

		private bool running;

		private ScriptExecutor.WaitType waitMode;

		private int pausedInstruction;

		private int programCounter;

		private bool pushedScript;

		public enum WaitType
		{
			None,
			Frame,
			Event
		}

		private struct ScriptContext
		{
			public ExecutionContext ExecutionContext
			{
				get
				{
					return this.context;
				}
			}

			public RufiniScript Script
			{
				get
				{
					return this.script;
				}
			}

			public int ProgramCounter
			{
				get
				{
					return this.programCounter;
				}
			}

			public ScriptContext(ExecutionContext context, RufiniScript script, int programCounter)
			{
				this.context = context;
				this.script = script;
				this.programCounter = programCounter;
			}

			private ExecutionContext context;

			private RufiniScript script;

			private int programCounter;
		}
	}
}

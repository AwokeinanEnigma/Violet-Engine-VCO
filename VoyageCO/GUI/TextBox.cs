using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using VCO.Data;
using VCO.GUI.Text;
using VCO.GUI.Text.PrintActions;
using Violet.Graphics;
using Violet.GUI;
using Violet.Utility;

namespace VCO.GUI
{
    internal abstract class TextBox : Renderable
	{
		public bool HasPrinted
		{
			get
			{
				return this.hasPrinted;
			}
		}

		public event TextBox.CompletionHandler OnTextboxComplete;

		public event TextBox.TextTriggerHandler OnTextTrigger;

		protected TextBox(Vector2f relativePosition, Vector2f size, bool showBullets)
		{
			this.relativePosition = relativePosition;
			this.size = size;
			this.depth = 2147450880;
			this.actionQueue = new Queue<PrintAction>();
			this.relativeTypewriterPosition = this.relativePosition + TextBox.TYPEWRITER_POSITION_OFFSET;
			this.typewriter = new Typewriter(Fonts.Main, VectorMath.ZERO_VECTOR, VectorMath.ZERO_VECTOR, this.size + TextBox.TYPEWRITER_SIZE_OFFSET, showBullets);
			this.window = new WindowBox(Settings.WindowStyle, Settings.WindowFlavor, VectorMath.ZERO_VECTOR, this.size, 0);
			this.window.Visible = false;
			this.relativeArrowPosition = this.relativePosition + this.size + TextBox.ARROW_POSITION_OFFSET;
			this.advanceArrow = new IndexedColorGraphic(DataHandler.instance.Load("cursor.dat"), "down", VectorMath.ZERO_VECTOR, 0);
			this.advanceArrow.Visible = false;
			this.visible = false;
			this.hideAdvanceArrow = false;
			this.typewriter.OnTypewriterComplete += this.typewriter_OnTypewriterComplete;
			ViewManager.Instance.OnMove += this.OnViewMove;
		}

		private void typewriter_OnTypewriterComplete(object sender, EventArgs e)
		{
			if (!this.isPaused && !this.isWaitingOnPlayer)
			{
				this.Dequeue();
			}
		}

		private void TimerManager_OnTimerEnd(int timerIndex)
		{
			if (this.pauseTimerIndex == timerIndex && this.isPaused)
			{
				this.isPaused = false;
				this.Dequeue();
			}
		}

		private void OnViewMove(ViewManager sender, Vector2f center)
		{
			if (this.visible)
			{
				this.Recenter();
			}
		}

		protected void ContinueFromWait()
		{
			if (this.isWaitingOnPlayer)
			{
				this.isWaitingOnPlayer = false;
				this.advanceArrow.Visible = false;
				this.Dequeue();
			}
		}

		public void Enqueue(PrintAction action)
		{
			if (action.Type != PrintActionType.None)
			{
				this.actionQueue.Enqueue(action);
			}
		}

		public void EnqueueAll(IEnumerable<PrintAction> actions)
		{
			foreach (PrintAction action in actions)
			{
				this.Enqueue(action);
			}
		}

		protected virtual void HandlePrintText(string text)
		{
			this.typewriter.PrintText(text);
		}

		protected virtual void HandlePrintGraphic(string subsprite)
		{
			this.typewriter.PrintGraphic(subsprite);
		}

		protected virtual void HandlePromptQuestion(object[] options)
		{
			string[] options2 = Array.ConvertAll<object, string>(options, (object x) => (string)x);
			this.typewriter.PrintQuestion(options2);
		}

		protected virtual void HandlePromptList(object[] options)
		{
			Array.ConvertAll<object, string>(options, (object x) => (string)x);
		}

		protected virtual void HandlePromptNumeric(int minValue, int maxValue)
		{
		}

		protected virtual void HandlePrompt()
		{
			this.isWaitingOnPlayer = true;
			this.advanceArrow.Visible = !this.hideAdvanceArrow;
		}

		protected virtual void HandlePause(int duration)
		{
			this.isPaused = true;
			this.pauseTimerIndex = FrameTimerManager.Instance.StartTimer(duration);
			FrameTimerManager.Instance.OnTimerEnd += this.TimerManager_OnTimerEnd;
		}

		protected virtual void HandleLineBreak()
		{
			this.typewriter.PrintNewLine();
		}

		protected virtual void HandleTrigger(object[] args)
		{
			if (this.OnTextTrigger != null)
			{
				int type = int.Parse((string)args[0]);
				string[] args2 = new string[args.Length - 1];
				this.OnTextTrigger(type, args2);
			}
			this.Dequeue();
		}

		protected virtual void HandleColor(Color color)
		{
			this.typewriter.SetTextColor(color, true);
		}

		protected virtual void HandleSound(int type)
		{
			int num = Math.Max(0, Math.Min(6, type));
			Typewriter.BlipSound soundType = Typewriter.BlipSound.None;
			switch (num)
			{
				case 0:
					soundType = Typewriter.BlipSound.None;
					break;
				case 1:
					soundType = Typewriter.BlipSound.Narration;
					break;
				case 2:
					soundType = Typewriter.BlipSound.Male;
					break;
				case 3:
					soundType = Typewriter.BlipSound.Female;
					break;
				case 4:
					soundType = Typewriter.BlipSound.Awkward;
					break;
				case 5:
					soundType = Typewriter.BlipSound.Robot;
					break;
			}
			this.typewriter.SetTextSound(soundType, true);
		}

		private void HandleAction(PrintAction action)
		{
			switch (action.Type)
			{
				case PrintActionType.PrintText:
					this.HandlePrintText((string)action.Data);
					return;
				case PrintActionType.PrintGraphic:
					this.HandlePrintGraphic((string)action.Data);
					return;
				case PrintActionType.PromptQuestion:
					this.HandlePromptQuestion((object[])action.Data);
					return;
				case PrintActionType.PromptList:
					this.HandlePromptList((object[])action.Data);
					return;
				case PrintActionType.PromptNumeric:
					{
						int[] array = (int[])action.Data;
						this.HandlePromptNumeric(array[0], array[1]);
						return;
					}
				case PrintActionType.Prompt:
					this.HandlePrompt();
					return;
				case PrintActionType.Pause:
					this.HandlePause((int)action.Data);
					return;
				case PrintActionType.LineBreak:
					this.HandleLineBreak();
					return;
				case PrintActionType.Trigger:
					this.HandleTrigger((object[])action.Data);
					return;
				case PrintActionType.Color:
					break;
				case PrintActionType.Sound:
					this.HandleSound((int)action.Data);
					break;
				default:
					return;
			}
		}

		protected void Dequeue()
		{
			if (this.actionQueue.Count > 0)
			{
				PrintAction action = this.actionQueue.Dequeue();
				try
				{
					this.HandleAction(action);
					this.hasPrinted = true;
					return;
				}
				catch (InvalidCastException ex)
				{
					Console.WriteLine("Ate an InvalidCastException in the PrintReceiver:");
					Console.WriteLine(ex.Message);
					return;
				}
			}
			if (!this.isComplete && !this.isWaitingOnPlayer)
			{
				this.isComplete = true;
				if (this.OnTextboxComplete != null)
				{
					this.OnTextboxComplete();
				}
			}
		}

		public virtual void Show()
		{
			if (!this.visible)
			{
				this.Recenter();
				this.visible = true;
				this.typewriter.Visible = true;
				this.window.Visible = true;
				this.advanceArrow.Visible = false;
				this.Dequeue();
			}
		}

		public virtual void Hide()
		{
			if (this.visible)
			{
				this.visible = false;
			}
		}

		public virtual void Clear()
		{
			this.typewriter.Clear();
			this.hasPrinted = false;
		}

		public virtual void Reset()
		{
			this.Clear();
			this.isWaitingOnPlayer = false;
			this.hasPrinted = false;
			this.isComplete = false;
			this.typewriter.SetTextColor(Color.Black, false);
			this.typewriter.SetTextSound(Typewriter.BlipSound.Narration, false);
		}

		protected virtual void Recenter()
		{
			Vector2f finalCenter = ViewManager.Instance.FinalCenter;
			Vector2f v = finalCenter - ViewManager.Instance.View.Size / 2f;
			this.position = v + this.relativePosition;
			this.window.Position = this.position;
			this.advanceArrow.Position = v + this.relativeArrowPosition;
			this.typewriter.Position = v + this.relativeTypewriterPosition;
		}

		public virtual void Update()
		{
			if (this.visible)
			{
				if (this.isComplete && this.actionQueue.Count > 0)
				{
					this.Dequeue();
					this.isComplete = false;
				}
				this.typewriter.Update();
			}
		}

		public override void Draw(RenderTarget target)
		{
			if (this.window.Visible)
			{
				this.window.Draw(target);
			}
			if (this.typewriter.Visible)
			{
				this.typewriter.Draw(target);
			}
			if (this.advanceArrow.Visible)
			{
				this.advanceArrow.Draw(target);
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (!this.disposed)
			{
				if (disposing)
				{
					this.advanceArrow.Dispose();
					this.window.Dispose();
					this.typewriter.Dispose();
				}
				this.typewriter.OnTypewriterComplete -= this.typewriter_OnTypewriterComplete;
				ViewManager.Instance.OnMove -= this.OnViewMove;
			}
			base.Dispose(disposing);
		}

		public const int DEPTH = 2147450880;

		private static readonly Vector2f TYPEWRITER_POSITION_OFFSET = new Vector2f(10f, 8f);

		private static readonly Vector2f TYPEWRITER_SIZE_OFFSET = new Vector2f(-31f, -14f);

		private static readonly Vector2f ARROW_POSITION_OFFSET = new Vector2f(-14f, -6f);

		private Vector2f relativePosition;

		private Vector2f relativeArrowPosition;

		private Vector2f relativeTypewriterPosition;

		protected Graphic advanceArrow;

		protected WindowBox window;

		protected Typewriter typewriter;

		private Queue<PrintAction> actionQueue;

		private bool isPaused;

		private int pauseTimerIndex;

		protected bool isWaitingOnPlayer;

		protected bool hideAdvanceArrow;

		private bool hasPrinted;

		private bool isComplete;

		public delegate void CompletionHandler();

		public delegate void TextTriggerHandler(int type, string[] args);
	}
}

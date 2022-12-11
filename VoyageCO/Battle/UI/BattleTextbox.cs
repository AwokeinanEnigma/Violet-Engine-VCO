using SFML.System;
using VCO.GUI;
using VCO.GUI.Text;
using Violet.Input;
using Violet.Utility;
using static Violet.GUI.WindowBox;

namespace VCO.Battle.UI
{
    internal class BattleTextBox : TextBox
	{
		public bool AutoScroll
		{
			get
			{
				return this.autoScroll;
			}
			set
			{
				this.autoScroll = value;
				this.hideAdvanceArrow = this.autoScroll;
			}
		}

		public BattleTextBox() : base(BattleTextBox.BOX_POSITION, BattleTextBox.BOX_SIZE, false)
		{
			this.autoScroll = true;
			this.typewriter.SetTextSound(Typewriter.BlipSound.None, false);
			InputManager.Instance.ButtonPressed += this.ButtonPressed;
		}

		private void ButtonPressed(InputManager sender, Button b)
		{
			if (!this.autoScroll && this.isWaitingOnPlayer && b == Button.A)
			{
				base.ContinueFromWait();
			}
		}

		public void ChangeStyle(WindowStyle style) {
			window.Style = style;
		}

		private void OnWaitTimerEnd(int timerIndex)
		{
			if (timerIndex == this.waitTimerIndex)
			{
				this.isWaitingOnPlayer = false;
				this.advanceArrow.Visible = false;
				FrameTimerManager.Instance.OnTimerEnd -= this.OnWaitTimerEnd;
				base.Dequeue();
			}
		}

		protected override void HandlePrompt()
		{
			this.isWaitingOnPlayer = true;
			this.advanceArrow.Visible = !this.hideAdvanceArrow;
			if (this.autoScroll)
			{
				this.waitTimerIndex = FrameTimerManager.Instance.StartTimer(45);
				FrameTimerManager.Instance.OnTimerEnd += this.OnWaitTimerEnd;
				this.advanceArrow.Visible = false;
			}
		}

		public override void Hide()
		{
			base.Hide();
			this.Clear();
		}

		public override void Reset()
		{
			base.Reset();
			this.autoScroll = true;
			this.typewriter.SetTextSound(Typewriter.BlipSound.None, false);
		}

		protected override void Dispose(bool disposing)
		{
			if (!this.disposed)
			{
				InputManager.Instance.ButtonPressed -= this.ButtonPressed;
			}
			base.Dispose(disposing);
		}

		private const Button ADVANCE_BUTTON = Button.A;

		protected const int MESSAGE_WAIT = 45;

		private static readonly Vector2f BOX_SIZE = new Vector2f(248f, 43f);

		private static readonly Vector2f BOX_POSITION = new Vector2f((float)(160L - (long)((int)(BattleTextBox.BOX_SIZE.X / 2f))), 0f);

		private int waitTimerIndex;

		private bool autoScroll;
	}
}

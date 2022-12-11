using System;
using Violet;
using Violet.Flags;
using Violet.Graphics;
using Violet.GUI;
using Violet.Input;
using Violet.Utility;
using VCO.Data;
using VCO.GUI.Text;
using SFML.Graphics;
using SFML.System;

namespace VCO.GUI
{
	internal class OverworldTextBox : TextBox
	{
		public string Nametag
		{
			get
			{
				return this.nametag.Name;
			}
			set
			{
				this.SetNametag(value);
			}
		}

		public OverworldTextBox() : base(OverworldTextBox.BOX_POSITION, OverworldTextBox.BOX_SIZE, true)
		{
			this.size = OverworldTextBox.BOX_SIZE;
			this.canTransitionIn = true;
			this.canTransitionOut = true;
			this.state = OverworldTextBox.AnimationState.Hidden;
			Vector2f finalCenter = ViewManager.Instance.FinalCenter;
			finalCenter -= ViewManager.Instance.View.Size / 2f;
			this.dimmer = new RectangleShape(Engine.SCREEN_SIZE);
			this.dimmer.Origin = Engine.HALF_SCREEN_SIZE;
			this.dimmer.Position = finalCenter;
			this.nametag = new Nametag(string.Empty, VectorMath.ZERO_VECTOR, 0);
			this.nametag.Visible = false;
			this.nametagVisible = false;
			InputManager.Instance.ButtonPressed += this.ButtonPressed;
		}

		private void ButtonPressed(InputManager sender, Button b)
		{
			if (this.isWaitingOnPlayer && b == Button.A)
			{
				base.ContinueFromWait();
			}
		}

		protected override void Recenter()
		{
			Vector2f finalCenter = ViewManager.Instance.FinalCenter;
			Vector2f vector2f = finalCenter - ViewManager.Instance.View.Size / 2f;
			this.position = new Vector2f(vector2f.X + OverworldTextBox.BOX_POSITION.X, vector2f.Y + OverworldTextBox.BOX_POSITION.Y);
			this.window.Position = new Vector2f(this.position.X, this.position.Y + this.textboxY);
			this.advanceArrow.Position = new Vector2f(vector2f.X + OverworldTextBox.BUTTON_POSITION.X, vector2f.Y + OverworldTextBox.BUTTON_POSITION.Y);
			this.nametag.Position = new Vector2f(vector2f.X + OverworldTextBox.NAMETAG_POSITION.X, vector2f.Y + OverworldTextBox.NAMETAG_POSITION.Y + this.textboxY);
			this.typewriter.Position = new Vector2f(vector2f.X + OverworldTextBox.TEXT_POSITION.X, vector2f.Y + OverworldTextBox.TEXT_POSITION.Y + this.textboxY);
		}

		private void SetNametag(string namestring)
		{
			if (namestring != null && namestring.Length > 0)
			{
				this.nametag.Name = TextProcessor.ProcessReplacements(namestring);
				this.nametagVisible = true;
			}
			else
			{
				this.nametagVisible = false;
			}
			this.nametag.Visible = this.nametagVisible;
		}

		public override void Reset()
		{
			base.Reset();
			this.SetNametag(null);
		}

		private void UpdateTextboxAnimation(float amount)
		{
			this.textboxY = 4f * (1f - Math.Max(0f, Math.Min(1f, amount)));
			this.typewriter.Position = new Vector2f((float)((int)(ViewManager.Instance.Viewrect.Left + OverworldTextBox.TEXT_POSITION.X)), (float)((int)(ViewManager.Instance.Viewrect.Top + OverworldTextBox.TEXT_POSITION.Y + this.textboxY)));
			this.window.Position = new Vector2f((float)((int)(ViewManager.Instance.Viewrect.Left + OverworldTextBox.BOX_POSITION.X)), (float)((int)(ViewManager.Instance.Viewrect.Top + OverworldTextBox.BOX_POSITION.Y + this.textboxY)));
			this.nametag.Position = new Vector2f((float)((int)(ViewManager.Instance.Viewrect.Left + OverworldTextBox.NAMETAG_POSITION.X)), (float)((int)(ViewManager.Instance.Viewrect.Top + OverworldTextBox.NAMETAG_POSITION.Y + this.textboxY)));
		}

		public override void Show()
		{
			if (!this.visible)
			{
				this.window.Style = (FlagManager.Instance[4] ? Settings.WindowStyle : Settings.WindowStyle);
				this.visible = true;
				this.Recenter();
				this.window.Visible = true;
				this.typewriter.Visible = true;
				this.nametag.Visible = this.nametagVisible;
				this.state = OverworldTextBox.AnimationState.SlideIn;
				this.slideProgress = (this.canTransitionIn ? 0f : 1f);
				this.UpdateTextboxAnimation(0f);
			}
		}

		public override void Hide()
		{
			if (this.visible)
			{
				this.Recenter();
				this.advanceArrow.Visible = false;
				this.state = OverworldTextBox.AnimationState.SlideOut;
				this.slideProgress = (this.canTransitionOut ? 1f : 0f);
				this.UpdateTextboxAnimation(this.slideProgress * 2f);
			}
		}

		public void SetDimmer(float dim)
		{
			this.dimmer.FillColor = new Color(0, 0, 0, (byte)(255f * dim));
		}

		public override void Update()
		{
			switch (this.state)
			{
				case OverworldTextBox.AnimationState.SlideIn:
					if (this.slideProgress < 1f)
					{
						this.UpdateTextboxAnimation(this.slideProgress);
						this.slideProgress += 0.2f;
						return;
					}
					this.state = OverworldTextBox.AnimationState.Textbox;
					this.UpdateTextboxAnimation(1f);
					base.Dequeue();
					return;
				case OverworldTextBox.AnimationState.Textbox:
					base.Update();
					return;
				case OverworldTextBox.AnimationState.SlideOut:
					if (this.slideProgress > 0f)
					{
						this.UpdateTextboxAnimation(this.slideProgress);
						this.slideProgress -= 0.2f;
						return;
					}
					this.state = OverworldTextBox.AnimationState.Hidden;
					this.UpdateTextboxAnimation(0f);
					this.typewriter.Visible = false;
					this.nametag.Visible = false;
					this.window.Visible = false;
					this.visible = false;
					return;
				default:
					return;
			}
		}

		public override void Draw(RenderTarget target)
		{
			if (this.nametag.Visible)
			{
				this.nametag.Draw(target);
			}
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
					this.nametag.Dispose();
				}
				InputManager.Instance.ButtonPressed -= this.ButtonPressed;
			}
			base.Dispose(disposing);
		}

		private const Button ADVANCE_BUTTON = Button.A;

		private const float TEXTBOX_ANIM_SPEED = 0.2f;

		private const float TEXTBOX_Y_OFFSET = 4f;

		private static readonly Vector2f BOX_POSITION = new Vector2f(16f, 120f);

		private static readonly Vector2f BOX_SIZE = new Vector2f(231f, 56f);

		private static readonly Vector2f TEXT_POSITION = new Vector2f(OverworldTextBox.BOX_POSITION.X + 10f, OverworldTextBox.BOX_POSITION.Y + 8f);

		private static readonly Vector2f TEXT_SIZE = new Vector2f(OverworldTextBox.BOX_SIZE.X - 31f, OverworldTextBox.BOX_SIZE.Y - 14f);

		private static readonly Vector2f NAMETAG_POSITION = new Vector2f(OverworldTextBox.BOX_POSITION.X + 3f, OverworldTextBox.BOX_POSITION.Y - 14f);

		private static readonly Vector2f NAMETEXT_POSITION = new Vector2f(OverworldTextBox.NAMETAG_POSITION.X + 5f, OverworldTextBox.NAMETAG_POSITION.Y + 1f);

		private static readonly Vector2f BUTTON_POSITION = new Vector2f(OverworldTextBox.BOX_POSITION.X + OverworldTextBox.BOX_SIZE.X - 14f, OverworldTextBox.BOX_POSITION.Y + OverworldTextBox.BOX_SIZE.Y - 6f);

		protected Nametag nametag;

		protected bool nametagVisible;

		private OverworldTextBox.AnimationState state;

		private float textboxY;

		private bool canTransitionIn;

		private bool canTransitionOut;

		private float slideProgress;

		private Shape dimmer;

		private enum AnimationState
		{
			SlideIn,
			Textbox,
			SlideOut,
			Hidden
		}
	}
}

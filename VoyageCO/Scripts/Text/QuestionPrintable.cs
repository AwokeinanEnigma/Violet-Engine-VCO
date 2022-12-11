using System;
using SFML.Graphics;
using SFML.System;
using VCO.Data;
using Violet.Audio;
using Violet.Flags;
using Violet.Graphics;
using Violet.GUI;
using Violet.Input;
using Violet.Utility;

namespace VCO.GUI.Text.Printables
{
    internal class QuestionPrintable : Printable
	{
		public override Vector2f Position
		{
			get
			{
				return this.position;
			}
			set
			{
				this.SetPosition(value);
			}
		}

		public QuestionPrintable(FontData font, float width, string[] options)
		{
			this.size = new Vector2f(width, (float)font.LineHeight);
			this.isRemovable = true;
			this.texts = new TextRegion[options.Length];
			for (int i = 0; i < this.texts.Length; i++)
			{
				this.texts[i] = new TextRegion(VectorMath.ZERO_VECTOR, 0, font, options[i]);
			}
			this.selectedIndex = 0;
			this.cursor = new IndexedColorGraphic(DataHandler.instance.Load("cursor.dat"), "right", VectorMath.ZERO_VECTOR, 0);
			this.selectRect = new RectangleShape();
			this.selectRect.Origin = new Vector2f(1f, 0f);
			this.selectRect.FillColor = UIColors.HighlightColor;
			this.selectRectStates = new RenderStates(BlendMode.Alpha, Transform.Identity, null, null);
			this.cursorOffsetY = (float)(font.LineHeight / 2);
			this.cellSize = (int)(width - this.cursor.Size.X * 2f) / Math.Max(1, this.texts.Length);
			this.moveSound = AudioManager.Instance.Use(DataHandler.instance.Load("cursorx.wav"), AudioType.Sound);
			this.selectSound = AudioManager.Instance.Use(DataHandler.instance.Load("cursory.wav"), AudioType.Sound);
			InputManager.Instance.ButtonPressed += this.ButtonPressed;
			InputManager.Instance.AxisPressed += this.AxisPressed;
		}

		private void AxisPressed(InputManager sender, Vector2f axis)
		{
			if (axis.X < 0f)
			{
				this.selectLeft = true;
				return;
			}
			if (axis.X > 0f)
			{
				this.selectRight = true;
			}
		}

		private void ButtonPressed(InputManager sender, Button b)
		{
			if (b == Button.A)
			{
				this.select = true;
			}
		}

		private void UpdateOptionPositions()
		{
			for (int i = 0; i < this.texts.Length; i++)
			{
				this.texts[i].Position = this.position + new Vector2f((float)((int)this.cursor.Size.X + this.cellSize * i), 0f);
			}
		}

		private void UpdateSelection()
		{
			this.cursor.Position = this.texts[this.selectedIndex].Position + new Vector2f(0f, this.cursorOffsetY);
			for (int i = 0; i < this.texts.Length; i++)
			{
				if (this.selectedIndex == i)
				{
					this.texts[i].Color = Color.Black;
					this.selectRect.Position = this.texts[i].Position;
					this.selectRect.Size = new Vector2f(this.texts[i].Size.X + 2f, (float)this.texts[i].FontData.LineHeight);
				}
				else
				{
					this.texts[i].Color = Color.White;
				}
			}
		}

		private void SetPosition(Vector2f newPosition)
		{
			this.position = newPosition;
			this.UpdateOptionPositions();
			this.UpdateSelection();
		}

		public override void Update()
		{
			if (this.selectLeft || this.selectRight)
			{
				this.moveSound.Play();
			}
			if (this.selectLeft)
			{
				this.selectedIndex = (this.selectedIndex + this.texts.Length - 1) % this.texts.Length;
				this.UpdateSelection();
				this.selectLeft = false;
			}
			if (this.selectRight)
			{
				this.selectedIndex = (this.selectedIndex + 1) % this.texts.Length;
				this.UpdateSelection();
				this.selectRight = false;
			}
			if (this.select && !this.hasSelected)
			{
				this.selectSound.Play();
				this.selectSound.OnComplete += this.selectSound_OnComplete;
				FlagManager.Instance[2] = (this.selectedIndex == 0);
				ValueManager.Instance[0] = this.selectedIndex;
				this.cursor.Visible = false;
				this.UnregisterInputDelegates();
				this.hasSelected = true;
			}
		}

		private void selectSound_OnComplete(VioletSound sender)
		{
			this.isDone = true;
			sender.OnComplete -= this.selectSound_OnComplete;
		}

		public override void Draw(RenderTarget target)
		{
			this.selectRect.Draw(target, this.selectRectStates);
			for (int i = 0; i < this.texts.Length; i++)
			{
				if (this.texts[i].Visible)
				{
					this.texts[i].Draw(target);
				}
			}
			if (this.cursor.Visible)
			{
				this.cursor.Draw(target);
			}
		}

		private void UnregisterInputDelegates()
		{
			InputManager.Instance.ButtonPressed -= this.ButtonPressed;
			InputManager.Instance.AxisPressed -= this.AxisPressed;
		}

		protected override void Dispose(bool disposing)
		{
			if (!this.disposed)
			{
				if (disposing)
				{
					this.selectRect.Dispose();
					this.cursor.Dispose();
					for (int i = 0; i < this.texts.Length; i++)
					{
						this.texts[i].Dispose();
					}
				}
				AudioManager.Instance.Unuse(this.moveSound);
				AudioManager.Instance.Unuse(this.selectSound);
				this.UnregisterInputDelegates();
			}
			base.Dispose(disposing);
		}

		private const string SUBSPRITE_NAME = "right";

		private IndexedColorGraphic cursor;

		private RectangleShape selectRect;

		private RenderStates selectRectStates;

		private float cursorOffsetY;

		private TextRegion[] texts;

		private int cellSize;

		private bool select;

		private bool selectLeft;

		private bool selectRight;

		private bool hasSelected;

		private int selectedIndex;

		private VioletSound moveSound;

		private VioletSound selectSound;
	}
}

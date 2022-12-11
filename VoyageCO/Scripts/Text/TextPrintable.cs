using System;
using Violet.Audio;
using Violet.GUI;
using Violet.Utility;
using VCO.Data;
using SFML.Graphics;
using SFML.System;

namespace VCO.GUI.Text.Printables
{
	internal class TextPrintable : Printable
	{
		public override Vector2f Position
		{
			get
			{
				return this.textRegion.Position;
			}
			set
			{
				this.textRegion.Position = value;
			}
		}

		public override Vector2f Origin
		{
			get
			{
				return this.textRegion.Origin;
			}
			set
			{
				this.textRegion.Origin = value;
			}
		}

		public override Vector2f Size
		{
			get
			{
				return this.textRegion.Size + new Vector2f(1f, 0f);
			}
		}

		public TextPrintable(FontData font, VioletSound sound, Color color, string text)
		{
			this.textRegion = new TextRegion(VectorMath.ZERO_VECTOR, 0, font, text);
			this.textRegion.Color = color;
			this.textLength = text.Length;
			this.textRegion.Length = 0;
			this.counter = 0f;
			this.speed = 10f / Math.Max(1f, (float)Settings.TextSpeed);
			this.blepCounter = 0;
			this.sound = sound;
		}

		public void TrimStart()
		{
			string text = this.textRegion.Text.TrimStart(new char[0]);
			this.textLength = text.Length;
			this.textRegion.Text = text;
		}

		public string SplitOffText(int pixelLength)
		{
			int num = 0;
			int i = 0;
			while (i < this.textRegion.Text.Length)
			{
				char c = this.textRegion.Text[i];
				if (char.IsWhiteSpace(c))
				{
					num = i;
				}
				Glyph glyph = this.textRegion.FontData.Font.GetGlyph((uint)c, this.textRegion.FontData.Size, false, 0);
				float num2 = this.textRegion.FindCharacterPosition((uint)i).X + glyph.Bounds.Width;
				if (num2 > (float)pixelLength)
				{
					if (num == 0)
					{
						num = i;
						break;
					}
					break;
				}
				else
				{
					i++;
				}
			}
			string text = this.textRegion.Text.Substring(0, num);
			string result = this.textRegion.Text.Substring(num + ((num > 0) ? 1 : 0));
			this.textRegion.Reset(text, 0, 0);
			this.textLength = text.Length;
			return result;
		}

		public override void Update()
		{
			if (this.textRegion.Length + 1 <= this.textLength)
			{
				this.counter += this.speed;
				if ((double)this.textRegion.Length < Math.Floor((double)this.counter))
				{
					this.textRegion.Length += (int)this.counter - this.textRegion.Length;
				}
				this.blepCounter++;
				if (this.blepCounter % 3 == 0 && this.sound != null)
				{
					this.sound.Play();
					return;
				}
			}
			else
			{
				this.textRegion.Length = this.textLength;
				this.isDone = true;
			}
		}

		public override void Draw(RenderTarget target)
		{
			this.textRegion.Draw(target);
		}

		protected override void Dispose(bool disposing)
		{
			if (!this.disposed && disposing)
			{
				this.textRegion.Dispose();
			}
			base.Dispose(disposing);
		}

		private const float MAX_CHARS_PER_FRAME = 10f;

		private const int FRAMES_PER_BLEP = 3;

		private TextRegion textRegion;

		private int textLength;

		private float speed;

		private float counter;

		private int blepCounter;

		private VioletSound sound;
	}
}

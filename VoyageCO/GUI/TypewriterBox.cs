using System;
using System.Collections.Generic;
using Violet.Audio;
using Violet.Graphics;
using Violet.GUI;
using Violet.Utility;
using VCO.Data;
using VCO.GUI.Text.Printables;
using SFML.Graphics;
using SFML.System;

namespace VCO.GUI.Text
{
	internal class Typewriter : Renderable
	{
		public override Vector2f Position
		{
			get
			{
				return this.position;
			}
			set
			{
				this.Reposition(value, this.origin);
			}
		}

		public override Vector2f Origin
		{
			get
			{
				return this.origin;
			}
			set
			{
				this.Reposition(this.position, value);
			}
		}

		public override Vector2f Size
		{
			get
			{
				return this.size;
			}
			set
			{
				this.size = value;
			}
		}

		public bool ShowBullets
		{
			get
			{
				return this.showBullets;
			}
		}

		public event Typewriter.TypewriterCompleteHandler OnTypewriterComplete;

		public Typewriter(FontData font, Vector2f position, Vector2f origin, Vector2f size, bool showBullets)
		{
			this.depth = 2147450880;
			this.fontData = font;
			this.position = position;
			this.origin = origin;
			this.size = size;
			this.textColor = Color.Black;
			this.currentLine = 0;
			int num = (int)(this.size.Y / (float)this.fontData.LineHeight);
			this.lines = new List<Printable>[num];
			for (int i = 0; i < this.lines.Length; i++)
			{
				this.lines[i] = new List<Printable>();
			}
			this.showBullets = showBullets;
			this.bullets = new Renderable[this.lines.Length];
			float num2 = 0f;
			for (int j = 0; j < this.bullets.Length; j++)
			{
				this.bullets[j] = new IndexedColorGraphic(DataHandler.instance.Load( "bullet.dat"), "bullet", this.position - this.origin + new Vector2f(0f, (float)(this.fontData.LineHeight * j + this.fontData.LineHeight / 2)), 0);
				this.bullets[j].Visible = false;
				num2 = Math.Max(num2, this.bullets[j].Size.X);
			}
			this.bulletWidth = (this.showBullets ? num2 : 0f);
			this.SetTextSound(Typewriter.BlipSound.Narration, false);
		}

		private void Reposition(Vector2f newPosition, Vector2f newOrigin)
		{
			Vector2f v = new Vector2f(newPosition.X - newOrigin.X - (this.position.X - this.origin.X), newPosition.Y - newOrigin.Y - (this.position.Y - this.origin.Y));
			for (int i = 0; i < this.lines.Length; i++)
			{
				for (int j = 0; j < this.lines[i].Count; j++)
				{
					this.lines[i][j].Position += v;
				}
				this.bullets[i].Position += v;
			}
			this.position = newPosition;
			this.origin = newOrigin;
		}

		private int CalculateLineWidth(int line)
		{
			int num = Math.Max(0, Math.Min(this.lines.Length - 1, line));
			int num2 = (int)this.bulletWidth;
			for (int i = 0; i < this.lines[num].Count; i++)
			{
				num2 += (int)this.lines[num][i].Size.X;
			}
			return num2;
		}

		private bool PrintableFitsInCurrentLine(Printable printable)
		{
			int num = this.CalculateLineWidth(this.currentLine);
			return num + (int)printable.Size.X <= (int)this.size.X;
		}

		public void Clear()
		{
			for (int i = 0; i < this.lines.Length; i++)
			{
				for (int j = 0; j < this.lines[i].Count; j++)
				{
					this.lines[i][j].Dispose();
				}
				this.lines[i].Clear();
				this.bullets[i].Visible = false;
			}
			this.currentLine = 0;
			this.currentPrintable = null;
			this.splitTextForLater = null;
		}

		private void ClearTopLine()
		{
			for (int i = 0; i < this.lines[0].Count; i++)
			{
				this.lines[0][i].Dispose();
			}
			this.lines[0].Clear();
		}

		private void ShiftLinesUp()
		{
			this.ClearTopLine();
			List<Printable> list = this.lines[0];
			for (int i = 1; i < this.lines.Length; i++)
			{
				this.bullets[i - 1].Visible = this.bullets[i].Visible;
				this.lines[i - 1] = this.lines[i];
				for (int j = 0; j < this.lines[i].Count; j++)
				{
					this.lines[i][j].Position -= new Vector2f(0f, (float)this.fontData.LineHeight);
				}
			}
			this.bullets[this.bullets.Length - 1].Visible = false;
			this.lines[this.lines.Length - 1] = list;
		}

		private void CompletePrintAction()
		{
			if (this.OnTypewriterComplete != null)
			{
				this.OnTypewriterComplete(this, new EventArgs());
			}
		}

		private void AdvanceLine()
		{
			if (this.currentLine + 1 >= this.lines.Length)
			{
				this.ShiftLinesUp();
				return;
			}
			this.currentLine++;
		}

		public void PrintNewLine()
		{
			this.AdvanceLine();
			this.CompletePrintAction();
		}

		public void SetTextColor(Color color, bool isPrintAction)
		{
			this.textColor = color;
			if (isPrintAction)
			{
				this.CompletePrintAction();
			}
		}

		public void SetTextSound(Typewriter.BlipSound soundType, bool isPrintAction)
		{
			if (soundType != this.textSoundType)
			{
				this.textSoundType = soundType;
				if (this.textSoundType != Typewriter.BlipSound.None)
				{
					this.textSound = AudioManager.Instance.Use(DataHandler.instance.Load(Typewriter.BLIP_SOUNDS[this.textSoundType]), AudioType.Sound);
				}
				else
				{
					if (this.textSound != null)
					{
						AudioManager.Instance.Unuse(this.textSound);
					}
					this.textSound = null;
				}
			}
			if (isPrintAction)
			{
				this.CompletePrintAction();
			}
		}

		private void Print(Printable printable)
		{
			List<Printable> list = this.lines[this.currentLine];
			float num = this.position.X - this.origin.X + this.bulletWidth;
			float num2 = 0f;
			if (list.Count > 0)
			{
				Renderable renderable = list[list.Count - 1];
				num = renderable.Position.X - renderable.Origin.X;
				num2 = renderable.Size.X;
			}
			printable.Position = new Vector2f((float)((int)(num + num2)), (float)((int)(this.position.Y - this.origin.Y + (float)(this.currentLine * this.fontData.LineHeight))));
			this.lines[this.currentLine].Add(printable);
			this.currentPrintable = printable;
		}

		public void PrintText(string text)
		{
			string text2 = text;
			if (text2.Length > 0 && text2[0] == '@')
			{
				this.bullets[this.currentLine].Visible = this.showBullets;
				text2 = text2.Substring(1);
			}
			TextPrintable textPrintable = new TextPrintable(this.fontData, this.textSound, this.textColor, text2);
			if (!this.PrintableFitsInCurrentLine(textPrintable))
			{
				int pixelLength = (int)this.size.X - this.CalculateLineWidth(this.currentLine);
				this.splitTextForLater = textPrintable.SplitOffText(pixelLength);
			}
			if (textPrintable.Size.X > 0f)
			{
				this.Print(textPrintable);
			}
		}

		public void PrintGraphic(string subsprite)
		{
			GraphicPrintable graphicPrintable = new GraphicPrintable(subsprite);
			graphicPrintable.Origin = VectorMath.ZERO_VECTOR;
			if (!this.PrintableFitsInCurrentLine(graphicPrintable))
			{
				this.PrintNewLine();
			}
			this.Print(graphicPrintable);
		}

		public void PrintQuestion(string[] options)
		{
			QuestionPrintable printable = new QuestionPrintable(this.fontData, this.size.X, options);
			if (this.lines[this.currentLine].Count > 0)
			{
				this.PrintNewLine();
			}
			this.Print(printable);
		}

		public void Update()
		{
			if (this.currentPrintable != null)
			{
				if (!this.currentPrintable.Complete)
				{
					this.currentPrintable.Update();
					return;
				}
				if (this.splitTextForLater != null)
				{
					this.AdvanceLine();
					string text = this.splitTextForLater;
					this.splitTextForLater = null;
					this.PrintText(text);
					this.currentPrintable.Update();
					return;
				}
				if (this.currentPrintable.Removable)
				{
					this.lines[this.currentLine].Remove(this.currentPrintable);
					this.currentPrintable.Dispose();
				}
				this.currentPrintable = null;
				this.CompletePrintAction();
			}
		}

		public override void Draw(RenderTarget target)
		{
			for (int i = 0; i < this.lines.Length; i++)
			{
				if (this.bullets[i].Visible)
				{
					this.bullets[i].Draw(target);
				}
				for (int j = 0; j < this.lines[i].Count; j++)
				{
					this.lines[i][j].Draw(target);
				}
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (!this.disposed)
			{
				if (disposing)
				{
					for (int i = 0; i < this.lines.Length; i++)
					{
						for (int j = 0; j < this.lines[i].Count; j++)
						{
							this.lines[i][j].Dispose();
						}
					}
				}
				AudioManager.Instance.Unuse(this.textSound);
				this.textSound = null;
				this.OnTypewriterComplete = null;
			}
			base.Dispose(disposing);
		}

		private const int DEPTH = 2147450880;

		private const char BULLET_CHAR = '@';

		private static readonly Dictionary<Typewriter.BlipSound, string> BLIP_SOUNDS = new Dictionary<Typewriter.BlipSound, string>
		{
			{
				Typewriter.BlipSound.Narration,
				"text1.wav"
			},
			{
				Typewriter.BlipSound.Male,
				"text2.wav"
			},
			{
				Typewriter.BlipSound.Female,
				"text3.wav"
			},
			{
				Typewriter.BlipSound.Awkward,
				"text4.wav"
			},
			{
				Typewriter.BlipSound.Robot,
				"text5.wav"
			}
		};

		private FontData fontData;

		private Color textColor;

		private List<Printable>[] lines;

		private int currentLine;

		private Printable currentPrintable;

		private string splitTextForLater;

		private Renderable[] bullets;

		private float bulletWidth;

		private bool showBullets;

		private Typewriter.BlipSound textSoundType;

		private VioletSound textSound;

		public enum BlipSound
		{
			None,
			Narration,
			Male,
			Female,
			Awkward,
			Robot
		}

		public delegate void TypewriterCompleteHandler(object sender, EventArgs e);
	}
}

﻿using SFML.Graphics;
using SFML.System;
using System;
using VCO.Scripts.Text;
using Violet;
using Violet.Actors;
using Violet.Graphics;
using Violet.GUI;
using Violet.Utility;

namespace VCO.GUI
{
    internal class FlyoverText : Actor
    {
        public event FlyoverText.OnCompletionHandler OnCompletion;

        public FlyoverText(RenderPipeline pipeline, FontData font, string text, FlyoverText.TextPosition textPosition, Color backColor, Color textColor, int transitionDuration, int holdDuration)
        {
            this.pipeline = pipeline;
            this.transitionDuration = transitionDuration;
            this.holdDuration = holdDuration;
            this.textPosition = textPosition;
            this.duration = this.transitionDuration * 2 + this.holdDuration;
            this.backColor = backColor;
            this.backColorTrans = new Color(backColor.R, backColor.G, backColor.B, 0);
            this.backgroundShape = new RectangleShape(Engine.SCREEN_SIZE)
            {
                FillColor = Color.Transparent
            };
            this.background = new ShapeGraphic(this.backgroundShape, ViewManager.Instance.View.Center, Engine.HALF_SCREEN_SIZE, Engine.SCREEN_SIZE, 2147467264);
            this.pipeline.Add(this.background);
            this.font = font;
            this.textColor = textColor;
            this.textColorTrans = new Color(textColor.R, textColor.G, textColor.B, 0);
            this.SetupTextRegions(text);
            this.PositionTextRegions(ViewManager.Instance.Center);
            this.UpdateTextColor(Color.Transparent);
            ViewManager.Instance.OnMove += this.ViewMoved;
        }

        private void SetupTextRegions(string text)
        {
            TextBlock textBlock = TextProcessor.Process(this.font, text, 288);
            this.texts = new TextRegion[textBlock.Lines.Count];
            for (int i = 0; i < this.texts.Length; i++)
            {
                this.texts[i] = new TextRegion(VectorMath.ZERO_VECTOR, 2147467265, this.font, textBlock.Lines[i].Text)
                {
                    Color = this.textColorTrans,
                    Origin = new Vector2f(font.XCompensation, font.YCompensation)
                };
                this.pipeline.Add(this.texts[i]);
            }
        }

        private void PositionTextRegions(Vector2f center)
        {
            int lineHeight = this.font.LineHeight;
            int num = 0;
            int num2 = 0;
            for (int i = 0; i < this.texts.Length; i++)
            {
                num2 += lineHeight;
                num = Math.Max(num, (int)this.texts[i].Size.X);
            }
            for (int j = 0; j < this.texts.Length; j++)
            {
                Vector2f zero_VECTOR;
                switch (this.textPosition)
                {
                    case FlyoverText.TextPosition.Center:
                        zero_VECTOR = new Vector2f(-(num / 2), (float)(-(float)(num2 / 2) + lineHeight * j));
                        break;
                    case FlyoverText.TextPosition.TopLeft:
                        zero_VECTOR = new Vector2f(-144f, -74L + lineHeight * j);
                        break;
                    case FlyoverText.TextPosition.Top:
                        zero_VECTOR = new Vector2f(-(num / 2), -74L + lineHeight * j);
                        break;
                    case FlyoverText.TextPosition.TopRight:
                        zero_VECTOR = new Vector2f(144L - num, -74L + lineHeight * j);
                        break;
                    case FlyoverText.TextPosition.Left:
                        zero_VECTOR = new Vector2f(-144f, (float)(-(float)(num2 / 2) + lineHeight * j));
                        break;
                    case FlyoverText.TextPosition.Right:
                        zero_VECTOR = new Vector2f(144L - num, (float)(-(float)(num2 / 2) + lineHeight * j));
                        break;
                    case FlyoverText.TextPosition.BottomLeft:
                        zero_VECTOR = new Vector2f(-144f, 74L - num2 + lineHeight * j);
                        break;
                    case FlyoverText.TextPosition.Bottom:
                        zero_VECTOR = new Vector2f(-(num / 2), 74L - num2 + lineHeight * j);
                        break;
                    case FlyoverText.TextPosition.BottomRight:
                        zero_VECTOR = new Vector2f(144L - num, 74L - num2 + lineHeight * j);
                        break;
                    default:
                        zero_VECTOR = VectorMath.ZERO_VECTOR;
                        break;
                }
                this.texts[j].Position = center + zero_VECTOR;
            }
        }

        private void UpdateTextColor(Color color)
        {
            for (int i = 0; i < this.texts.Length; i++)
            {
                this.texts[i].Color = color;
            }
        }

        private void ViewMoved(ViewManager sender, Vector2f newCenter)
        {
            this.background.Position = newCenter;
            this.PositionTextRegions(newCenter);
        }

        public override void Update()
        {
            base.Update();
            if (this.timer < this.duration)
            {
                if (this.timer < this.transitionDuration)
                {
                    this.backgroundShape.FillColor = ColorHelper.BlendAlpha(this.backColorTrans, this.backColor, timer / (float)this.transitionDuration);
                    this.UpdateTextColor(ColorHelper.BlendAlpha(this.textColorTrans, this.textColor, timer / (float)this.transitionDuration));
                }
                else if (this.timer == this.transitionDuration)
                {
                    this.backgroundShape.FillColor = this.backColor;
                    this.UpdateTextColor(this.textColor);
                }
                else if (this.timer > this.transitionDuration + this.holdDuration)
                {
                    this.backgroundShape.FillColor = ColorHelper.BlendAlpha(this.backColor, this.backColorTrans, (this.timer - (this.transitionDuration + this.holdDuration)) / (float)this.transitionDuration);
                    this.UpdateTextColor(ColorHelper.BlendAlpha(this.textColor, this.textColorTrans, (this.timer - (this.transitionDuration + this.holdDuration)) / (float)this.transitionDuration));
                }
                this.timer++;
                if (this.timer == this.duration && this.OnCompletion != null)
                {
                    this.OnCompletion();
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    for (int i = 0; i < this.texts.Length; i++)
                    {
                        this.texts[i].Dispose();
                    }
                    this.background.Dispose();
                    this.backgroundShape.Dispose();
                }
                this.pipeline.Remove(this.background);
                for (int j = 0; j < this.texts.Length; j++)
                {
                    this.pipeline.Remove(this.texts[j]);
                }
                ViewManager.Instance.OnMove -= this.ViewMoved;
                base.Dispose(disposing);
            }
        }

        private const int DEPTH = 2147467264;

        private const int MARGIN = 16;

        private const int WIDTH = 288;

        private const int HEIGHT = 148;

        private readonly RenderPipeline pipeline;

        private Color textColor;

        private Color textColorTrans;

        private TextRegion[] texts;

        private int timer;

        private readonly int duration;

        private readonly int transitionDuration;

        private readonly int holdDuration;

        private readonly FlyoverText.TextPosition textPosition;

        private readonly FontData font;

        private Color backColor;

        private Color backColorTrans;

        private readonly Shape backgroundShape;

        private readonly ShapeGraphic background;

        public enum TextPosition
        {
            Center,
            TopLeft,
            Top,
            TopRight,
            Left,
            Right,
            BottomLeft,
            Bottom,
            BottomRight
        }

        public delegate void OnCompletionHandler();
    }
}

using SFML.Graphics;
using SFML.System;
using System;
using VCO.Data;
using VCO.GUI.Modifiers;
using Violet.Graphics;

namespace VCO.Battle.UI
{
    internal class BattleMeter : IDisposable
    {
        public Vector2f Position
        {
            get => this.position;
            set => this.position = value;
        }

        public BattleMeter(RenderPipeline pipeline, Vector2f position, float initialFill, int depth)
        {
            this.targetFill = initialFill;
            this.fill = this.targetFill;
            this.meter = new IndexedColorGraphic(DataHandler.instance.Load("battleui2.dat"), "meter2", default(Vector2f), depth)
            {
                CurrentPalette = Settings.WindowFlavor
            };
            this.initialTextureRect = this.meter.TextureRect;
            this.hOffset = this.initialTextureRect.Height - (int)(initialTextureRect.Height * this.fill);
            this.position = position;
            this.meter.Position = new Vector2f(position.X, position.Y + hOffset);
            this.meter.TextureRect = new IntRect(this.initialTextureRect.Left, this.initialTextureRect.Top + this.hOffset, this.initialTextureRect.Width, this.initialTextureRect.Height - this.hOffset);
            pipeline.Add(this.meter);
            //meter.Visible = false;
            this.fillMaxThreshold = 1f - 1f / initialTextureRect.Height / 2f;
        }

        ~BattleMeter()
        {
            this.Dispose(false);
        }

        public void SetFill(float newFill)
        {
            this.targetFill = Math.Max(0f, Math.Min(1f, newFill));
        }

        public void SetGroovy(bool groovy)
        {
            bool flag = this.isGroovy;
            this.isGroovy = groovy;
            if (flag != this.isGroovy)
            {
                if (this.isGroovy)
                {
                    this.flashFlag = false;
                    this.fader = new GraphicFader(this.meter, BattleMeter.INITIAL_GLOW_COLOR, ColorBlendMode.Multiply, 6, 1);
                    return;
                }
                this.fader = null;
                this.meter.Color = Color.White;
                this.meter.ColorBlendMode = ColorBlendMode.Multiply;
            }
        }

        public void Update()
        {
            if (!this.isGroovy)
            {
                if (this.fill != this.targetFill)
                {
                    this.fill += Math.Max(-1f, Math.Min(1f, this.targetFill - this.fill)) / 10f;
                    this.hOffset = ((this.fill > this.fillMaxThreshold) ? 0 : (this.initialTextureRect.Height - (int)(initialTextureRect.Height * this.fill)));
                    this.meter.TextureRect = new IntRect(this.initialTextureRect.Left, this.initialTextureRect.Top + this.hOffset, this.initialTextureRect.Width, this.initialTextureRect.Height - this.hOffset);
                }
                if (this.targetFill >= 0.999999f && this.fill > this.fillMaxThreshold)
                {
                    this.SetGroovy(true);
                }
            }
            else if (this.fader != null)
            {
                this.fader.Update();
                if (this.fader.Done && !this.flashFlag)
                {
                    this.flashFlag = true;
                    this.fader = new GraphicFader(this.meter, BattleMeter.GLOW_COLOR, ColorBlendMode.Multiply, 6, -1);
                }
            }
            this.meter.Position = new Vector2f(this.position.X, this.position.Y + hOffset);
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    this.meter.Dispose();
                }
                this.disposed = true;
            }
        }

        private const int HEIGHT = 55;

        private const float FILL_FACTOR = 10f;

        private const int INITIAL_GLOW_FRAMES = 6;

        private const int GLOW_FRAMES = 6;

        private static Color INITIAL_GLOW_COLOR = new Color(100, 100, 100);

        private static Color GLOW_COLOR = new Color(128, 128, 128);

        private bool disposed;

        private IntRect initialTextureRect;

        private readonly IndexedColorGraphic meter;

        private float fill;

        private float targetFill;

        private readonly float fillMaxThreshold;

        private int hOffset;

        private Vector2f position;

        private bool isGroovy;

        private GraphicFader fader;

        private bool flashFlag;
    }
}

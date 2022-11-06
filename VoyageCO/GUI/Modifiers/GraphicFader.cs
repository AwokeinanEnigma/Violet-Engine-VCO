using SFML.Graphics;
using Violet.Graphics;
using Violet.Utility;

namespace VCO.GUI.Modifiers
{
    internal class GraphicFader : IGraphicModifier
    {
        public bool Done => this.done;

        public Graphic Graphic => this.graphic;

        public GraphicFader(IndexedColorGraphic graphic, Color color, ColorBlendMode blendMode, int duration, int count)
        {
            this.graphic = graphic;
            this.blendMode = blendMode;
            this.baseBlendMode = this.graphic.ColorBlendMode;
            this.baseColor = ((blendMode == ColorBlendMode.Screen) ? this.graphic.Color.Invert() : this.graphic.Color);
            this.color = color;
            this.duration = duration;
            this.total = count * 2;
        }

        public void Update()
        {
            if (!this.firstStep)
            {
                this.graphic.ColorBlendMode = this.blendMode;
                this.graphic.Color = this.baseColor;
                this.firstStep = true;
            }
            if (this.count >= this.total && this.total >= 0)
            {
                if (!this.done)
                {
                    this.graphic.Color = ((this.blendMode == ColorBlendMode.Screen) ? this.baseColor.Invert() : this.baseColor);
                    this.graphic.ColorBlendMode = this.baseBlendMode;
                    this.done = true;
                }
                return;
            }
            if (this.timer < this.duration)
            {
                this.graphic.Color = ColorHelper.Blend((this.count % 2 == 0) ? this.baseColor : this.color, (this.count % 2 == 1) ? this.baseColor : this.color, timer / (float)this.duration);
                this.timer++;
                return;
            }
            this.count++;
            this.graphic.Color = ((this.count % 2 == 0) ? this.baseColor : this.color);
            this.timer = 0;
        }

        private readonly IndexedColorGraphic graphic;

        private Color baseColor;

        private Color color;

        private readonly ColorBlendMode baseBlendMode;

        private readonly ColorBlendMode blendMode;

        private int count;

        private readonly int total;

        private int timer;

        private readonly int duration;

        private bool done;

        private bool firstStep;
    }
}

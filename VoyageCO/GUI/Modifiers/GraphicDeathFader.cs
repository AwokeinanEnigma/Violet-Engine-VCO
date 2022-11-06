using SFML.Graphics;
using Violet.Graphics;
using Violet.Utility;

namespace VCO.GUI.Modifiers
{
    internal class GraphicDeathFader : IGraphicModifier
    {
        public bool Done => this.done;

        public Graphic Graphic => this.graphic;

        public GraphicDeathFader(IndexedColorGraphic graphic, int frames)
        {
            this.graphic = graphic;
            this.graphic.Color = Color.Black;
            this.graphic.ColorBlendMode = ColorBlendMode.Screen;
            this.speed = 2f / frames;
        }

        public void Update()
        {
            if (!this.done)
            {
                if (this.progress < 1f)
                {
                    this.graphic.Color = ColorHelper.Blend(Color.Black, Color.White, this.progress);
                }
                else if (this.progress < 2f)
                {
                    if (this.progress < 1f + this.speed)
                    {
                        this.graphic.ColorBlendMode = ColorBlendMode.Replace;
                        this.graphic.Color = Color.White;
                    }
                    this.graphic.Color = ColorHelper.Blend(Color.White, Color.Black, this.progress - 1f);
                }
                else
                {
                    this.done = true;
                }
                this.progress += this.speed;
            }
        }

        private bool done;

        private readonly IndexedColorGraphic graphic;

        private float progress;

        private readonly float speed;
    }
}

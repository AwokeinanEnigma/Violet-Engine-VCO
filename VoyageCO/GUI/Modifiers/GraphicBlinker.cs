using Violet.Graphics;

namespace VCO.GUI.Modifiers
{
    internal class GraphicBlinker : IGraphicModifier
    {
        public bool Done => this.done;

        public Graphic Graphic => this.graphic;

        public GraphicBlinker(Graphic graphic, int duration, int count)
        {
            this.graphic = graphic;
            this.initialVisibility = graphic.Visible;
            this.duration = duration;
            this.total = count * 2;
        }

        public void Update()
        {
            if (this.count >= this.total && this.total >= 0)
            {
                if (!this.done)
                {
                    this.graphic.Visible = this.initialVisibility;
                    this.done = true;
                }
                return;
            }
            if (this.timer < this.duration)
            {
                this.timer++;
                return;
            }
            this.count++;
            this.graphic.Visible = !this.graphic.Visible;
            this.timer = 0;
        }

        private readonly Graphic graphic;

        private readonly bool initialVisibility;

        private int count;

        private readonly int total;

        private int timer;

        private readonly int duration;

        private bool done;
    }
}

using SFML.System;
using System;
using Violet.Graphics;

namespace VCO.GUI.Modifiers
{
    internal class GraphicTranslator : IGraphicModifier
    {
        public bool Done => this.done;

        public Graphic Graphic => this.graphic;

        public GraphicTranslator(Graphic graphic, Vector2f target, int frames)
        {
            this.graphic = graphic;
            this.target = target;
            this.moveVector = (this.target - this.graphic.Position) / frames;
            this.done = false;
        }

        public void Update()
        {
            float value = this.target.X - this.graphic.Position.X;
            float value2 = this.target.Y - this.graphic.Position.Y;
            this.done = (Math.Abs(value) <= 1f && Math.Abs(value2) <= 1f);
            if (!this.done)
            {
                this.graphic.Position += this.moveVector;
                return;
            }
            Console.WriteLine($"Moving {graphic} to {target}");
            if (!this.cleanupFlag)
            {
                this.graphic.Position = this.target;
                this.cleanupFlag = true;
            }
        }

        private readonly Graphic graphic;

        private Vector2f target;

        private Vector2f moveVector;

        private bool done;

        private bool cleanupFlag;
    }
}

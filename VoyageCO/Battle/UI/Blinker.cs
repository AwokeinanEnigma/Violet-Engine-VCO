using SFML.Graphics;
using System;
using Violet.Actors;
using Violet.Graphics;

namespace VCO.Battle.UI
{
    internal class Blinker : Actor
    {
        public event Blinker.CompletionHandler OnComplete;

        public Blinker(Graphic graphic, int blinks)
        {
            this.graphic = graphic;
            graphic.Color = new Color(0, 0, 0, 0);
            this.blinkCap = blinks;
        }

        public override void Update()
        {
            base.Update();
            this.graphic.Color = new Color(0, 0, 0, (byte)((Math.Sin(timer) + 1.0) / 2.0 * 255.0));
            if (timer > 6.283185307179586 * blinkCap)
            {
                if (this.OnComplete != null)
                {
                    this.OnComplete();
                    return;
                }
            }
            else
            {
                this.timer += 0.3926991f;
            }
        }

        private readonly Graphic graphic;

        private readonly int blinkCap;

        private float timer;

        public delegate void CompletionHandler();
    }
}

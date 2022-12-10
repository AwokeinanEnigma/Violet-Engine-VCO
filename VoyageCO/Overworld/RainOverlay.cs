using SFML.Graphics;
using SFML.System;
using System;
using Violet;
using Violet.Graphics;
using Violet.Utility;

namespace VCO.Overworld
{
    internal class RainOverlay
    {
        public RainOverlay()
        {
            this.drops = new RainOverlay.Droplet[50];
            this.splashes = new IndexedColorGraphic[this.drops.Length];
            for (int i = 0; i < this.drops.Length; i++)
            {
                int num = Engine.Random.Next(RainOverlay.COLOR_CHOICES.Length);
                this.drops[i] = new RainOverlay.Droplet(RainOverlay.COLOR_CHOICES[num]);
                int num2 = Engine.Random.Next(RainOverlay.SPLASH_CHOICES.Length);
                this.splashes[i] = new IndexedColorGraphic(DataHandler.instance.Load("rainsplash.dat"), RainOverlay.SPLASH_CHOICES[num2], new Vector2f(-9999f, -9999f), 0)
                {
                    Visible = false
                };
            }
        }
        public void Update()
        {
            for (int i = 0; i < this.drops.Length; i++)
            {
                bool flag = this.drops[i].Update();
                if (flag)
                {
                    IndexedColorGraphic splash = this.splashes[i];
                    //	Console.WriteLine(OverworldScene.instance.MapGroups[0].(splash.Position));
                    splash.Position = VectorMath.Truncate(this.drops[i].Position);
                    splash.Depth = 24;
                    splash.Frame = 0f;
                    splash.Visible = true;
                    splash.OnAnimationComplete += this.OnAnimationComplete;
                }
            }
        }
        private void OnAnimationComplete(AnimatedRenderable graphic)
        {
            graphic.Visible = false;
            graphic.OnAnimationComplete -= this.OnAnimationComplete;
        }
        public void Draw(RenderTarget target)
        {
            for (int i = 0; i < this.drops.Length; i++)
            {
                this.drops[i].Draw(target);
                if (this.splashes[i].Visible)
                {
                    this.splashes[i].Draw(target);
                }
            }
        }
        private static readonly Color[] COLOR_CHOICES = new Color[]
        {
            new Color(203, 219, 252),
            new Color(142, 177, 248),
            new Color(151, 170, 210)
        };
        private static readonly string[] SPLASH_CHOICES = new string[]
        {
            "splash1",
            "splash2",
            "splash3"
        };

        private readonly RainOverlay.Droplet[] drops;
        private readonly IndexedColorGraphic[] splashes;

        private struct Droplet
        {
            public Vector2f Position => this.position;
            public Droplet(Color color)
            {
                this.position = VectorMath.ZERO_VECTOR;
                this.verts = new Vertex[]
                {
                    new Vertex(this.position, color),
                    new Vertex(this.position - RainOverlay.Droplet.DROP_SIZE, color)
                };
                this.position = new Vector2f(ViewManager.Instance.Viewrect.Left + Engine.Random.Next(320), ViewManager.Instance.Viewrect.Top + Engine.Random.Next(180));
                this.endY = Math.Min(ViewManager.Instance.Viewrect.Top + 180f, this.position.Y + Engine.Random.Next(180));
            }

            private void ResetPosition()
            {
                this.position = new Vector2f(ViewManager.Instance.Viewrect.Left + Engine.Random.Next(320), ViewManager.Instance.Viewrect.Top - Engine.Random.Next(180));
                this.endY = this.position.Y + 180f;
                this.UpdateVertices();
            }

            private void UpdateVertices()
            {
                this.verts[0].Position = this.position;
                this.verts[1].Position = this.position - RainOverlay.Droplet.DROP_SIZE;
            }

            public bool Update()
            {
                bool result = false;
                this.position += RainOverlay.Droplet.DROP_VELOCITY;
                this.UpdateVertices();
                if (this.position.Y > this.endY)
                {
                    this.ResetPosition();
                }
                else if (this.position.Y + RainOverlay.Droplet.DROP_VELOCITY.Y > this.endY)
                {
                    result = true;
                }
                return result;
            }
            public void Draw(RenderTarget target)
            {
                target.Draw(this.verts, PrimitiveType.Lines);
            }
            private static readonly Vector2f DROP_SIZE = new Vector2f(0f, 24f);
            private static readonly Vector2f DROP_VELOCITY = new Vector2f(0f, 8f);
            private float endY;
            private Vector2f position;
            private readonly Vertex[] verts;
        }
    }
}

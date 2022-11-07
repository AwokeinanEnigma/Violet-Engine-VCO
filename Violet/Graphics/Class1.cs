using SFML.Graphics;
using SFML.System;
using Violet.Utility;

namespace Violet.Graphics
{
    public class ExperimentalColorGraphic : Graphic
    {

        public ExperimentalColorGraphic(FullColorTexture spriteName, Vector2f position, int depth)
        {
            // time = new Clock();
            // time.Restart();
            this.texture = spriteName;
            this.sprite = new Sprite(this.texture.Image);
            this.Position = position;
            this.sprite.Position = this.Position;
            this.Depth = depth;
            this.Rotation = 0f;
            this.scale = new Vector2f(1f, 1f);
            this.blend = Color.White;
            //multiply by default
            this.blendMode = ColorBlendMode.Multiply;
            this.renderStates = new RenderStates(BlendMode.Alpha, Transform.Identity, null, INDEXED_COLOR_SHADER);
            this.animationEnabled = true;
            this.Visible = true;
        }

        public override void Draw(RenderTarget target)
        {
            if (!this.disposed && this.visible)
            {
                if (base.Frames > 1 && this.animationEnabled)
                {
                    base.UpdateAnimation();
                }
                this.sprite.Position = this.Position;
                this.sprite.Origin = this.Origin;
                this.sprite.Rotation = this.Rotation;
                this.finalScale.X = (this.flipX ? (-this.scale.X) : this.scale.X);
                this.finalScale.Y = (this.flipY ? (-this.scale.Y) : this.scale.Y);
                this.sprite.Scale = this.finalScale;
                Debug.LogL("1.1");
                //INDEXED_COLOR_SHADER.SetParameter("time", time.ElapsedTime.AsSeconds());
                if (!this.disposed)
                {
                    target.Draw(this.sprite, this.renderStates);
                }
            }
        }

        public FullColorTexture CopyToTexture()
        {
            return ((IndexedTexture)this.texture).ToFullColorTexture();
        }

        private static readonly Shader INDEXED_COLOR_SHADER = new Shader(EmbeddedResources.GetStream("Violet.Resources.pal.vert"), null, EmbeddedResources.GetStream("Violet.Resources.pal.frag"));

        private RenderStates renderStates;

        private ColorBlendMode blendMode;

        private bool flipX;

        private bool flipY;

        private int mode; 

        private float betaFrame;

        private uint previousPalette;

        private uint currentPalette;

        private Color blend;

        private bool animationEnabled;
    }
}

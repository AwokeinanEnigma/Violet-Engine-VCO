using SFML.Graphics;
using SFML.System;
using Violet.Graphics;
using Violet.Utility;

namespace VCO.Overworld
{
    internal class ParallaxBackground : TiledBackground
    {
        public Vector2f Vector
        {
            get => this.vector;
            set => this.vector = value;
        }

        private enum ParallaxShaderMode
        {
            Invalid = -1,
            None,
            Default,
            WavyAndShavy,
        }

        private ParallaxShaderMode mode;

        public ParallaxBackground(string sprite, Vector2f vector, IntRect area, int depth, int shaderMode) : base(sprite, area, true, true, VectorMath.ZERO_VECTOR, depth)
        {
            this.mode = (ParallaxShaderMode. shaderMode);

            this.vector = vector;
            this.areaPoint = new Vector2f(area.Left, area.Top);
            this.areaDimensions = new Vector2f(area.Width, area.Height);
            this.position = this.areaPoint;
            this.size = this.areaDimensions;
            this.w = this.areaDimensions.X - this.texture.Image.Size.X;
            this.h = this.areaDimensions.Y - this.texture.Image.Size.Y;
            this.tw = (this.texture.Image.Size.X - 320U) / 2f;
            this.th = (this.texture.Image.Size.Y - 180U) / 2f;
            this.Update();
            

            SetShader();
        }

        private void Update()
        {
            float num = ViewManager.Instance.FinalCenter.X - 160f;
            float num2 = ViewManager.Instance.FinalCenter.Y - 90f;
            this.previousPosition = this.position;
            this.position.X = num + (this.areaPoint.X - num) / this.w * (this.vector.X * this.tw);
            this.position.Y = num2 + (this.areaPoint.Y - num2) / this.h * (this.vector.Y * this.th);
            for (int i = 0; i < this.yRepeatCount; i++)
            {
                for (int j = 0; j < this.xRepeatCount; j++)
                {
                    this.sprites[j, i].Position += this.position - this.previousPosition;
                }
            }
        }
        protected override void SetShader()
        {
            switch (mode)
            {
                case ParallaxShaderMode.Default:
                    this.shader = new Shader(EmbeddedResources.GetStream("Violet.Resources.pal.vert"), null, EmbeddedResources.GetStream("Violet.Resources.pal.frag"));
                    this.shader.SetUniform("image", this.texture.Image);
                    this.shader.SetUniform("palette", this.texture.Palette);
                    this.shader.SetUniform("palIndex", 0f);
                    this.shader.SetUniform("palSize", this.texture.PaletteSize);
                    this.shader.SetUniform("blend", new SFML.Graphics.Glsl.Vec4(Color.White));
                    this.shader.SetUniform("blendMode", 1f);
                    this.states = new RenderStates(BlendMode.Alpha, Transform.Identity, this.texture.Image, this.shader); return;

                case ParallaxShaderMode.WavyAndShavy:
                    shader = new Shader(VCO.Utility.EmbeddedResources.GetStream("VCO.Resources.bbg.vert"), null, Utility.EmbeddedResources.GetStream("VCO.Resources.WavyAndShavy.frag"));
                    this.shader.SetUniform("texture", this.texture.Image);
                    this.shader.SetUniform("pixel_threshold", 3);

                    states = new RenderStates(BlendMode.Alpha, Transform.Identity, this.texture.Image, this.shader);
                    break;
            }
        }

        public override void Draw(RenderTarget target)
        {
            this.Update();

            switch (mode) {
                case ParallaxShaderMode.Default:
                    base.Draw(target);
                    break;
                case ParallaxShaderMode.WavyAndShavy:
                    base.Draw(target);
                    break;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                TextureManager.Instance.Unuse(this.texture);
            }
            this.disposed = true;
        }

        private Vector2f previousPosition;

        private Vector2f vector;

        private Vector2f areaPoint;

        private Vector2f areaDimensions;

        private readonly float w;

        private readonly float h;

        private readonly float tw;

        private readonly float th;
    }
}
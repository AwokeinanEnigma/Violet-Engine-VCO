using SFML.Graphics;
using SFML.System;
using System;
using Violet.Graphics;

namespace VCO.Battle.Background
{
    internal class BackgroundLayer : IDisposable
    {
        public BackgroundLayer(Shader shader, LayerParams newParams)
        {
            this.shader = shader;
            this.parameters = newParams;
            this.SetupTexture();
        }

        ~BackgroundLayer()
        {
            this.Dispose(false);
        }

        private void SetupTexture()
        {
            this.texture = TextureManager.Instance.Use(this.parameters.File);
            this.texture.Image.Repeated = true;
            this.states = new RenderStates(BlendMode.None, Transform.Identity, this.texture.Image, this.shader);
            uint x = this.texture.Image.Size.X;
            uint y = this.texture.Image.Size.Y;
            this.verts = new VertexArray(PrimitiveType.Quads, 4U);
            this.verts[0U] = new Vertex(new Vector2f(0f, 0f), new Vector2f(0f, 1f));
            this.verts[1U] = new Vertex(new Vector2f(x, 0f), new Vector2f(1f, 1f));
            this.verts[2U] = new Vertex(new Vector2f(x, y), new Vector2f(1f, 0f));
            this.verts[3U] = new Vertex(new Vector2f(0f, y), new Vector2f(0f, 0f));
            this.time = 0f;
            this.xTrans = 0f;
            this.yTrans = 0f;
            this.factors = new float[this.parameters.Variation.Length];
            this.palChangeIndex = 0;
            this.palFrame = 0;
        }

        public void UpdateParameters(LayerParams newParams)
        {
            LayerParams layerParams = this.parameters;
            this.parameters = newParams;
            if (layerParams.File != this.parameters.File)
            {
                TextureManager.Instance.Unuse(this.texture);
                this.SetupTexture();
            }
        }

        public void ActiveUpdate()
        {

        }

        public void ResetTranslation()
        {
            this.xTrans = 0f;
            this.yTrans = 0f;
        }

        public void AddTranslation(float x, float y, float xFactor, float yFactor)
        {
            this.xTrans += x * xFactor;
            this.yTrans += y * yFactor;
        }




        public void Draw(RenderTexture oldFrame, RenderTexture newFrame)
        {
            this.shader.SetUniform("layerTex", this.texture.Image);
            this.shader.SetUniform("bottomTex", oldFrame.Texture);
            this.shader.SetUniform("height", 180f);
            int num = this.factors.Length;
            for (int i = 0; i < num; i++)
            {
                float a = this.parameters.Variation[i].A;
                float b = this.parameters.Variation[i].B;
                float c = this.parameters.Variation[i].C;
                float d = this.parameters.Variation[i].D;
                float e = this.parameters.Variation[i].E;
                switch (this.parameters.Variation[i].Mode)
                {
                    case 0:
                        this.factors[i] = a;
                        break;
                    case 1:
                        this.factors[i] = (float)(a * Math.Sin(b * this.time + c) + d);
                        break;
                    case 2:
                        this.factors[i] = (float)(a * Math.Sin(b * this.time + c) * Math.Sin(d * this.time) + e);
                        break;
                }
            }
            this.shader.SetUniform("time", this.time);
            this.shader.SetUniform("amp", this.parameters.Amplitude * this.factors[3]);
            this.shader.SetUniform("freq", this.parameters.Frequency * this.factors[0]);
            this.shader.SetUniform("scale", this.parameters.Scale * this.factors[1]);
            this.shader.SetUniform("comp", this.parameters.Compression * this.factors[2]);
            this.shader.SetUniform("blend", parameters.Blend);
            this.shader.SetUniform("mode", parameters.Mode);
            this.shader.SetUniform("opacity", this.parameters.Opacity * this.factors[5]);
            float num2 = this.parameters.Speed * this.factors[4];
            this.xTrans += this.parameters.Xtrans * this.factors[6] * num2;
            this.yTrans += this.parameters.Ytrans * this.factors[7] * num2;
            this.xTrans %= 320f;
            this.yTrans %= 180f;
            this.shader.SetUniform("xTrans", this.xTrans);
            this.shader.SetUniform("yTrans", this.yTrans);
            this.shader.SetUniform("palette", this.texture.Palette);
            if (this.parameters.Palette.Length > 0 && this.palChangeIndex < this.parameters.Palette.Length)
            {
                this.texture.CurrentPalette = (uint)this.parameters.Palette[this.palChangeIndex].Index;
                this.shader.SetUniform("palIndex", this.texture.CurrentPaletteFloat);
                this.shader.SetUniform("palSize", this.texture.PaletteSize);
                this.shader.SetUniform("palShift", (float)this.parameters.Palette[this.palChangeIndex].Shift / this.texture.PaletteSize);
                if (this.palFrame < this.parameters.Palette[this.palChangeIndex].Duration)
                {
                    this.palFrame++;
                }
                else
                {
                    this.palChangeIndex = (this.palChangeIndex + 1) % this.parameters.Palette.Length;
                    this.palFrame = 0;
                }
            }
            else
            {
                this.palChangeIndex = 0;
                this.palFrame = 0;
                this.shader.SetUniform("palIndex", 0f);
                this.shader.SetUniform("palSize", this.texture.PaletteSize);
                this.shader.SetUniform("palShift", 0f);
            }
            newFrame.Draw(this.verts, this.states);
            this.time += num2;
        }

        public virtual void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                TextureManager.Instance.Unuse(this.texture);
            }
            this.disposed = true;
        }

        public LayerParams Parameters => parameters;

        protected bool disposed;

        private LayerParams parameters;

        private float time;

        private float xTrans;

        private float yTrans;

        private float[] factors;

        private int palChangeIndex;

        private int palFrame;

        private readonly Shader shader;

        private RenderStates states;

        private IndexedTexture texture;

        private VertexArray verts;
    }
}

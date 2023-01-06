using SFML.Graphics;
using SFML.System;
using System;
using Violet.Graphics;
using Violet.Utility;

namespace Violet.GUI
{
    public class TextRegion : Renderable
    {
        public override Vector2f Position
        {
            get => this.position;
            set
            {
                this.position = value;
                this.drawText.Position = new Vector2f(this.position.X + (float)this.font.XCompensation, this.position.Y + (float)this.font.YCompensation);
            }
        }

        public string Text
        {
            get => this.text;

            set
            {
                this.text = value;
                this.dirtyText = true;
            }
        }

        public int Index
        {
            get => this.index;
            set
            {
                this.index = value;
                this.dirtyText = true;
            }
        }

        public int Length
        {
            get => this.length;
            set
            {
                int currentLength = this.length;
                this.length = value;
                this.dirtyText = (this.length != currentLength);
            }
        }

        public Color Color
        {
            get => this.drawText.FillColor;
            set
            {
                this.drawText.FillColor = value;
                this.dirtyColor = true;
            }
        }

        public FontData FontData
        {
            get => this.font;

        }

        private RenderStates renderStates;
        private Text drawText;

        private string text;
        private int index;
        private int length;

        private bool dirtyText;

        private bool dirtyColor;

        private FontData font;


        public TextRegion(Vector2f position, int depth, FontData font, string text) : this(position, depth, font, (text != null) ? text : string.Empty, 0, (text != null) ? text.Length : 0) { }

        public TextRegion(Vector2f position, int depth, FontData font, string text, int index, int length)
        {
            this.position = position;
            this.text = text;
            this.index = index;
            this.length = length;
            this.depth = depth;
            this.font = font;
            this.drawText = new Text(string.Empty, this.font.Font, this.font.Size);
            this.drawText.Position = new Vector2f(position.X + (float)this.font.XCompensation, position.Y + (float)this.font.YCompensation);
            this.UpdateText(index, length);
            this.shader = new Shader(EmbeddedResources.GetStream("Violet.Resources.text.vert"), null, EmbeddedResources.GetStream("Violet.Resources.text.frag"));
            this.shader.SetUniform("color", new SFML.Graphics.Glsl.Vec4(this.drawText.FillColor));
            this.shader.SetUniform("threshold", font.AlphaThreshold);
            this.renderStates = new RenderStates(BlendMode.Alpha, Transform.Identity, null, this.shader);
        }

        public Vector2f FindCharacterPosition(uint index)
        {
            uint num = Math.Max(0U, Math.Min((uint)this.text.Length, index));
            return this.drawText.FindCharacterPos(num);
        }

        public void Reset(string text, int index, int length)
        {
            this.text = text;
            this.index = index;
            this.length = length;
            this.UpdateText(index, length);
        }

        private void UpdateText(int index, int length)
        {
            this.drawText.DisplayedString = this.text.Substring(index, length);
            FloatRect localBounds = this.drawText.GetLocalBounds();
            this.size = new Vector2f(Math.Max(1f, localBounds.Width), Math.Max(1f, localBounds.Height));
        }

        public override void Draw(RenderTarget target)
        {
            if (this.dirtyText)
            {
                this.UpdateText(this.index, Math.Min(this.text.Length, this.length));
                this.dirtyText = false;
            }
            if (this.dirtyColor)
            {
                this.shader.SetParameter("color", this.drawText.Color);
            }
            target.Draw(this.drawText, this.renderStates);
        }

        protected override void Dispose(bool disposing)
        {
            if (!this.disposed && disposing)
            {
                this.drawText.Dispose();
            }
            this.disposed = true;
        }

        private Shader shader;


    }
}

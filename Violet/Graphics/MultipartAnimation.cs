using SFML.Graphics;
using SFML.System;
using Violet.Utility;

namespace Violet.Graphics
{
    /// <summary>
    /// An animation with multiple parts
    /// </summary>
    public class MultipartAnimation : Renderable
    {
        /// <summary>
        /// The scale of each animation 
        /// </summary>
        public virtual Vector2f Scale { get; set; }

        /// <summary>
        /// When the animation is complete
        /// </summary>
        public event MultipartAnimation.AnimationCompleteHandler OnAnimationComplete;

        /// <summary>
        /// Creates a new multipart animation
        /// </summary>
        /// <param name="resource">The name of the animations to pull from the texture manager</param>
        /// <param name="position">The position of each animation</param>
        /// <param name="speed">The speed of each animation</param>
        /// <param name="depth">The depth of each animation</param>
        public MultipartAnimation(string resource, Vector2f position, float speed, int depth)
        {
            this.textures = TextureManager.Instance.UseMultipart(resource);
            this.sprite = new Sprite(this.textures[0].Image);
            this.frames = this.textures.Length;
            this.speed = speed;
            this.Position = position;
            this.Origin = this.textures[0].GetSpriteDefinition("default").Origin;
            this.Depth = depth;
            this.Size = new Vector2f(this.textures[0].Image.Size.X, this.textures[0].Image.Size.Y);
            this.Scale = new Vector2f(1f, 1f);
            this.blend = Color.White;
            this.shader = new Shader(EmbeddedResources.GetStream("Violet.Resources.pal.vert"), null, EmbeddedResources.GetStream("Violet.Resources.pal.frag"));
            this.shader.SetUniform("image", this.textures[0].Image);
            this.shader.SetUniform("palette", this.textures[0].Palette);
            this.shader.SetUniform("palIndex", this.textures[0].CurrentPaletteFloat);
            this.shader.SetUniform("palSize", this.textures[0].PaletteSize);
            this.shader.SetUniform("blend", new SFML.Graphics.Glsl.Vec4(this.blend));
            this.shader.SetUniform("blendMode", 1f);
            this.renderStates = new RenderStates(BlendMode.Alpha, Transform.Identity, null, this.shader);
            this.Visible = true;
        }

        /// <summary>
        /// Resets every variable to its default position.
        /// </summary>
        public void Reset()
        {
            this.frame = 0f;
            this.intFrame = (int)this.frame;
            this.lastFrame = this.intFrame;
            this.sprite.Texture = this.textures[this.intFrame].Image;
            this.renderStates.Texture = this.sprite.Texture;
            this.shader.SetUniform("image", this.sprite.Texture);
        }

        /// <summary>
        /// Updates the animation and pushes it by the frames specified
        /// </summary>
        /// <param name="newFrame">How many frames to push the animation forward by.</param>
        private void UpdateFrame(float newFrame)
        {
            this.frame = newFrame % frames;
            this.lastFrame = this.intFrame;
            this.intFrame = (int)this.frame;
            if (newFrame >= frames)
            {
                this.OnAnimationComplete?.Invoke(this);
            }
            if (this.visible && this.intFrame != this.lastFrame)
            {
                this.sprite.Texture = this.textures[this.intFrame].Image;
                this.renderStates.Texture = this.sprite.Texture;
                this.shader.SetUniform("image", this.sprite.Texture);
            }
        }

        public override void Draw(RenderTarget target)
        {
            if (this.visible)
            {
                float newFrame = this.frame + this.speed;
                this.UpdateFrame(newFrame);
                this.sprite.Position = this.Position;
                this.sprite.Origin = this.Origin;
                this.sprite.Scale = this.Scale;
                target.Draw(this.sprite, this.renderStates);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    this.sprite.Dispose();
                }
                for (int i = 0; i < this.textures.Length; i++)
                {
                    TextureManager.Instance.Unuse(this.textures[i]);
                }
            }
            base.Dispose(disposing);
        }

        private const string SPRITE_NAME = "default";

        private Shader shader;

        private RenderStates renderStates;

        private IndexedTexture[] textures;

        private Sprite sprite;

        private Color blend;

        private float frame;

        private float speed;

        private int lastFrame;

        private int intFrame;

        private int frames;

        public delegate void AnimationCompleteHandler(MultipartAnimation anim);
    }
}

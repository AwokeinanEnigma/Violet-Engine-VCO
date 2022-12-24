using SFML.Graphics;
using SFML.System;
using Violet.Utility;
using static Violet.Graphics.SpriteDefinition;

namespace Violet.Graphics
{
    public class IndexedColorGraphic : Graphic
    {
        #region Properties
        /// <summary>
        /// The current palette this IndexedColorGraphic is using.
        /// </summary>
        public uint CurrentPalette
        {
            get
            {
                return this.currentPalette;
            }
            set
            {
                if (this.currentPalette != value)
                {
                    this.previousPalette = this.currentPalette;
                    this.currentPalette = value;
                }
            }
        }

        /// <summary>
        /// The color to overlay the texture with.
        /// </summary>
        public override Color Color
        {
            get
            {
                return this.blend;
            }
            set
            {
                this.blend = value;
            }
        }

        /// <summary>
        /// The mode to use when blending the texture with the color.
        /// </summary>
        public ColorBlendMode ColorBlendMode
        {
            get
            {
                return this.blendMode;
            }
            set
            {
                this.blendMode = value;
            }
        }

        /// <summary>
        /// Self explanatory. The last palette used. 
        /// </summary>
        public uint PreviousPalette
        {
            get
            {
                return this.previousPalette;
            }
        }

        /// <summary>
        /// The render states this IndexedColorGraphic is using.
        /// </summary>
        public RenderStates RenderStates
        {
            get
            {
                return this.renderStates;
            }
        }

        /// <summary>
        /// Can this graphic be animated?
        /// </summary>
        public bool AnimationEnabled
        {
            get
            {
                return this.animationEnabled;
            }
            set
            {
                this.animationEnabled = value;
            }
        }
        #endregion

        #region Fields
        private static readonly int[] MODE_ONE_FRAMES = new int[] {
            0,
            1,
            0,
            2
        };

        // shaaaaaaaaaaaaader!
        private static readonly Shader INDEXED_COLOR_SHADER = new Shader(EmbeddedResources.GetStream("Violet.Resources.pal.vert"), null, EmbeddedResources.GetStream("Violet.Resources.pal.frag"));

        private RenderStates renderStates;

        // fliped stuff
        private bool flipX;
        private bool flipY;

        // palette stuff
        private uint previousPalette;
        private uint currentPalette;

        // color stuff
        private Color blend;
        private ColorBlendMode blendMode;

        // it's all stuff
        // animation stuff
        private AnimationMode mode;
        private float betaFrame;
        private bool animationEnabled; 
        #endregion

        /// <summary>
        /// Creates a new IndexedColorGraphic.
        /// </summary>
        /// <param name="resource">The name of the sprite file.</param>
        /// <param name="spriteName">The sprite to initialize the IndexedColorGraphic with. This will be the starting sprite it uses.</param>
        /// <param name="position">Where the sprite is located.</param>
        /// <param name="depth">The depth of the sprite.</param>
        public IndexedColorGraphic(string resource, string spriteName, Vector2f position, int depth)
        {
            this.texture = TextureManager.Instance.Use(resource);
            this.sprite = new Sprite(this.texture.Image);
            this.Position = position;
            this.sprite.Position = this.Position;
            this.Depth = depth;
            this.Rotation = 0f;
            this.scale = new Vector2f(1f, 1f);
            this.SetSprite(spriteName);
            ((IndexedTexture)this.texture).CurrentPalette = this.currentPalette;
            this.blend = Color.White;
            //multiply by default
            this.blendMode = ColorBlendMode.Multiply;
            this.renderStates = new RenderStates(BlendMode.Alpha, Transform.Identity, null, IndexedColorGraphic.INDEXED_COLOR_SHADER);
            this.animationEnabled = true;
            this.Visible = true;
        }

        /// <summary>
        /// Creates a new IndexedColorGraphic.
        /// </summary>
        /// <param name="texture">The texture to use with the IndexedColorGraphic.</param>
        /// <param name="spriteName">The sprite to initialize the IndexedColorGraphic with. This will be the starting sprite it uses.</param>
        /// <param name="position">Where the sprite is located.</param>
        /// <param name="depth">The depth of the sprite.</param>
        public IndexedColorGraphic(IndexedTexture texture, string spriteName, Vector2f position, int depth)
        {
            this.texture = texture;
            this.sprite = new Sprite(this.texture.Image);
            this.Position = position;
            this.sprite.Position = this.Position;
            this.Depth = depth;
            this.Rotation = 0f;
            this.scale = new Vector2f(1f, 1f);
            this.SetSprite(spriteName);
            ((IndexedTexture)this.texture).CurrentPalette = this.currentPalette;
            this.blend = Color.White;
            //multiply by default
            this.blendMode = ColorBlendMode.Multiply;
            this.renderStates = new RenderStates(BlendMode.Alpha, Transform.Identity, null, IndexedColorGraphic.INDEXED_COLOR_SHADER);
            this.animationEnabled = true;
            this.Visible = true;
        }

        /// <summary>
        /// Creates a new IndexedColorGraphic.
        /// </summary>
        /// <param name="resource">The name of the sprite file.</param>
        /// <param name="spriteName">The sprite to initialize the IndexedColorGraphic with. This will be the starting sprite it uses.</param>
        /// <param name="position">Where the sprite is located.</param>
        /// <param name="depth">The depth of the sprite.</param>
        /// <param name="palette">The palette to initialize the IndexedColorGraphic with. This will be the starting palette it uses.</param>
        public IndexedColorGraphic(string resource, string spriteName, Vector2f position, int depth, uint palette)
        {
            this.texture = TextureManager.Instance.Use(resource);
            this.sprite = new Sprite(this.texture.Image);

            this.Position = position;
            this.sprite.Position = this.Position;

            this.Depth = depth;

            this.Rotation = 0f;

            this.scale = new Vector2f(1f, 1f);

            this.SetSprite(spriteName);

            currentPalette = palette;
            ((IndexedTexture)this.texture).CurrentPalette = palette;

            this.blend = Color.White;
            //multiply by default
            this.blendMode = ColorBlendMode.Multiply;
            this.renderStates = new RenderStates(BlendMode.Alpha, Transform.Identity, null, IndexedColorGraphic.INDEXED_COLOR_SHADER);
            this.animationEnabled = true;

            this.Visible = true;
        }


        public void SetSprite(string name)
        {
            this.SetSprite(name, true);
        }

        public void SetSprite(string name, bool reset)
        {
            SpriteDefinition spriteDefinition = ((IndexedTexture)this.texture).GetSpriteDefinition(name);
            if (spriteDefinition == null)
            {
                spriteDefinition = ((IndexedTexture)this.texture).GetDefaultSpriteDefinition();
            }
          
            this.sprite.Origin = spriteDefinition.Origin;
            this.Origin = spriteDefinition.Origin;
          
            this.sprite.TextureRect = new IntRect(spriteDefinition.Coords.X, spriteDefinition.Coords.Y, spriteDefinition.Bounds.X, spriteDefinition.Bounds.Y);
            this.startTextureRect = this.sprite.TextureRect;
            this.Size = new Vector2f(sprite.TextureRect.Width, sprite.TextureRect.Height);
         
            this.flipX = spriteDefinition.FlipX;
            this.flipY = spriteDefinition.FlipY;
           
            this.finalScale.X = (this.flipX ? (-this.scale.X) : this.scale.X);
            this.finalScale.Y = (this.flipY ? (-this.scale.Y) : this.scale.Y);
          
            this.sprite.Scale = this.finalScale;
            base.Frames = spriteDefinition.Frames;
            base.Speeds = spriteDefinition.Speeds;
            this.mode = spriteDefinition.Mode;
          
            if (reset)
            {
                this.frame = 0f;
                this.betaFrame = 0f;
                this.speedIndex = 0f;
                this.speedModifier = 1f;
                return;
            }
            this.frame %= Frames;
        }

        //  public Clock time;

        protected override void IncrementFrame()
        {
            float frameSpeed = base.GetFrameSpeed();
           
            switch (this.mode)
            {
                case AnimationMode.Continous:
                    this.frame = (this.frame + frameSpeed) % Frames;
                    break;

                case AnimationMode.ZeroTwoOneThree:
                    this.betaFrame = (this.betaFrame + frameSpeed) % 4f;
                    this.frame = IndexedColorGraphic.MODE_ONE_FRAMES[(int)this.betaFrame];
                    break;
            }

            this.speedIndex = (int)this.frame % this.speeds.Length;
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
                ((IndexedTexture)this.texture).CurrentPalette = this.currentPalette;
                IndexedColorGraphic.INDEXED_COLOR_SHADER.SetUniform("image", this.texture.Image);
                IndexedColorGraphic.INDEXED_COLOR_SHADER.SetUniform("palette", ((IndexedTexture)this.texture).Palette);
                IndexedColorGraphic.INDEXED_COLOR_SHADER.SetUniform("palIndex", ((IndexedTexture)this.texture).CurrentPaletteFloat);
                //Debug.Log($"Current palette is {currentPalette}. Texture's palette is {((IndexedTexture)this.texture).CurrentPalette}. Float palette is {((IndexedTexture)this.texture).CurrentPaletteFloat} & the palette max is {((IndexedTexture)this.texture).PaletteCount}. {(float)CurrentPalette/ (float)((IndexedTexture)this.texture).PaletteCount} ");
                IndexedColorGraphic.INDEXED_COLOR_SHADER.SetUniform("palSize", ((IndexedTexture)this.texture).PaletteSize);
                IndexedColorGraphic.INDEXED_COLOR_SHADER.SetUniform("blend", new SFML.Graphics.Glsl.Vec4(this.blend));
                IndexedColorGraphic.INDEXED_COLOR_SHADER.SetUniform("blendMode", (float)this.blendMode);
                //IndexedColorGraphic.INDEXED_COLOR_SHADER.SetUniform("time", time.ElapsedTime.AsSeconds());
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

        public SpriteDefinition GetSpriteDefinition(string sprite)
        {
            int hashCode = sprite.GetHashCode();
            return ((IndexedTexture)this.texture).GetSpriteDefinition(hashCode);
        }
    }
}

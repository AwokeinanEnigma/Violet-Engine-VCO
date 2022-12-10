using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using Violet.Graphics;
using Violet.Utility;

namespace Violet.Tiles
{
    public class TileGroup : Renderable, IDisposable
    {
        #region Properties
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

        public override Vector2f Position
        {
            get
            {
                return this.position;
            }
            set
            {
                this.position = value;
                this.ResetTransform();
            }
        }

        public override Vector2f Origin
        {
            get
            {
                return this.origin;
            }
            set
            {
                this.origin = value;
                this.ResetTransform();
            }
        }

        public IndexedTexture Tileset
        {
            get
            {
                return this.tileset;
            }
        }
        #endregion

        #region Private fields
        private Vertex[] vertices;

        private IndexedTexture tileset;

        private RenderStates renderState;

        private TileGroup.TileAnimation[] animations;

        private bool animationEnabled;
        #endregion

        private struct TileAnimation
        {
            public int[] Tiles;

            public IList<int> VertIndexes;

            public float Speed;
        }
        public TileGroup(IList<Tile> tiles, string resource, int depth, Vector2f position, uint palette)
        {
            //Debug.Log($"trying to! {resource}");
            this.tileset = TextureManager.Instance.Use(resource);
            this.tileset.CurrentPalette = palette;
            this.position = position;
            this.depth = depth;
            this.renderState = new RenderStates(BlendMode.Alpha, Transform.Identity, this.tileset.Image, TileGroup.TILE_GROUP_SHADER);
            this.animationEnabled = true;
            this.CreateAnimations(this.tileset.GetSpriteDefinitions());

            // this is like putting a screaming baby on mute, but it works!
            // update: it did not work
            // try
            // {
            this.CreateVertexArray(tiles);
            // }

            this.ResetTransform();
        }

        private void ResetTransform()
        {
            Transform identity = Transform.Identity;
            identity.Translate(this.position - this.origin);
            this.renderState.Transform = identity;
        }

        public int GetTileId(Vector2f location)
        {
            Vector2f vector2f = location - this.position + this.origin;
            uint num = (uint)(vector2f.X / 8f + vector2f.Y / 8f * (this.size.X / 8f));
            Vertex vertex = this.vertices[(int)((UIntPtr)(num * 4U))];
            Vector2f texCoords = vertex.TexCoords;
            return (int)(texCoords.X / 8f + texCoords.Y / 8f * (this.tileset.Image.Size.X / 8U));
        }

        private void IDToTexCoords(uint id, out uint tx, out uint ty)
        {
            tx = id * 8U % this.tileset.Image.Size.X;
            ty = id * 8U / this.tileset.Image.Size.X * 8U;
        }

        private void CreateAnimations(ICollection<SpriteDefinition> definitions)
        {
            this.animations = new TileGroup.TileAnimation[definitions.Count];
            foreach (SpriteDefinition spriteDefinition in definitions)
            {
                int num = -1;
                int.TryParse(spriteDefinition.Name, out num);
                if (num >= 0)
                {
                    if (spriteDefinition.Data != null && spriteDefinition.Data.Length > 0)
                    {
                        int[] data = spriteDefinition.Data;
                        float speed = spriteDefinition.Speeds[0];
                        this.animations[num].Tiles = data;
                        this.animations[num].VertIndexes = new List<int>();
                        this.animations[num].Speed = speed;
                    }
                    else
                    {
                        Debug.LogWarning($"Tried to load tile animation data for animation {num}, but there was no tile data.");
                    }
                }
            }
        }

        private void AddVertIndex(Tile tile, int index)
        {
            if (tile.AnimationId > 0)
            {
                int num = tile.AnimationId - 1;
                this.animations[num].VertIndexes.Add(index);
            }
        }

        private unsafe void CreateVertexArray(IList<Tile> tiles)
        {
            this.vertices = new Vertex[tiles.Count * 4];
            uint tileX = 0U;
            uint tileY = 0U;
            Vector2f v = default(Vector2f);
            Vector2f v2 = default(Vector2f);
            fixed (Vertex* ptr = this.vertices)
            {
                for (int i = 0; i < tiles.Count; i++)
                {
                    Vertex* ptr2 = ptr + i * 4;
                    Tile tile = tiles[i];
                    float x = tile.Position.X;
                    float y = tile.Position.Y;
                    ptr2->Position.X = x;
                    ptr2->Position.Y = y;
                    ptr2[1].Position.X = x + 8f;
                    ptr2[1].Position.Y = y;
                    ptr2[2].Position.X = x + 8f;
                    ptr2[2].Position.Y = y + 8f;
                    ptr2[3].Position.X = x;
                    ptr2[3].Position.Y = y + 8f;
                    this.IDToTexCoords(tile.ID, out tileX, out tileY);
                    if (!tile.FlipHorizontal && !tile.FlipVertical)
                    {
                        ptr2->TexCoords.X = tileX;
                        ptr2->TexCoords.Y = tileY;
                        ptr2[1].TexCoords.X = tileX + 8U;
                        ptr2[1].TexCoords.Y = tileY;
                        ptr2[2].TexCoords.X = tileX + 8U;
                        ptr2[2].TexCoords.Y = tileY + 8U;
                        ptr2[3].TexCoords.X = tileX;
                        ptr2[3].TexCoords.Y = tileY + 8U;
                    }
                    else if (tile.FlipHorizontal && !tile.FlipVertical)
                    {
                        ptr2->TexCoords.X = tileX + 8U;
                        ptr2->TexCoords.Y = tileY;
                        ptr2[1].TexCoords.X = tileX;
                        ptr2[1].TexCoords.Y = tileY;
                        ptr2[2].TexCoords.X = tileX;
                        ptr2[2].TexCoords.Y = tileY + 8U;
                        ptr2[3].TexCoords.X = tileX + 8U;
                        ptr2[3].TexCoords.Y = tileY + 8U;
                    }
                    else if (!tile.FlipHorizontal && tile.FlipVertical)
                    {
                        ptr2->TexCoords.X = tileX;
                        ptr2->TexCoords.Y = tileY + 8U;
                        ptr2[1].TexCoords.X = tileX + 8U;
                        ptr2[1].TexCoords.Y = tileY + 8U;
                        ptr2[2].TexCoords.X = tileX + 8U;
                        ptr2[2].TexCoords.Y = tileY;
                        ptr2[3].TexCoords.X = tileX;
                        ptr2[3].TexCoords.Y = tileY;
                    }
                    else
                    {
                        ptr2->TexCoords.X = tileX + 8U;
                        ptr2->TexCoords.Y = tileY + 8U;
                        ptr2[1].TexCoords.X = tileX;
                        ptr2[1].TexCoords.Y = tileY + 8U;
                        ptr2[2].TexCoords.X = tileX;
                        ptr2[2].TexCoords.Y = tileY;
                        ptr2[3].TexCoords.X = tileX + 8U;
                        ptr2[3].TexCoords.Y = tileY;
                    }
                    v.X = Math.Min(v.X, ptr2->Position.X);
                    v.Y = Math.Min(v.Y, ptr2->Position.Y);
                    v2.X = Math.Max(v2.X, ptr2[2].Position.X - v.X);
                    v2.Y = Math.Max(v2.Y, ptr2[2].Position.Y - v.Y);
                    this.AddVertIndex(tile, i * 4);
                }
            }
            this.size = v2 - v;
        }

        private unsafe void UpdateAnimations()
        {
            if (!this.animationEnabled)
            {
                return;
            }
            for (int i = 0; i < this.animations.Length; i++)
            {
                TileGroup.TileAnimation tileAnimation = this.animations[i];
                float speed = Engine.Frame * tileAnimation.Speed;
                uint tileID = (uint)tileAnimation.Tiles[(int)speed % tileAnimation.Tiles.Length];
                this.IDToTexCoords(tileID - 1U, out uint tileX, out uint tileY);
                fixed (Vertex* ptr = this.vertices)
                {
                    for (int j = 0; j < tileAnimation.VertIndexes.Count; j++)
                    {
                        int vertexIndex = tileAnimation.VertIndexes[j];
                        Vertex* ptr2 = ptr + vertexIndex;
                        ptr2->TexCoords.X = tileX;
                        ptr2->TexCoords.Y = tileY;
                        ptr2[1].TexCoords.X = tileX + 8U;
                        ptr2[1].TexCoords.Y = tileY;
                        ptr2[2].TexCoords.X = tileX + 8U;
                        ptr2[2].TexCoords.Y = tileY + 8U;
                        ptr2[3].TexCoords.X = tileX;
                        ptr2[3].TexCoords.Y = tileY + 8U;
                    }
                }
            }
        }

        public override void Draw(RenderTarget target)
        {
            TileGroup.TILE_GROUP_SHADER.SetUniform("image", this.tileset.Image);
            TileGroup.TILE_GROUP_SHADER.SetUniform("palette", this.tileset.Palette);
            TileGroup.TILE_GROUP_SHADER.SetUniform("palIndex", this.tileset.CurrentPaletteFloat);
            TileGroup.TILE_GROUP_SHADER.SetUniform("palSize", this.tileset.PaletteSize);
            TileGroup.TILE_GROUP_SHADER.SetUniform("blend", new SFML.Graphics.Glsl.Vec4(Color.White));
            TileGroup.TILE_GROUP_SHADER.SetUniform("blendMode", 1f);
            this.UpdateAnimations();
            target.Draw(this.vertices, PrimitiveType.Quads, this.renderState);
        }

        protected override void Dispose(bool disposing)
        {
            if (!this.disposed)
            {

                // Unuse tileset
                TextureManager.Instance.Unuse(this.tileset);

                // Additionally, dispose of the tileset and set it to null.
                // Just for safety :^)
                tileset.Dispose();
                tileset = null;

                // These would stay in memory and just shit up MEMORY AND SHIT UP PERFORMANCE
                Array.Clear(vertices, 0, vertices.Length);
                vertices = null;

                // For safety, we're going to clear the animations array.
                Array.Clear(animations, 0, animations.Length);
                animations = null;
            }
            this.disposed = true;
        }

        private static readonly Shader TILE_GROUP_SHADER = new Shader(EmbeddedResources.GetStream("Violet.Resources.pal.vert"), null, EmbeddedResources.GetStream("Violet.Resources.pal.frag"));


    }
}
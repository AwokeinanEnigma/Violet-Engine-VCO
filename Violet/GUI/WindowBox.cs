using SFML.Graphics;
using SFML.System;
using Violet.Graphics;
using Violet.Utility;

namespace Violet.GUI
{
    public class WindowBox : Renderable
    {
        #region Properties
        public override Vector2f Position
        {
            get
            {
                return this.position;
            }
            set
            {
                this.position = value;
                this.ConfigureTransform();
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
                this.ConfigureQuads();
            }
        }

        public override Vector2f Size
        {
            get
            {
                return this.size;
            }
            set
            {
                this.size = value;
                this.ConfigureQuads();
            }
        }

        public uint Palette
        {
            get
            {
                return this.palette;
            }
            set
            {
                this.palette = value;
            }
        }

        public WindowStyle Style
        {
            get
            {
                return this.windowStyle;
            }
            set
            {
                this.windowStyle = value;
                SetStyle(value);
            }
        }
        #endregion

        #region Private fields
        private uint palette;
        private IndexedColorGraphic frame;
        private RenderStates states;
        private Transform transform;
        private VertexArray verts;
        private Shader shader;
        private WindowStyle windowStyle;
        public struct WindowStyle
        {
            public WindowStyle(string resourcePath, bool beamRepeat) {
                this.resourcePath = resourcePath;
                this.beamRepeat = beamRepeat;
            }
            public string resourcePath;
            public bool beamRepeat;
        }
        #endregion
        public WindowBox(WindowStyle style, uint palette, Vector2f position, Vector2f size, int depth)
        {
            this.windowStyle = style;
            this.palette = palette;
            this.position = position;
            this.size = size;
            this.depth = depth;
            this.SetStyle(style);
        }

        private void SetStyle(WindowStyle newStyle)
        {
            if (string.IsNullOrEmpty(newStyle.resourcePath))
            {
                Debug.LogW("Tried to apply a WindowStyle to WindowBox that had no ResourcePath!");

                // if the style has no resourcePath :^) ( enigma note: this was really funny before i revamped this section of the code )
                newStyle.resourcePath = "Data/Graphics/window1.dat";

                this.windowStyle = newStyle;
            }

            this.frame = new IndexedColorGraphic(windowStyle.resourcePath, "center", this.position, this.depth);
            this.frame.CurrentPalette = this.palette;

            ((IndexedTexture)this.frame.Texture).CurrentPalette = this.palette;

            this.shader = new Shader(EmbeddedResources.GetStream("Violet.Resources.pal.vert"), null, EmbeddedResources.GetStream("Violet.Resources.pal.frag"));
            this.shader.SetUniform("image", this.frame.Texture.Image);
            this.shader.SetUniform("palette", ((IndexedTexture)this.frame.Texture).Palette);
            this.shader.SetUniform("palIndex", ((IndexedTexture)this.frame.Texture).CurrentPaletteFloat);
            this.shader.SetUniform("palSize", ((IndexedTexture)this.frame.Texture).PaletteSize);
            this.shader.SetUniform("blend", new SFML.Graphics.Glsl.Vec4(Color.White));
            this.shader.SetUniform("blendMode", 1f);

            this.states = new RenderStates(BlendMode.Alpha, this.transform, this.frame.Texture.Image, this.shader);
            this.verts = new VertexArray(PrimitiveType.Quads);

            this.ConfigureQuads();
            this.ConfigureTransform();

        }

        private void ConfigureTransform()
        {
            this.transform = new Transform(1f, 0f, this.position.X, 0f, 1f, this.position.Y, 0f, 0f, 1f);
            this.states.Transform = this.transform;
        }

        private void ConfigureQuads()
        {
            // have fun fuckos, because i am NOT touching this code!
            SpriteDefinition spriteDefinition = this.frame.GetSpriteDefinition("topleft");
            SpriteDefinition spriteDefinition2 = this.frame.GetSpriteDefinition("topright");
            SpriteDefinition spriteDefinition3 = this.frame.GetSpriteDefinition("bottomleft");
            SpriteDefinition spriteDefinition4 = this.frame.GetSpriteDefinition("bottomright");
            SpriteDefinition spriteDefinition5 = this.frame.GetSpriteDefinition("top");
            SpriteDefinition spriteDefinition6 = this.frame.GetSpriteDefinition("bottom");
            SpriteDefinition spriteDefinition7 = this.frame.GetSpriteDefinition("left");
            SpriteDefinition spriteDefinition8 = this.frame.GetSpriteDefinition("right");
            SpriteDefinition spriteDefinition9 = this.frame.GetSpriteDefinition("center");
            Vector2i bounds = spriteDefinition.Bounds;
            Vector2i bounds2 = spriteDefinition5.Bounds;
            Vector2i bounds3 = spriteDefinition2.Bounds;
            Vector2i bounds4 = spriteDefinition8.Bounds;
            Vector2i bounds5 = spriteDefinition4.Bounds;
            Vector2i bounds6 = spriteDefinition6.Bounds;
            Vector2i bounds7 = spriteDefinition3.Bounds;
            Vector2i bounds8 = spriteDefinition7.Bounds;
            Vector2i bounds9 = spriteDefinition9.Bounds;
            int num = (int)this.Size.X - bounds.X - bounds3.X;
            int num2 = (int)this.Size.X - bounds7.X - bounds5.X;
            int num3 = (int)this.Size.Y - bounds3.Y - bounds5.Y;
            int num4 = (int)this.Size.Y - bounds.Y - bounds7.Y;
            this.verts.Clear();
            Vector2f vector2f = default(Vector2f);
            Vector2f vector2f2 = vector2f;
            this.verts.Append(new Vertex(vector2f2, new Vector2f(spriteDefinition.Coords.X, spriteDefinition.Coords.Y)));
            this.verts.Append(new Vertex(vector2f2 + new Vector2f(bounds.X, 0f), new Vector2f(spriteDefinition.Coords.X + bounds.X, spriteDefinition.Coords.Y)));
            this.verts.Append(new Vertex(vector2f2 + new Vector2f(bounds.X, bounds.Y), new Vector2f(spriteDefinition.Coords.X + bounds.X, spriteDefinition.Coords.Y + bounds.Y)));
            this.verts.Append(new Vertex(vector2f2 + new Vector2f(0f, bounds.Y), new Vector2f(spriteDefinition.Coords.X, spriteDefinition.Coords.Y + bounds.Y)));
            vector2f2 += new Vector2f(num + bounds.X, 0f);
            this.verts.Append(new Vertex(vector2f2, new Vector2f(spriteDefinition2.Coords.X, spriteDefinition2.Coords.Y)));
            this.verts.Append(new Vertex(vector2f2 + new Vector2f(bounds3.X, 0f), new Vector2f(spriteDefinition2.Coords.X + bounds3.X, spriteDefinition2.Coords.Y)));
            this.verts.Append(new Vertex(vector2f2 + new Vector2f(bounds3.X, bounds3.Y), new Vector2f(spriteDefinition2.Coords.X + bounds3.X, spriteDefinition2.Coords.Y + bounds3.Y)));
            this.verts.Append(new Vertex(vector2f2 + new Vector2f(0f, bounds3.Y), new Vector2f(spriteDefinition2.Coords.X, spriteDefinition2.Coords.Y + bounds3.Y)));
            vector2f2 += new Vector2f(0f, num3 + bounds3.Y);
            this.verts.Append(new Vertex(vector2f2, new Vector2f(spriteDefinition4.Coords.X, spriteDefinition4.Coords.Y)));
            this.verts.Append(new Vertex(vector2f2 + new Vector2f(bounds5.X, 0f), new Vector2f(spriteDefinition4.Coords.X + bounds5.X, spriteDefinition4.Coords.Y)));
            this.verts.Append(new Vertex(vector2f2 + new Vector2f(bounds5.X, bounds5.Y), new Vector2f(spriteDefinition4.Coords.X + bounds5.X, spriteDefinition4.Coords.Y + bounds5.Y)));
            this.verts.Append(new Vertex(vector2f2 + new Vector2f(0f, bounds5.Y), new Vector2f(spriteDefinition4.Coords.X, spriteDefinition4.Coords.Y + bounds5.Y)));
            vector2f2 -= new Vector2f(num2 + bounds7.X, 0f);
            this.verts.Append(new Vertex(vector2f2, new Vector2f(spriteDefinition3.Coords.X, spriteDefinition3.Coords.Y)));
            this.verts.Append(new Vertex(vector2f2 + new Vector2f(bounds7.X, 0f), new Vector2f(spriteDefinition3.Coords.X + bounds7.X, spriteDefinition3.Coords.Y)));
            this.verts.Append(new Vertex(vector2f2 + new Vector2f(bounds7.X, bounds7.Y), new Vector2f(spriteDefinition3.Coords.X + bounds7.X, spriteDefinition3.Coords.Y + bounds7.Y)));
            this.verts.Append(new Vertex(vector2f2 + new Vector2f(0f, bounds7.Y), new Vector2f(spriteDefinition3.Coords.X, spriteDefinition3.Coords.Y + bounds7.Y)));
            vector2f2 += new Vector2f(bounds8.X, -num4);
            this.verts.Append(new Vertex(vector2f2, new Vector2f(spriteDefinition9.Coords.X, spriteDefinition9.Coords.Y)));
            this.verts.Append(new Vertex(vector2f2 + new Vector2f(num, 0f), new Vector2f(spriteDefinition9.Coords.X + bounds9.X, spriteDefinition9.Coords.Y)));
            this.verts.Append(new Vertex(vector2f2 + new Vector2f(num, num4), new Vector2f(spriteDefinition9.Coords.X + bounds9.X, spriteDefinition9.Coords.Y + bounds9.Y)));
            this.verts.Append(new Vertex(vector2f2 + new Vector2f(0f, num4), new Vector2f(spriteDefinition9.Coords.X, spriteDefinition9.Coords.Y + bounds9.Y)));
            if (!windowStyle.beamRepeat)
            {
                vector2f2 = vector2f;
                vector2f2 += new Vector2f(bounds.X, 0f);
                this.verts.Append(new Vertex(vector2f2, new Vector2f(spriteDefinition5.Coords.X, spriteDefinition5.Coords.Y)));
                this.verts.Append(new Vertex(vector2f2 + new Vector2f(num, 0f), new Vector2f(spriteDefinition5.Coords.X + bounds2.X, spriteDefinition5.Coords.Y)));
                this.verts.Append(new Vertex(vector2f2 + new Vector2f(num, bounds2.Y), new Vector2f(spriteDefinition5.Coords.X + bounds2.X, spriteDefinition5.Coords.Y + bounds2.Y)));
                this.verts.Append(new Vertex(vector2f2 + new Vector2f(0f, bounds2.Y), new Vector2f(spriteDefinition5.Coords.X, spriteDefinition5.Coords.Y + bounds2.Y)));
                vector2f2 = vector2f;
                vector2f2 += new Vector2f(bounds.X + num, bounds3.Y);
                this.verts.Append(new Vertex(vector2f2, new Vector2f(spriteDefinition8.Coords.X, spriteDefinition8.Coords.Y)));
                this.verts.Append(new Vertex(vector2f2 + new Vector2f(bounds4.X, 0f), new Vector2f(spriteDefinition8.Coords.X + bounds4.X, spriteDefinition8.Coords.Y)));
                this.verts.Append(new Vertex(vector2f2 + new Vector2f(bounds3.X, num3), new Vector2f(spriteDefinition8.Coords.X + bounds4.X, spriteDefinition8.Coords.Y + bounds4.Y)));
                this.verts.Append(new Vertex(vector2f2 + new Vector2f(0f, num3), new Vector2f(spriteDefinition8.Coords.X, spriteDefinition8.Coords.Y + bounds4.Y)));
                vector2f2 = vector2f;
                vector2f2 += new Vector2f(bounds7.X, bounds.Y + num4);
                this.verts.Append(new Vertex(vector2f2, new Vector2f(spriteDefinition6.Coords.X, spriteDefinition6.Coords.Y)));
                this.verts.Append(new Vertex(vector2f2 + new Vector2f(num2, 0f), new Vector2f(spriteDefinition6.Coords.X + bounds6.X, spriteDefinition6.Coords.Y)));
                this.verts.Append(new Vertex(vector2f2 + new Vector2f(num2, bounds6.Y), new Vector2f(spriteDefinition6.Coords.X + bounds6.X, spriteDefinition6.Coords.Y + bounds6.Y)));
                this.verts.Append(new Vertex(vector2f2 + new Vector2f(0f, bounds6.Y), new Vector2f(spriteDefinition6.Coords.X, spriteDefinition6.Coords.Y + bounds6.Y)));
                vector2f2 = vector2f;
                vector2f2 += new Vector2f(0f, bounds.Y);
                this.verts.Append(new Vertex(vector2f2, new Vector2f(spriteDefinition7.Coords.X, spriteDefinition7.Coords.Y)));
                this.verts.Append(new Vertex(vector2f2 + new Vector2f(bounds8.X, 0f), new Vector2f(spriteDefinition7.Coords.X + bounds8.X, spriteDefinition7.Coords.Y)));
                this.verts.Append(new Vertex(vector2f2 + new Vector2f(bounds8.X, num4), new Vector2f(spriteDefinition7.Coords.X + bounds8.X, spriteDefinition7.Coords.Y + bounds8.Y)));
                this.verts.Append(new Vertex(vector2f2 + new Vector2f(0f, num4), new Vector2f(spriteDefinition7.Coords.X, spriteDefinition7.Coords.Y + bounds8.Y)));
                return;
            }
            int num5 = num / bounds2.X;
            int num6 = num % bounds2.X;
            vector2f2 = vector2f + new Vector2f(bounds.X, 0f);
            for (int i = 0; i < num5; i++)
            {
                this.verts.Append(new Vertex(vector2f2, new Vector2f(spriteDefinition5.Coords.X, spriteDefinition5.Coords.Y)));
                this.verts.Append(new Vertex(vector2f2 + new Vector2f(bounds2.X, 0f), new Vector2f(spriteDefinition5.Coords.X + bounds2.X, spriteDefinition5.Coords.Y)));
                this.verts.Append(new Vertex(vector2f2 + new Vector2f(bounds2.X, bounds2.Y), new Vector2f(spriteDefinition5.Coords.X + bounds2.X, spriteDefinition5.Coords.Y + bounds2.Y)));
                this.verts.Append(new Vertex(vector2f2 + new Vector2f(0f, bounds2.Y), new Vector2f(spriteDefinition5.Coords.X, spriteDefinition5.Coords.Y + bounds2.Y)));
                vector2f2 += new Vector2f(bounds2.X, 0f);
            }
            if (num6 != 0)
            {
                this.verts.Append(new Vertex(vector2f2, new Vector2f(spriteDefinition5.Coords.X, spriteDefinition5.Coords.Y)));
                this.verts.Append(new Vertex(vector2f2 + new Vector2f(num6, 0f), new Vector2f(spriteDefinition5.Coords.X + num6, spriteDefinition5.Coords.Y)));
                this.verts.Append(new Vertex(vector2f2 + new Vector2f(num6, bounds2.Y), new Vector2f(spriteDefinition5.Coords.X + num6, spriteDefinition5.Coords.Y + bounds2.Y)));
                this.verts.Append(new Vertex(vector2f2 + new Vector2f(0f, bounds2.Y), new Vector2f(spriteDefinition5.Coords.X, spriteDefinition5.Coords.Y + bounds2.Y)));
            }
            int num7 = num2 / bounds6.X;
            int num8 = num2 % bounds6.X;
            vector2f2 = vector2f + new Vector2f(bounds.X, bounds.Y + num4);
            for (int j = 0; j < num7; j++)
            {
                this.verts.Append(new Vertex(vector2f2, new Vector2f(spriteDefinition6.Coords.X, spriteDefinition6.Coords.Y)));
                this.verts.Append(new Vertex(vector2f2 + new Vector2f(bounds6.X, 0f), new Vector2f(spriteDefinition6.Coords.X + bounds6.X, spriteDefinition6.Coords.Y)));
                this.verts.Append(new Vertex(vector2f2 + new Vector2f(bounds6.X, bounds6.Y), new Vector2f(spriteDefinition6.Coords.X + bounds6.X, spriteDefinition6.Coords.Y + bounds6.Y)));
                this.verts.Append(new Vertex(vector2f2 + new Vector2f(0f, bounds6.Y), new Vector2f(spriteDefinition6.Coords.X, spriteDefinition6.Coords.Y + bounds6.Y)));
                vector2f2 += new Vector2f(bounds6.X, 0f);
            }
            if (num8 != 0)
            {
                this.verts.Append(new Vertex(vector2f2, new Vector2f(spriteDefinition6.Coords.X, spriteDefinition6.Coords.Y)));
                this.verts.Append(new Vertex(vector2f2 + new Vector2f(num8, 0f), new Vector2f(spriteDefinition6.Coords.X + num8, spriteDefinition6.Coords.Y)));
                this.verts.Append(new Vertex(vector2f2 + new Vector2f(num8, bounds6.Y), new Vector2f(spriteDefinition6.Coords.X + num8, spriteDefinition6.Coords.Y + bounds6.Y)));
                this.verts.Append(new Vertex(vector2f2 + new Vector2f(0f, bounds6.Y), new Vector2f(spriteDefinition6.Coords.X, spriteDefinition6.Coords.Y + bounds6.Y)));
            }
            int num9 = num4 / bounds8.Y;
            int num10 = num4 % bounds8.Y;
            vector2f2 = vector2f + new Vector2f(0f, bounds.Y);
            for (int k = 0; k < num9; k++)
            {
                this.verts.Append(new Vertex(vector2f2, new Vector2f(spriteDefinition7.Coords.X, spriteDefinition7.Coords.Y)));
                this.verts.Append(new Vertex(vector2f2 + new Vector2f(bounds8.X, 0f), new Vector2f(spriteDefinition7.Coords.X + bounds8.X, spriteDefinition7.Coords.Y)));
                this.verts.Append(new Vertex(vector2f2 + new Vector2f(bounds8.X, bounds8.Y), new Vector2f(spriteDefinition7.Coords.X + bounds8.X, spriteDefinition7.Coords.Y + bounds8.Y)));
                this.verts.Append(new Vertex(vector2f2 + new Vector2f(0f, bounds8.Y), new Vector2f(spriteDefinition7.Coords.X, spriteDefinition7.Coords.Y + bounds8.Y)));
                vector2f2 += new Vector2f(0f, bounds8.Y);
            }
            if (num10 != 0)
            {
                this.verts.Append(new Vertex(vector2f2, new Vector2f(spriteDefinition7.Coords.X, spriteDefinition7.Coords.Y)));
                this.verts.Append(new Vertex(vector2f2 + new Vector2f(bounds8.X, 0f), new Vector2f(spriteDefinition7.Coords.X + bounds8.X, spriteDefinition7.Coords.Y)));
                this.verts.Append(new Vertex(vector2f2 + new Vector2f(bounds8.X, num10), new Vector2f(spriteDefinition7.Coords.X + bounds8.X, spriteDefinition7.Coords.Y + num10)));
                this.verts.Append(new Vertex(vector2f2 + new Vector2f(0f, num10), new Vector2f(spriteDefinition7.Coords.X, spriteDefinition7.Coords.Y + num10)));
            }
            int num11 = num3 / bounds4.Y;
            int num12 = num3 % bounds4.Y;
            vector2f2 = vector2f + new Vector2f(bounds.X + num, bounds.Y);
            for (int l = 0; l < num11; l++)
            {
                this.verts.Append(new Vertex(vector2f2, new Vector2f(spriteDefinition8.Coords.X, spriteDefinition8.Coords.Y)));
                this.verts.Append(new Vertex(vector2f2 + new Vector2f(bounds4.X, 0f), new Vector2f(spriteDefinition8.Coords.X + bounds4.X, spriteDefinition8.Coords.Y)));
                this.verts.Append(new Vertex(vector2f2 + new Vector2f(bounds4.X, bounds4.Y), new Vector2f(spriteDefinition8.Coords.X + bounds4.X, spriteDefinition8.Coords.Y + bounds4.Y)));
                this.verts.Append(new Vertex(vector2f2 + new Vector2f(0f, bounds4.Y), new Vector2f(spriteDefinition8.Coords.X, spriteDefinition8.Coords.Y + bounds4.Y)));
                vector2f2 += new Vector2f(0f, bounds4.Y);
            }
            if (num12 != 0)
            {
                this.verts.Append(new Vertex(vector2f2, new Vector2f(spriteDefinition8.Coords.X, spriteDefinition8.Coords.Y)));
                this.verts.Append(new Vertex(vector2f2 + new Vector2f(bounds4.X, 0f), new Vector2f(spriteDefinition8.Coords.X + bounds4.X, spriteDefinition8.Coords.Y)));
                this.verts.Append(new Vertex(vector2f2 + new Vector2f(bounds4.X, num12), new Vector2f(spriteDefinition8.Coords.X + bounds4.X, spriteDefinition8.Coords.Y + num12)));
                this.verts.Append(new Vertex(vector2f2 + new Vector2f(0f, num12), new Vector2f(spriteDefinition8.Coords.X, spriteDefinition8.Coords.Y + num12)));
            }
        }

        public override void Draw(RenderTarget target)
        {
            target.Draw(this.verts, this.states);
        }

        protected override void Dispose(bool disposing)
        {
            if (!this.disposed && disposing)
            {
                this.frame.Dispose();
            }
            this.disposed = true;
        }
    }
}
using SFML.Graphics;
using SFML.System;
using Violet.Graphics;
using Violet.GUI;

namespace VCO.GUI
{
    internal class Nametag : Renderable
    {
        public override Vector2f Position
        {
            get => this.position;
            set => this.Reposition(value);
        }

        public string Name
        {
            get => this.nameText.Text;
            set => this.SetName(value);
        }

        public Nametag(string nameString, Vector2f position, int depth)
        {
            this.position = position;
            this.depth = depth;
            this.nameText = new TextRegion(this.position + Nametag.TEXT_POSITION, this.depth + 1, Fonts.Main, nameString);
            this.left = new IndexedColorGraphic(DataHandler.instance.Load("nametag.dat"), "left", this.position, this.depth);
            this.center = new IndexedColorGraphic(DataHandler.instance.Load("nametag.dat"), "center", this.left.Position + new Vector2f(this.left.Size.X, 0f), this.depth)
            {
                Scale = new Vector2f(this.nameText.Size.X + 2f, 1f)
            };
            this.right = new IndexedColorGraphic(DataHandler.instance.Load("nametag.dat"), "right", this.center.Position + new Vector2f(this.nameText.Size.X + 2f, 0f), this.depth);
            this.nameText.Color = Color.Black;
            this.CalculateSize();
        }

        private void Reposition(Vector2f newPosition)
        {
            this.position = newPosition;
            this.nameText.Position = this.position + Nametag.TEXT_POSITION;
            this.left.Position = this.position;
            this.center.Position = this.left.Position + new Vector2f(this.left.Size.X, 0f);
            this.right.Position = this.center.Position + new Vector2f(this.nameText.Size.X + 2f, 0f);
        }

        private void SetName(string newName)
        {
            this.nameText.Reset(newName, 0, newName.Length);
            this.center.Scale = new Vector2f(this.nameText.Size.X + 2f, 1f);
            this.right.Position = this.center.Position + new Vector2f(this.nameText.Size.X + 2f, 0f);
            this.CalculateSize();
        }

        private void CalculateSize()
        {
            this.size = new Vector2f(this.left.Size.X + this.nameText.Size.X + 2f + this.right.Size.X, this.left.Size.Y);
        }

        public override void Draw(RenderTarget target)
        {
            this.left.Draw(target);
            this.center.Draw(target);
            this.right.Draw(target);
            this.nameText.Draw(target);
        }

        protected override void Dispose(bool disposing)
        {
            if (!this.disposed && disposing)
            {
                this.left.Dispose();
                this.center.Dispose();
                this.right.Dispose();
                this.nameText.Dispose();
            }
            base.Dispose(disposing);
        }

        private const string LEFT_SPRITE_NAME = "left";

        private const string CENTER_SPRITE_NAME = "center";

        private const string RIGHT_SPRITE_NAME = "right";

        private const int MARGIN = 2;

        private static readonly Vector2f TEXT_POSITION = new Vector2f(5f, 1f);

        private readonly IndexedColorGraphic left;

        private readonly IndexedColorGraphic center;

        private readonly IndexedColorGraphic right;

        private readonly TextRegion nameText;
    }
}

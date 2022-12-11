using SFML.Graphics;
using SFML.System;
using Violet.Graphics;
using Violet.Utility;

namespace VCO.GUI.Text.Printables
{
    internal class GraphicPrintable : Printable
	{
		public override Vector2f Position
		{
			get
			{
				return this.graphic.Position;
			}
			set
			{
				this.graphic.Position = value;
			}
		}

		public override Vector2f Origin
		{
			get
			{
				return this.graphic.Origin;
			}
			set
			{
				this.graphic.Origin = value;
			}
		}

		public override Vector2f Size
		{
			get
			{
				return this.graphic.Size;
			}
		}

		public GraphicPrintable(string subsprite)
		{
			this.graphic = new IndexedColorGraphic(DataHandler.instance.Load( "emote.dat"), subsprite, VectorMath.ZERO_VECTOR, 0);
		}

		public override void Update()
		{
			this.isDone = true;
		}

		public override void Draw(RenderTarget target)
		{
			this.graphic.Draw(target);
		}

		protected override void Dispose(bool disposing)
		{
			if (!this.disposed && disposing)
			{
				this.graphic.Dispose();
			}
			base.Dispose(disposing);
		}

		private IndexedColorGraphic graphic;
	}
}

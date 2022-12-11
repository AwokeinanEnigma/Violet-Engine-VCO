using Violet.Graphics;

namespace VCO.GUI.Text.Printables
{
    internal abstract class Printable : Renderable
	{
		public bool Complete
		{
			get
			{
				return this.isDone;
			}
		}

		public bool Removable
		{
			get
			{
				return this.isRemovable;
			}
		}

		public abstract void Update();

		protected bool isDone;

		protected bool isRemovable;
	}
}

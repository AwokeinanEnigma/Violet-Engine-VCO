using Violet.Graphics;

namespace VCO.GUI.Text.Printables
{
    internal abstract class Printable : Renderable
	{
		protected bool isDone;
		public bool Complete
		{
			get => this.isDone;
			 
		}

		protected bool isRemovable;
		public bool Removable
		{
			get => this.isRemovable;
			
		}

		public abstract void Update();


	}
}

using System;

namespace SunsetRhapsody.Items
{
	internal class ItemPropertyNotFoundException : ApplicationException
	{
		public ItemPropertyNotFoundException(string name) : base(string.Format("\"{0}\" was not found in the item's properties.", name))
		{
		}
	}
}

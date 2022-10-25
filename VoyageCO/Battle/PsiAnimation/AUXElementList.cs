using System;
using System.Collections.Generic;

namespace VCO.Battle.AUXAnimation
{
	internal class AUXElementList
	{
		public bool HasElements
		{
			get
			{
				return this.elementCounter < this.elements.Count;
			}
		}

		public AUXElementList(List<AUXElement> elements)
		{
			this.elements = new List<AUXElement>(elements);
			this.elements.Sort(new Comparison<AUXElement>(AUXElementList.CompareElements));
		}

		private static int CompareElements(AUXElement x, AUXElement y)
		{
			return y.Timestamp - x.Timestamp;
		}

		public List<AUXElement> GetElementsAtTime(int timestamp)
		{
			List<AUXElement> list = this.elements.FindAll((AUXElement x) => x.Timestamp == timestamp);
			this.elementCounter += list.Count;
			return list;
		}

		private List<AUXElement> elements;

		private int elementCounter;
	}
}

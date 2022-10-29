using VCO.SOMETHING;
using System;

namespace VCO.AUX
{
	public struct DefensiveAUX : IAUX
	{
		public DefensiveAUX(OffenseAUX ability)
		{
			_aux = ability.aux;
		}


		private AUXBase _aux;
		public AUXBase aux
		{
			get { return _aux; }
			set
			{
				_aux = aux;
			}
		}
	}
}
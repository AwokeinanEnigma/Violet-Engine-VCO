using SunsetRhapsody.SOMETHING;
using System;

namespace SunsetRhapsody.AUX
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
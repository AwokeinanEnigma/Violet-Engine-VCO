using SunsetRhapsody.SOMETHING;
using System;

namespace SunsetRhapsody.AUX
{
	public struct OffenseAUX : IAUX
	{
		public OffenseAUX(AUXBase baseo) {
			_aux = baseo;
		}
		public OffenseAUX(OffenseAUX ability)
		{
			Console.WriteLine("hey");
			_aux = ability.aux;
		}


		private AUXBase _aux;
		public AUXBase aux
		{
			get { return _aux; }
			set
			{
				_aux = value;
			}
		}
	}
}
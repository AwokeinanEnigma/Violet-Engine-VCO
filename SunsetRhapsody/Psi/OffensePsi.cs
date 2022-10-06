using SunsetRhapsody.SOMETHING;
using System;

namespace SunsetRhapsody.Psi
{
	public struct OffensePsi : IPsi
	{
		public OffensePsi(AUXBase baseo) {
			_aux = baseo;
		}
		public OffensePsi(OffensePsi ability)
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
using SunsetRhapsody.SOMETHING;
using System;

namespace SunsetRhapsody.Psi
{
	public struct DefensivePsi : IPsi
	{
		public DefensivePsi(OffensePsi ability)
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
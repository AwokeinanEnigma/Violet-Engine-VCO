using SunsetRhapsody.SOMETHING;
using System;

namespace SunsetRhapsody.Psi
{
	public struct OtherPsi : IPsi
	{

		public OtherPsi(OffensePsi ability)
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
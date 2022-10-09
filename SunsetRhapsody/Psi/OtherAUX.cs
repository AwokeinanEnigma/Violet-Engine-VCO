using SunsetRhapsody.SOMETHING;
using System;

namespace SunsetRhapsody.AUX
{
	public struct OtherAUX : IAUX
	{

		public OtherAUX(AUXBase ability)
		{
			_aux = ability ;
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
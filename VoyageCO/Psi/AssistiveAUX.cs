﻿using VCO.SOMETHING;
using System;

namespace VCO.AUX
{
	public struct AssistiveAUX : IAUX
	{
		public AssistiveAUX(AUXBase ability)
		{
			_aux = ability;
		}
		public AssistiveAUX(AssistiveAUX ability)
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
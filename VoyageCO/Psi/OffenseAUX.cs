﻿using System;
using VCO.SOMETHING;

namespace VCO.AUX
{
    public struct OffenseAUX : IAUX
    {
        public OffenseAUX(AUXBase baseo)
        {
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
            get => _aux;
            set => _aux = value;
        }
    }
}
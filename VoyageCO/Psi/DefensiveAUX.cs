using VCO.SOMETHING;

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
            get => _aux;
            set => _aux = aux;
        }
    }
}
using VCO.SOMETHING;

namespace VCO.AUX
{
    public struct OtherAUX : IAUX
    {

        public OtherAUX(AUXBase ability)
        {
            _aux = ability;
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
namespace VCO.Scripts.Actions.ParamTypes
{
    internal struct RufiniOption
    {
        public override string ToString()
        {
            return string.Format("{0}", this.Option);
        }

        public int Option;
    }
}

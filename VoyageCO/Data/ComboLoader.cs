using System.Collections.Generic;
using VCO.Battle.Combos;

namespace VCO.Data
{
    internal class ComboLoader
    {
        public static ComboSet Load(string resource)
        {
            return new ComboSet(new List<ComboNode>
            {
                new ComboNode(ComboType.Point, 0U, 1U),
                new ComboNode(ComboType.Point, 375U, 1U),
                new ComboNode(ComboType.Point, 750U, 1U),
                new ComboNode(ComboType.Point, 1125U, 1U),
                new ComboNode(ComboType.Point, 1500U, 1U),
                new ComboNode(ComboType.Point, 1875U, 1U),
                new ComboNode(ComboType.BPMRange, 2500U, 97500U, 120f)
            });
        }
    }
}

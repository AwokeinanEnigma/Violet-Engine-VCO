using System.Collections.Generic;

namespace VCO.Battle.Combos
{
    internal class ComboSet
    {
        public IList<ComboNode> ComboNodes => this.combos;

        public ComboSet(IList<ComboNode> nodes)
        {
            this.combos = new List<ComboNode>(nodes);
            this.combos.Sort();
        }

        public ComboNode GetFirstCombo()
        {
            if (this.combos.Count <= 0)
            {
                return null;
            }
            return this.combos[0];
        }

        public ComboNode GetNextCombo(ComboNode node)
        {
            if (node != null)
            {
                ComboNode comboNode = null;
                foreach (ComboNode comboNode2 in this.combos)
                {
                    if (comboNode2.Timestamp > node.Timestamp)
                    {
                        comboNode = comboNode2;
                        break;
                    }
                }
                if (comboNode == null)
                {
                    comboNode = this.GetFirstCombo();
                }
                return comboNode;
            }
            return this.GetFirstCombo();
        }

        private readonly List<ComboNode> combos;
    }
}

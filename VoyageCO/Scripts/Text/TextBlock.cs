using System.Collections.Generic;

namespace VCO.Scripts.Text
{
    internal class TextBlock
    {
        public List<TextLine> Lines
        {
            get
            {
                return this.lines;
            }
        }

        public TextBlock(List<TextLine> lines)
        {
            this.lines = new List<TextLine>(lines);
        }

        private List<TextLine> lines;
    }
}

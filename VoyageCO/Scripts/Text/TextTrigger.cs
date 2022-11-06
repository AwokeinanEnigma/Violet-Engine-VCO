namespace VCO.Scripts.Text
{
    internal class TextTrigger : ITextCommand
    {
        public int Position
        {
            get => this.position;
            set => this.position = value;
        }

        public int Type => this.type;

        public string[] Data => this.data;

        public TextTrigger(int position, int type, string[] data)
        {
            this.position = position;
            this.type = type;
            this.data = data;
        }

        private int position;

        private readonly int type;

        private readonly string[] data;
    }
}

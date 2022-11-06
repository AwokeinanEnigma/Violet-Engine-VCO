namespace VCO.Scripts.Text
{
    internal class TextPause : ITextCommand
    {
        public int Position
        {
            get => this.position;
            set => this.position = value;
        }

        public int Duration => this.duration;

        public TextPause(int position, int duration)
        {
            this.position = position;
            this.duration = duration;
        }

        private int position;

        private readonly int duration;
    }
}

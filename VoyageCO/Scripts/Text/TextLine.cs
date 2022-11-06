namespace VCO.Scripts.Text
{
    internal class TextLine
    {
        public string Text => this.text;

        public bool HasBullet => this.bullet;

        public ITextCommand[] Commands => this.commands;

        public TextLine(bool bullet, ITextCommand[] commands, string text)
        {
            this.bullet = bullet;
            this.commands = commands;
            this.text = text;
        }

        private readonly string text;

        private readonly bool bullet;

        private readonly ITextCommand[] commands;
    }
}

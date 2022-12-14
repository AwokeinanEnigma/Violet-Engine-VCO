using System;

namespace VCO.Scripts.Actions.ParamTypes
{
    internal struct RufiniString
    {
        public string QualifiedName => string.Join('.'.ToString(), this.nameParts);

        public string[] Names => this.nameParts;

        public string Value => this.value;

        public RufiniString(string qualifiedName, string value)
        {
            this.nameParts = qualifiedName.Split(new char[]
            {
                '.'
            });
            this.value = value;
        }

        public RufiniString(string[] nameParts, string value)
        {
            this.nameParts = new string[nameParts.Length];
            Array.Copy(nameParts, this.nameParts, nameParts.Length);
            this.value = value;
        }

        public override string ToString()
        {
            string text;
            if (this.Value != null)
            {
                text = (this.value ?? string.Empty).Replace("\n", "");
                if (text.Length > 50)
                {
                    int val = text.Substring(0, 50).LastIndexOf(' ');
                    int length = Math.Max(50, val);
                    text = text.Substring(0, length) + "…";
                }
            }
            else
            {
                text = this.QualifiedName;
            }
            return text;
        }

        public const char SEPARATOR = '.';

        private const int MAX_LENGTH = 50;

        private const string TRAIL = "…";

        private readonly string[] nameParts;

        private readonly string value;
    }
}

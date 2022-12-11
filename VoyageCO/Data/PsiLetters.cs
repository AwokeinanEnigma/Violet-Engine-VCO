using System.Collections.Generic;

namespace VCO.Data
{
    internal class AUXLetters
    {
        public static char Get(int level)
        {
            AUXLetters.letters.TryGetValue(level, out char result);
            return result;
        }
        private static readonly Dictionary<int, char> letters = new Dictionary<int, char>
        {
            {
                0,
                'α'
            },
            {
                1,
                'β'
            },
            {
                2,
                'γ'
            },
            {
                3,
                'Ω'
            }
        };
    }
}

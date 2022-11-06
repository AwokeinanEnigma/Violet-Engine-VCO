using SFML.Graphics;

namespace VCO.Data
{
    internal static class UIColors
    {
        public static Color HighlightColor => new Color(255, 89, 209);

        private static readonly Color[] highlightColors = new Color[]
        {
            new Color(66, 240, 15),
            new Color(142, 234, 172),
            new Color(240, 121, 145),
            new Color(182, 96, 10),
            new Color(201, 144, 111),
            new Color(101, 206, 237),
            new Color(106, 103, 199),
            new Color(209, 3, 10)
        };
    }
}

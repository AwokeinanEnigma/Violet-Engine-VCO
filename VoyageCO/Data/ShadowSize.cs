using SFML.System;

namespace VCO.Data
{
    internal static class ShadowSize
    {
        public static string GetSubsprite(Vector2f size)
        {
            return ShadowSize.GetSubsprite((int)size.X);
        }
        public static string GetSubsprite(float width)
        {
            return ShadowSize.GetSubsprite((int)width);
        }
        public static string GetSubsprite(int width)
        {
            string result;
            if (width <= 10)
            {
                result = "small";
            }
            else if (width <= 36)
            {
                result = "medium";
            }
            else if (width <= 64)
            {
                result = "large";
            }
            else
            {
                result = "huge";
            }
            return result;
        }
        private const string SMALL = "small";
        private const string MEDIUM = "medium";
        private const string LARGE = "large";
        private const string HUGE = "huge";
        private const int WIDTH_SMALL = 10;
        private const int WIDTH_MEDIUM = 36;
        private const int WIDTH_LARGE = 64;
    }
}
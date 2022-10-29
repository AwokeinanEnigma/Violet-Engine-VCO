using SFML.Graphics;

namespace Violet.Utility
{
    /// <summary>
    /// Helper class for colors
    /// </summary>
    public static class ColorHelper
    {
        /// <summary>
        /// Gets a color from an integer
        /// </summary>
        /// <param name="color">The integer to get the color from.</param>
        /// <returns>Returns the color from the integer</returns>
        public static Color FromInt(int color) => ColorHelper.FromInt((uint)color);

        /// <summary>
        /// Gets a color from an unsigned integer
        /// </summary>
        /// <param name="color">The unsigned integer to get the color from.</param>
        /// <returns>Returns the color from the unsigned integer</returns>
        public static Color FromInt(uint color)
        {
            // inherited from carbine
            // i don't know how this code works, and frankly, i don't want to know. 
            byte alpha = (byte)(color >> 24);
            return new Color((byte)(color >> 16), (byte)(color >> 8), (byte)color, alpha);
        }

        /// <summary>
        /// Blends two colors together
        /// </summary>
        /// <param name="col1"></param>
        /// <param name="col2"></param>
        /// <param name="amount">The amount to blend by. The higher the value, the less it'll blend. Vice versa. </param>
        /// <returns></returns>
        public static Color Blend(Color col1, Color col2, float amount)
        {
            float num = 1f - amount;
            return new Color((byte)(col1.R * (double)num + col2.R * (double)amount), (byte)(col1.G * (double)num + col2.G * (double)amount), (byte)(col1.B * (double)num + col2.B * (double)amount), byte.MaxValue);
        }

        public static Color BlendAlpha(Color col1, Color col2, float amount)
        {
            float num = 1f - amount;
            return new Color((byte)(col1.R * (double)num + col2.R * (double)amount), (byte)(col1.G * (double)num + col2.G * (double)amount), (byte)(col1.B * (double)num + col2.B * (double)amount), (byte)(col1.A * (double)num + col2.A * (double)amount));
        }

        public static Color Invert(this Color color) => new Color((byte)(byte.MaxValue - (uint)color.R), (byte)(byte.MaxValue - (uint)color.G), (byte)(byte.MaxValue - (uint)color.B), color.A);
    }
}

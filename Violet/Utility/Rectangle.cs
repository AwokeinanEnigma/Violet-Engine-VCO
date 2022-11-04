namespace Violet.Utility
{
    /// <summary>
    /// A generic class containing information for a triangle.
    /// </summary>
    internal class Rectangle
    {
        public float X { get; set; }

        public float Y { get; set; }

        public float Width { get; set; }

        public float Height { get; set; }

        /// <summary>
        /// Creates a new rectangle
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public Rectangle(float x, float y, float width, float height)
        { 
            this.X = x;
            this.Y = y;
            this.Width = width;
            this.Height = height;
        }
    }
}

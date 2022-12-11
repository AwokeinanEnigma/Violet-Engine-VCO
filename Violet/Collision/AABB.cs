using SFML.Graphics;
using SFML.System;

namespace Violet.Collision
{
    /// <summary>
    /// Struct for bounding boxes.
    /// You can read more about axis-aligned minimum bounding boxes here
    /// https://en.wikipedia.org/wiki/Minimum_bounding_box#Axis-aligned_minimum_bounding_box
    /// </summary>
    public struct AABB
    {
        private FloatRect floatRect;

        public readonly Vector2f Position;

        public readonly Vector2f Size;

        public readonly bool IsPlayer;

        public readonly bool OnlyPlayer;
        public AABB(Vector2f position, Vector2f size)
        {
            this.Position = position;
            this.Size = size;
            this.IsPlayer = false;
            this.OnlyPlayer = false;
            this.floatRect = new FloatRect(this.Position.X, this.Position.Y, this.Size.X, this.Size.Y);
        }

        public AABB(Vector2f position, Vector2f size, bool isPlayer, bool onlyPlayer)
        {
            this.Position = position;
            this.Size = size;
            this.IsPlayer = isPlayer;
            this.OnlyPlayer = onlyPlayer;
            this.floatRect = new FloatRect(this.Position.X, this.Position.Y, this.Size.X, this.Size.Y);
        }

        public FloatRect GetFloatRect()
        {
            return this.floatRect;
        }
    }
}

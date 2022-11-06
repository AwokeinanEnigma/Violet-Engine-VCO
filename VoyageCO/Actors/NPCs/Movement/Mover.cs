using SFML.System;
using System;

namespace VCO.Actors.NPCs.Movement
{
    internal abstract class Mover
    {
        protected void MovementStep(float steAUXze, ref Vector2f position, ref Vector2f target, ref Vector2f velocity)
        {
            velocity.X = Math.Sign(target.X - position.X) * Math.Min(steAUXze, Math.Abs(target.X - position.X));
            velocity.Y = Math.Sign(target.Y - position.Y) * Math.Min(steAUXze, Math.Abs(target.Y - position.Y));
        }

        public abstract bool GetNextMove(ref Vector2f position, ref Vector2f velocity, ref int direction);

        protected const float TOLERANCE = 1f;
    }
}

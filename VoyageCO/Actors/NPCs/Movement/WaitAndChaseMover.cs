using SFML.System;
using Violet.Graphics;
using Violet.Utility;

namespace VCO.Actors.NPCs.Movement
{
    internal class WaitAndChaseMover : Mover
    {
        public WaitAndChaseMover(float chaseTreshold, float speed)
        {
            this.chaseThreshold = chaseTreshold;
            this.speed = speed;
            this.oldDirection = -1;
        }

        public override bool GetNextMove(ref Vector2f position, ref Vector2f velocity, ref int direction)
        {
            this.oldDirection = direction;
            if (ViewManager.Instance.FollowActor != null)
            {
                float num = VectorMath.Magnitude(ViewManager.Instance.FollowActor.Position - position);
                this.mode = ((num < this.chaseThreshold) ? WaitAndChaseMover.Mode.Chase : WaitAndChaseMover.Mode.Wait);
            }
            if (this.mode == WaitAndChaseMover.Mode.Wait)
            {
                velocity = VectorMath.ZERO_VECTOR;
            }
            else if (this.mode == WaitAndChaseMover.Mode.Chase && ViewManager.Instance.FollowActor != null)
            {
                direction = VectorMath.VectorToDirection(ViewManager.Instance.FollowActor.Position - position);
                velocity = VectorMath.DirectionToVector(direction) * this.speed;
            }
            this.changed = (this.oldDirection != direction);
            return this.changed;
        }

        private readonly float chaseThreshold;

        private readonly float speed;

        private WaitAndChaseMover.Mode mode;

        private int oldDirection;

        private bool changed;

        private enum Mode
        {
            Wait,
            Chase
        }
    }
}

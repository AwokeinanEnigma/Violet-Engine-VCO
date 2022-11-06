using SFML.Graphics;
using SFML.System;
using System;
using Violet;

namespace VCO.Actors.NPCs.Movement
{
    internal class AreaMover : Mover
    {
        public AreaMover(float speed, int timeOut, float distance, float left, float top, float width, float height)
        {
            this.speed = speed;
            this.timeOut = 60;
            this.distance = distance;
            this.bounds = new FloatRect(left, top, width, height);
            this.Randomize();
        }

        private void Randomize()
        {
            this.target.X = this.bounds.Left + Engine.Random.Next((int)this.bounds.Width);
            this.target.Y = this.bounds.Top + Engine.Random.Next((int)this.bounds.Height);
        }

        private bool IsAtTarget(ref Vector2f position)
        {
            return Math.Abs(this.target.X - position.X) <= 1f && Math.Abs(this.target.Y - position.Y) <= 1f;
        }

        public override bool GetNextMove(ref Vector2f position, ref Vector2f velocity, ref int direction)
        {
            this.changed = false;
            if (!this.targetReachedFlag)
            {
                base.MovementStep(this.speed, ref position, ref this.target, ref velocity);
                this.changed = true;

                if (this.IsAtTarget(ref position) || this.abortTimer > this.timeToAbort)
                {
                    this.targetReachedFlag = true;
                    velocity = new Vector2f(0f, 0f);
                }
            }
            else if (this.timer < this.timeOut)
            {
                this.timer++;
            }
            else
            {
                this.Randomize();
                this.timer = 0;
                this.abortTimer = 0;
                this.targetReachedFlag = false;
            }
            this.abortTimer++;
            return this.changed;
        }

        private FloatRect bounds;

        private readonly float distance;

        private Vector2f target;

        private int timer;

        private readonly int timeOut;

        private int abortTimer;

        private readonly int timeToAbort = 120;

        private readonly float speed;

        private bool changed;

        private bool targetReachedFlag;
    }
}

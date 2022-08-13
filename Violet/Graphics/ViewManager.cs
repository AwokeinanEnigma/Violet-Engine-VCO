using SFML.Graphics;
using SFML.System;
using System;
using Violet.Actors;
using Violet.Utility;

namespace Violet.Graphics
{
    public class ViewManager
    {
        public static ViewManager Instance
        {
            get
            {
                if (ViewManager.instance == null)
                {
                    ViewManager.instance = new ViewManager();
                }
                return ViewManager.instance;
            }
        }

        public event ViewManager.OnMoveHandler OnMove;

        public event ViewManager.OnMoveToCompleteHandler OnMoveToComplete;

        public View View
        {
            get
            {
                return this.view;
            }
        }

        public FloatRect Viewrect
        {
            get
            {
                this.SetViewRect();
                return this.viewRect;
            }
        }

        public Vector2f FinalCenter
        {
            get
            {
                return this.GetCenter();
            }
        }

        public Vector2f FinalTopLeft
        {
            get
            {
                return this.GetCenter() - Engine.HALF_SCREEN_SIZE;
            }
        }

        public Vector2f Center
        {
            get
            {
                return this.viewCenter;
            }
            set
            {
                this.previousViewCenter = this.viewCenter;
                this.viewCenter = value;
                if (this.previousViewCenter != this.viewCenter && this.OnMove != null)
                {
                    this.OnMove(this, this.view.Center);
                }
            }
        }

        public Vector2f TopLeft
        {
            get
            {
                return this.viewCenter - Engine.HALF_SCREEN_SIZE;
            }
        }

        public Actor FollowActor
        {
            get
            {
                return this.followActor;
            }
            set
            {
                this.followActor = value;
            }
        }

        public Vector2f Offset
        {
            get
            {
                return this.offset;
            }
            set
            {
                this.offset = value;
            }
        }

        public ViewManager.MoveMode MoveToMode
        {
            get
            {
                return this.moveToMode;
            }
            set
            {
                this.moveToMode = value;
            }
        }

        private ViewManager()
        {
            this.window = Engine.FrameBuffer;
            this.view = new View(new Vector2f(0f, 0f), new Vector2f(320f, 180f));
            this.window.SetView(this.view);
            this.viewCenter = VectorMath.ZERO_VECTOR;
            this.offset = VectorMath.ZERO_VECTOR;
            this.shakeOffset = VectorMath.ZERO_VECTOR;
            this.viewRect = default(FloatRect);
            this.SetViewRect();
        }

        private Vector2f GetCenter()
        {
            return VectorMath.Truncate(this.viewCenter + this.offset + this.shakeOffset);
        }

        private void SetViewRect()
        {
            this.viewRect.Left = this.view.Center.X - this.view.Size.X / 2f;
            this.viewRect.Top = this.view.Center.Y - this.view.Size.Y / 2f;
            this.viewRect.Width = this.view.Size.X;
            this.viewRect.Height = this.view.Size.Y;
        }

        private float CalculateSmoothMovement(float progress)
        {
            return (float)(1.0 - (Math.Cos(progress * 2.0 * 3.141592653589793) / 2.0 + 0.5)) * (this.moveToSpeed - 0.5f) + 0.5f;
        }

        public void Update()
        {
            if (this.shakeProgress < this.shakeDuration)
            {
                float num = this.shakeIntensity.X * (1f - shakeProgress / (float)this.shakeDuration);
                float num2 = this.shakeIntensity.Y * (1f - shakeProgress / (float)this.shakeDuration);
                this.shakeOffset.X = -num + (float)Engine.Random.NextDouble() * num * 2f;
                this.shakeOffset.Y = -num2 + (float)Engine.Random.NextDouble() * num2 * 2f;
                this.shakeProgress++;
            }
            if (this.followActor != null)
            {
                if (!this.isMovingTo)
                {
                    this.previousViewCenter = this.viewCenter;
                    this.viewCenter = this.followActor.Position;
                }
                else
                {
                    this.moveToPosition = this.followActor.Position;
                }
            }
            if (this.isMovingTo)
            {
                float num3 = VectorMath.Magnitude(this.moveFromPosition - this.moveToPosition);
                float num4 = VectorMath.Magnitude(this.viewCenter - this.moveToPosition);
                float num5 = (num3 > 0f) ? (1f - num4 / num3) : 1f;
                float num6 = 1f;
                switch (this.moveToMode)
                {
                    case ViewManager.MoveMode.Linear:
                        num6 = this.moveToSpeed;
                        break;
                    case ViewManager.MoveMode.Smoothed:
                        num6 = this.CalculateSmoothMovement(num5);
                        break;
                    case ViewManager.MoveMode.ExpIn:
                        num6 = (1f - num5) * (this.moveToSpeed - 0.5f) + 0.5f;
                        break;
                    case ViewManager.MoveMode.ExpOut:
                        num6 = num5 * (this.moveToSpeed - 0.5f) + 0.5f;
                        break;
                }
                this.previousViewCenter = this.viewCenter;
                this.viewCenter += VectorMath.Normalize(this.moveToPosition - this.moveFromPosition) * Math.Max(0.1f, num6);
                if (num4 - num6 <= 0.5f)
                {
                    this.viewCenter = this.moveToPosition;
                    this.isMovingTo = false;
                    if (this.OnMoveToComplete != null)
                    {
                        this.OnMoveToComplete(this);
                    }
                }
            }
            this.view.Center = VectorMath.Truncate(this.viewCenter + this.offset + this.shakeOffset);
            if (this.previousViewCenter != this.viewCenter && this.OnMove != null)
            {
                this.OnMove(this, this.view.Center);
            }
        }

        public void MoveTo(Actor actor, float speed)
        {
            if (actor != null)
            {
                this.followActor = actor;
                this.MoveTo(actor.Position, speed);
            }
        }

        public void MoveTo(Vector2f position, float speed)
        {
            if (speed > 0f)
            {
                this.moveFromPosition = this.viewCenter;
                this.moveToPosition = position;
                this.moveToSpeed = speed;
                this.isMovingTo = true;
                return;
            }
            this.moveFromPosition = this.viewCenter;
            this.moveToPosition = position;
            this.viewCenter = this.moveToPosition;
            this.moveToSpeed = 0f;
            this.isMovingTo = false;
        }

        public void MoveTo(float x, float y, float speed)
        {
            this.MoveTo(new Vector2f(x, y), speed);
        }

        public void CancelMoveTo()
        {
            this.isMovingTo = false;
        }

        public void Move(float x, float y)
        {
            this.Move(new Vector2f(x, y));
        }

        public void Move(Vector2f offset)
        {
            this.view.Move(offset);
        }

        public void UseView()
        {
            this.window.SetView(this.view);
        }

        public void UseDefault()
        {
            this.window.SetView(this.window.DefaultView);
        }

        public void Reset()
        {
            this.view.Reset(new FloatRect(0f, 0f, 320f, 180f));
            this.viewCenter = this.view.Center;
            this.shakeOffset = VectorMath.ZERO_VECTOR;
        }

        public void Shake(Vector2f intensity, float duration)
        {
            this.shakeIntensity = intensity;
            this.shakeDuration = (int)(duration * 60f);
            this.shakeProgress = 0;
            this.shakeOffset = VectorMath.ZERO_VECTOR;
        }

        private static ViewManager instance;

        private View view;

        private RenderTarget window;

        private Actor followActor;

        private FloatRect viewRect;

        private Vector2f previousViewCenter;

        private Vector2f viewCenter;

        private Vector2f offset;

        private Vector2f shakeOffset;

        private Vector2f shakeIntensity;

        private int shakeDuration;

        private int shakeProgress;

        private bool isMovingTo;

        private Vector2f moveFromPosition;

        private Vector2f moveToPosition;

        private float moveToSpeed;

        private ViewManager.MoveMode moveToMode;

        public enum MoveMode
        {
            Linear,
            Smoothed,
            ExpIn,
            ExpOut
        }

        public delegate void OnMoveHandler(ViewManager sender, Vector2f newCenter);

        public delegate void OnMoveToCompleteHandler(ViewManager sender);
    }
}

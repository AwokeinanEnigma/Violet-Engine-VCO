using SFML.System;
using System;
using VCO.GUI;
using Violet;
using Violet.Graphics;
using Violet.Scenes.Transitions;

namespace VCO.Scenes.Transitions
{
    internal class IrisTransition : ITransition
    {
        public bool IsComplete => this.isComplete;

        public float Progress => this.progress;

        public bool ShowNewScene => this.progress > 0.5f;

        public bool Blocking { get; set; }

        public IrisTransition(float duration)
        {
            Vector2f origin = new Vector2f(160f, 90f);
            this.overlay = new IrisOverlay(ViewManager.Instance.FinalCenter, origin, 0f);
            float num = 60f * duration;
            this.speed = 1f / num;
            this.holdFrames = 30;
        }

        public void Update()
        {
            if (!this.isComplete)
            {
                if (this.progress < 0.5f)
                {
                    float num = 1f - this.progress / 0.5f;
                    this.overlay.Progress = (float)(-(float)Math.Cos(num * 3.141592653589793) + 1.0) / 2f;
                }
                else if (this.holdTimer < this.holdFrames)
                {
                    this.holdTimer++;
                }
                else
                {
                    float num2 = (this.progress - 0.5f) / 0.5f;
                    this.overlay.Progress = (float)(-(float)Math.Cos(num2 * 3.141592653589793) + 1.0) / 2f;
                }
                if (this.holdTimer == 0 || this.holdTimer >= this.holdFrames)
                {
                    if (this.progress < 1f)
                    {
                        this.progress += this.speed;
                        return;
                    }
                    this.isComplete = true;
                }
            }
        }

        public void Draw()
        {
            this.overlay.Draw(Engine.FrameBuffer);
        }

        public void Reset()
        {
            this.isComplete = false;
            this.progress = 0f;
            this.overlay.Progress = 1f;
        }

        public void Destroy()
        {
        }

        private bool isComplete;

        private float progress;

        private readonly float speed;

        private int holdTimer;

        private readonly int holdFrames;

        private readonly IrisOverlay overlay;
    }
}

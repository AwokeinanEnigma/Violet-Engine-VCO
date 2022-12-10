using SFML.System;
using System;
using VCO.GUI.Modifiers;
using Violet.Graphics;
using Violet.Utility;

namespace VCO.Battle.UI.Modifiers
{
    internal class GraphicShielder : IGraphicModifier, IDisposable
    {
        // (get) Token: 0x0600006B RID: 107 RVA: 0x00004B3F File Offset: 0x00002D3F
        public bool Done => this.isDone;
        // (get) Token: 0x0600006C RID: 108 RVA: 0x00004B47 File Offset: 0x00002D47
        public Graphic Graphic => this.graphic;
        public GraphicShielder(RenderPipeline pipeline, Graphic graphic)
        {
            this.pipeline = pipeline;
            this.graphic = graphic;
            this.shieldAnims = new AnimatedRenderable[GraphicShielder.SHIELD_POINTS.Length];
            for (int i = 0; i < this.shieldAnims.Length; i++)
            {
                this.shieldAnims[i] = new IndexedColorGraphic(DataHandler.instance.Load("shield.dat"), "bubble", this.graphic.Position + GraphicShielder.SHIELD_POINTS[i], this.graphic.Depth + 10)
                {
                    Visible = false,
                    SpeedModifier = 0f
                };
                this.shieldAnims[i].OnAnimationComplete += this.OnAnimationComplete;
                this.pipeline.Add(this.shieldAnims[i]);
            }
            this.timerIndex = FrameTimerManager.Instance.StartTimer(6);
            FrameTimerManager.Instance.OnTimerEnd += this.OnTimerEnd;
            this.nextAnim = true;
            this.animIndex = 0;
            this.isDone = false;
        }
        ~GraphicShielder()
        {
            this.Dispose(false);
        }
        public void Update()
        {
            if (!this.isDone && this.nextAnim)
            {
                this.nextAnim = false;
                this.shieldAnims[this.animIndex].SpeedModifier = 1f;
                this.shieldAnims[this.animIndex].Visible = true;
                this.animIndex++;
                if (this.animIndex < this.shieldAnims.Length)
                {
                    this.timerIndex = FrameTimerManager.Instance.StartTimer(6);
                    return;
                }
                FrameTimerManager.Instance.OnTimerEnd -= this.OnTimerEnd;
                this.isDone = true;
            }
        }
        private void OnTimerEnd(int timerIndex)
        {
            if (this.timerIndex == timerIndex)
            {
                this.nextAnim = true;
            }
        }
        private void OnAnimationComplete(AnimatedRenderable anim)
        {
            anim.Visible = false;
            anim.OnAnimationComplete -= this.OnAnimationComplete;
            this.pipeline.Remove(anim);
        }
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    for (int i = 0; i < this.shieldAnims.Length; i++)
                    {
                        this.shieldAnims[i].Dispose();
                    }
                }
                this.disposed = true;
            }
        }
        private const int SHIELD_WAIT_FRAMES = 6;
        private static readonly Vector2f[] SHIELD_POINTS = new Vector2f[]
        {
            new Vector2f(16f, 12f),
            new Vector2f(45f, 43f),
            new Vector2f(16f, 35f),
            new Vector2f(45f, 20f)
        };
        private bool disposed;
        private bool isDone;
        private readonly Graphic graphic;
        private readonly RenderPipeline pipeline;
        private readonly AnimatedRenderable[] shieldAnims;
        private int timerIndex;
        private bool nextAnim;
        private int animIndex;
    }
}

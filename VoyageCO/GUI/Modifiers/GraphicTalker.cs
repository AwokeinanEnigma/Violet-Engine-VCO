using SFML.System;
using System;
using VCO.GUI.Modifiers;
using Violet.Graphics;

namespace VCO.Battle.UI.Modifiers
{
    internal class GraphicTalker : IGraphicModifier, IDisposable
    {
        // (get) Token: 0x060002FC RID: 764 RVA: 0x00013166 File Offset: 0x00011366
        public bool Done => this.done;
        // (get) Token: 0x060002FD RID: 765 RVA: 0x0001316E File Offset: 0x0001136E
        public Graphic Graphic => this.graphic;
        public GraphicTalker(RenderPipeline pipeline, Graphic graphic)
        {
            this.pipeline = pipeline;
            this.graphic = graphic;
            this.done = false;
            this.rightOffset = new Vector2f(this.graphic.TextureRect.Width, 0f);
            this.talker = new IndexedColorGraphic(DataHandler.instance.Load("chat.dat"), "left", this.graphic.Position - this.graphic.Origin, this.graphic.Depth + 1);
            this.talker.OnAnimationComplete += this.AnimationComplete;
            this.pipeline.Add(this.talker);
        }
        ~GraphicTalker()
        {
            this.Dispose(false);
        }
        private void AnimationComplete(AnimatedRenderable graphic)
        {
            this.count++;
            if (this.count > 2)
            {
                this.right = !this.right;
                this.talker.SetSprite(this.right ? "right" : "left", true);
                this.count = 0;
                this.Update();
            }
        }
        public void Update()
        {
            if (this.right)
            {
                this.talker.Position = this.graphic.Position - this.graphic.Origin + this.rightOffset;
                return;
            }
            this.talker.Position = this.graphic.Position - this.graphic.Origin;
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
                    this.pipeline.Remove(this.talker);
                    this.talker.Dispose();
                }
                this.disposed = true;
            }
        }
        private const int LOOP_TOTAL = 2;
        private bool disposed;
        private readonly RenderPipeline pipeline;
        private readonly Graphic graphic;
        private readonly IndexedColorGraphic talker;
        private Vector2f rightOffset;
        private int count;
        private bool right;
        private readonly bool done;
    }
}
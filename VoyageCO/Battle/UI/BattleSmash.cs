using SFML.System;
using Violet.Graphics;

namespace VCO.Battle.UI
{
    internal class BattleSmash
    {
        // (get) Token: 0x0600025C RID: 604 RVA: 0x0000EDE9 File Offset: 0x0000CFE9
        public bool Done => this.done;
        public BattleSmash(RenderPipeline pipeline, Vector2f position)
        {
            this.pipeline = pipeline;
            this.smashGraphic = new IndexedColorGraphic(DataHandler.instance.Load("smash.dat"), "smash", position, 32767);
            this.smashGraphic.OnAnimationComplete += this.SmashgraphicAnimationComplete;
            this.pipeline.Add(this.smashGraphic);
        }
        private void SmashgraphicAnimationComplete(AnimatedRenderable graphic)
        {
            this.pipeline.Remove(this.smashGraphic);
            this.done = true;
        }
        private readonly RenderPipeline pipeline;
        private readonly Graphic smashGraphic;
        private bool done;
    }
}
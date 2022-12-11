using SFML.System;
using Violet.Graphics;

namespace VCO.Battle.UI
{
    internal class BattleSmash
    {
        #region Properties
        public bool Done => this.done;
        #region Fields
        private readonly RenderPipeline pipeline;
        
        private readonly Graphic smashGraphic;
        
        private bool done;
#endregion
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

    }
}
using SFML.Graphics;
using SFML.System;
using Violet.Graphics;
using Violet.Utility;

namespace VCO.Battle.Background
{
    internal class BattleBackgroundRenderable : Renderable
    {
        public BattleBackgroundRenderable(string file, int depth)
        {
            this.position = VectorMath.ZERO_VECTOR;
            this.origin = VectorMath.ZERO_VECTOR;
            this.size = new Vector2f(float.MaxValue, float.MaxValue);
            this.depth = depth;
            this.bbg = new BattleBackground(file);
        }

        public void AddTranslation(float x, float y, float xFactor, float yFactor)
        {
            this.bbg.AddTranslation(x, y, xFactor, yFactor);
        }

        public override void Draw(RenderTarget target)
        {
            this.bbg.Draw(target);
        }

        private BattleBackground bbg;
    }
}

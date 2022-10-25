using System;
using Violet;
using Violet.Actors;
using Violet.Graphics;
using Violet.Scenes;

namespace VCO.Scenes
{
	internal class StandardScene : Scene
	{
		protected StandardScene()
		{
			this.pipeline = new RenderPipeline(Engine.FrameBuffer);
			this.actorManager = new ActorManager();
		}

		public override void Focus()
		{
			base.Focus();
			ViewManager.Instance.CancelMoveTo();
		}

		public override void Update()
		{
			base.Update();
			this.actorManager.Step();
		}

		public override void Draw()
		{
			base.Draw();
			this.pipeline.Draw();
		}

		protected RenderPipeline pipeline;

		protected ActorManager actorManager;
	}
}

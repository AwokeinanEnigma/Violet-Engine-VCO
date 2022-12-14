using SFML.Graphics;
using SFML.System;
using System;
using System.IO;
using Violet.Graphics;
using Violet.GUI;
using Violet.Input;

namespace Violet.Scenes
{
    public class ErrorScene : Scene
    {
        private RenderPipeline pipeline;

        private TextRegion title;
        private TextRegion message;
        private TextRegion pressenter;
        private TextRegion exceptionDetails;
        private TextRegion additionalUserDetails;

        public ErrorScene(Exception ex)
        {
            StreamWriter streamWriter = new StreamWriter("Data/Logs/error.log");
            streamWriter.WriteLine(ex);
            streamWriter.Close();
            //  Engine.ClearColor = Color.Blue;
            this.title = new TextRegion(new Vector2f(3f, 8f), 0, Engine.DefaultFont, "An unhandled exception has occurred.");
            this.message = new TextRegion(new Vector2f(3f, 32f), 0, Engine.DefaultFont, "Enigma is obviously an incompetent programmer.");
            this.pressenter = new TextRegion(new Vector2f(3f, 48f), 0, Engine.DefaultFont, "Press Enter/Start to exit.");
            this.exceptionDetails = new TextRegion(new Vector2f(3f, 80f), 0, Engine.DefaultFont, string.Format("{0}\nSee error.log for more details.", ex.Message));
            this.additionalUserDetails = new TextRegion(new Vector2f(3f, 110), 0, Engine.DefaultFont, "Additionally, files detailing the state of the " +
                "\nTextureManager and all logs prior to the error have " +
                "\nbeen dumped.");

            //todo - change this to a nonpersistant path
            //IndexedColorGraphic graphic = new IndexedColorGraphic($"C:\\Users\\Tom\\source\\repos\\SunsetRhapsody\\SunsetRhapsody\\bin\\Release\\Resources\\Graphics\\whoops.dat", "whoops", new Vector2f(160, 90), 100);
            this.pipeline = new RenderPipeline(Engine.FrameBuffer);
            //pipeline.Add(graphic);


            this.pipeline.Add(this.title);
            this.pipeline.Add(this.message);
            this.pipeline.Add(this.pressenter);
            this.pipeline.Add(this.exceptionDetails);
            this.pipeline.Add(this.additionalUserDetails);
            Debug.DumpLogs();
            TextureManager.Instance.DumpEveryLoadedTexture();
            TextureManager.Instance.DumpLoadedTextures();
        }

        public override void Focus()
        {
            base.Focus();
            ViewManager.Instance.FollowActor = null;
            ViewManager.Instance.Center = new Vector2f(160f, 90f);
            Engine.ClearColor = Color.Black;
        }

        public override void Update()
        {
            base.Update();
            if (InputManager.Instance.State[Button.Start] || InputManager.Instance.State[Button.A])
            {
                InputManager.Instance.State[Button.Start] = false;
                InputManager.Instance.State[Button.A] = false;
                this.pipeline.Remove(this.title);
                this.pipeline.Remove(this.message);
                this.pipeline.Remove(this.pressenter);
                this.pipeline.Remove(this.exceptionDetails);
                SceneManager.Instance.Pop();
            }
        }

        public override void Draw()
        {
            this.pipeline.Draw();
            base.Draw();
        }

        protected override void Dispose(bool disposing)
        {
            if (!this.disposed && disposing)
            {
                this.title.Dispose();
                this.message.Dispose();
                this.pressenter.Dispose();
                this.exceptionDetails.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}

using SFML.System;
using System;
using System.Collections.Generic;
using System.Threading;
using VCO.Data;
using Violet;
using Violet.Graphics;

namespace VCO.Scenes
{
    internal class SaveScene : StandardScene
    {
        #region Fields
        private static readonly string POSTCARD_PATH = Paths.GRAPHICS + "postcard.dat";
        private static readonly Vector2f STAMP_OFFSET = new Vector2f(61f, -19f);
        private static readonly Vector2f WRITING_OFFSET = new Vector2f(-44f, 0f);
        private readonly SaveScene.Location location;
        private SaveProfile profile;
        private bool initialized;
        private bool done;
        private int delta;
        private int writingFrames;
        private bool writingAnimationDone;
        private bool saveThreadDone;
        private Thread saveThread;
        private IndexedColorGraphic cardFront;
        private IndexedColorGraphic cardBack;
        private IndexedColorGraphic stamp;
        private IndexedColorGraphic flagMark;
        private IndexedColorGraphic writing;
        #endregion
        #region Enums
        public enum Location
        {
            Belring
        }
        #endregion
        public SaveScene(SaveScene.Location location, SaveProfile profile)
        {
            this.writingFrames = 32767;
            this.location = location;
            this.profile = profile;
        }

        private void StartSave()
        {
            this.saveThread = new Thread(delegate ()
            {
                SaveFileManager.Instance.CurrentProfile = this.profile;
                SaveFileManager.Instance.SaveFile();
                this.saveThreadDone = true;
                if (this.writingAnimationDone)
                {
                    this.writingFrames = this.delta + 8;
                }
            });
            this.saveThread.Start();
        }

        private void Initialize()
        {
            this.StartSave();
            this.cardFront = new IndexedColorGraphic(SaveScene.POSTCARD_PATH, "front " + SaveScene.LOCATION_STRINGS[this.location], Engine.HALF_SCREEN_SIZE, 0);
            this.cardBack = new IndexedColorGraphic(SaveScene.POSTCARD_PATH, "back", Engine.HALF_SCREEN_SIZE, 0)
            {
                Visible = false
            };
            this.stamp = new IndexedColorGraphic(SaveScene.POSTCARD_PATH, "stamp " + SaveScene.LOCATION_STRINGS[this.location], Engine.HALF_SCREEN_SIZE + SaveScene.STAMP_OFFSET, 0)
            {
                Visible = false,
                Rotation = 10f
            };
            this.flagMark = new IndexedColorGraphic(SaveScene.POSTCARD_PATH, "flag stamp", Engine.HALF_SCREEN_SIZE + SaveScene.STAMP_OFFSET, 0)
            {
                Visible = false
            };
            this.writing = new IndexedColorGraphic(SaveScene.POSTCARD_PATH, "writing", Engine.HALF_SCREEN_SIZE + SaveScene.WRITING_OFFSET, 0)
            {
                Visible = false,
                SpeedModifier = 0f
            };
            this.writing.OnAnimationComplete += this.WritingAnimationDone;
            this.pipeline.Add(this.cardFront);
            this.pipeline.Add(this.cardBack);
            this.pipeline.Add(this.stamp);
            this.pipeline.Add(this.flagMark);
            this.pipeline.Add(this.writing);
            this.delta = -8;
            this.initialized = true;
        }

        private void WritingAnimationDone(AnimatedRenderable graphic)
        {
            this.writing.Frame = this.writing.Frames - 1;
            this.writing.SpeedModifier = 0f;
            this.writingAnimationDone = true;
            if (this.saveThreadDone)
            {
                this.writingFrames = this.delta + 8;
            }
        }

        public override void Focus()
        {
            base.Focus();
            if (!this.initialized)
            {
                this.Initialize();
            }
        }

        public override void Update()
        {
            base.Update();
            if (this.initialized && !this.done)
            {
                if (this.delta >= 0 && this.delta < 19)
                {
                    float x = (float)(Math.Cos(3.141592653589793 * (delta / 19.0)) / 2.0 + 0.5);
                    this.cardFront.Scale = new Vector2f(x, 1f);
                }
                if (this.delta == 19)
                {
                    this.cardFront.Visible = false;
                    this.cardBack.Visible = true;
                }
                if (this.delta >= 19 && this.delta < 38)
                {
                    float x2 = (float)(Math.Cos(3.141592653589793 * (delta / 19.0)) / 2.0 + 0.5);
                    this.cardBack.Scale = new Vector2f(x2, 1f);
                }
                if (this.delta == 40)
                {
                    this.writing.Visible = true;
                    this.writing.SpeedModifier = 1f;
                }
                if (this.delta >= 40 && this.delta < 42)
                {
                    ViewManager.Instance.View.Rotate(0.5f);
                }
                if (this.delta >= 42 && this.delta < 44)
                {
                    ViewManager.Instance.View.Rotate(-0.5f);
                }
                if (this.delta >= this.writingFrames && this.delta < this.writingFrames + 4)
                {
                    this.stamp.Visible = true;
                    this.stamp.Rotation -= 2.5f;
                }
                if (this.delta == this.writingFrames + 4)
                {
                    ViewManager.Instance.Reset();
                }
                if (this.delta >= this.writingFrames + 8 + 1 && this.delta < this.writingFrames + 8 + 1 + 5)
                {
                    this.flagMark.Visible = true;
                    ViewManager.Instance.View.Zoom(1.05f);
                    ViewManager.Instance.View.Rotate(-0.5f);
                }
                if (this.delta >= this.writingFrames + 8 + 1 + 5 && this.delta < this.writingFrames + 8 + 1 + 10)
                {
                    this.flagMark.Visible = true;
                    ViewManager.Instance.View.Rotate(0.5f);
                    ViewManager.Instance.View.Zoom(0.95f);
                }
                if (this.delta >= this.writingFrames + 8 + 1 + 10)
                {
                    ViewManager.Instance.Reset();
                    this.done = true;
                }
                this.delta++;
            }
        }

        private static readonly Dictionary<SaveScene.Location, string> LOCATION_STRINGS = new Dictionary<SaveScene.Location, string>
        {
            {
                SaveScene.Location.Belring,
                "belring"
            }
        };
    }
}

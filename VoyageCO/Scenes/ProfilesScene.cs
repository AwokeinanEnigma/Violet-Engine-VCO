using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using VCO.Data;
using VCO.GUI;
using VCO.GUI.ProfileMenu;
using Violet;
using Violet.Audio;
using Violet.Graphics;
using Violet.Input;
using Violet.Scenes;

namespace VCO.Scenes
{
    internal class ProfilesScene : StandardScene
    {
        public ProfilesScene()
        {
            this.panelList = new List<MenuPanel>();
        }

        private void ButtonPressed(InputManager sender, Button b)
        {
            if (b == Button.A)
            {
                this.DoLoad();
                return;
            }
            if (b == Button.B)
            {
                this.sfxCancel.Play();
                SceneManager.Instance.Pop();
            }
        }

        private void AxisPressed(InputManager sender, Vector2f axis)
        {
            if (axis.Y < 0f)
            {
                this.selectedIndex = Math.Max(0, this.selectedIndex - 1);
                this.UpdateCursor();
                return;
            }
            if (axis.Y > 0f)
            {
                this.selectedIndex = Math.Min(this.panelList.Count - 1, this.selectedIndex + 1);
                this.UpdateCursor();
            }
        }

        private void UpdateCursor()
        {
            this.cursorGraphic.Position = new Vector2f(24f, 32 + 57 * this.selectedIndex);
            this.sfxCursorY.Play();
        }

        private void DoLoad()
        {
            if (this.profileList.ContainsKey(this.selectedIndex))
            {
                this.sfxConfirm.Play();
                SaveFileManager.Instance.LoadFile(this.selectedIndex);
                Engine.StartSession();
                OverworldScene newScene = new OverworldScene(SaveFileManager.Instance.CurrentProfile.MapName, SaveFileManager.Instance.CurrentProfile.Position, 6, false, false, true);
                SceneManager.Instance.Push(newScene, true);
                return;
            }
            this.sfxCancel.Play();
        }

        private void GenerateSelectionList()
        {
            this.profileList = SaveFileManager.Instance.LoadProfiles();
            int num = Math.Max(3, this.profileList.Count);
            for (int i = 0; i < num; i++)
            {
                MenuPanel item = new ProfilePanel(new Vector2f(8f, 8 + 57 * i), new Vector2f(288f, 33f), i, this.profileList.ContainsKey(i) ? this.profileList[i] : default(SaveProfile));
                this.panelList.Add(item);
            }
            this.pipeline.AddAll<MenuPanel>(this.panelList);
            this.cursorGraphic = new IndexedColorGraphic(DataHandler.instance.Load("realcursor.dat"), "right", new Vector2f(24f, 32 + 57 * this.selectedIndex), 100);
            this.pipeline.Add(this.cursorGraphic);
        }

        public override void Focus()
        {
            base.Focus();
            if (!this.isInitialized)
            {
                this.GenerateSelectionList();
                this.sfxCursorX = AudioManager.Instance.Use(DataHandler.instance.Load(  "cursorx.wav"), AudioType.Sound);
                this.sfxCursorY = AudioManager.Instance.Use(DataHandler.instance.Load("cursory.wav"), AudioType.Sound);
                this.sfxConfirm = AudioManager.Instance.Use(DataHandler.instance.Load(  "confirm.wav"), AudioType.Sound);
                this.sfxCancel = AudioManager.Instance.Use(DataHandler.instance.Load("cancel.wav"), AudioType.Sound);
                this.isInitialized = true;
            }
            ViewManager.Instance.Center = Engine.HALF_SCREEN_SIZE;
            Engine.ClearColor = Color.Black;
            InputManager.Instance.ButtonPressed += this.ButtonPressed;
            InputManager.Instance.AxisPressed += this.AxisPressed;
        }

        public override void Unfocus()
        {
            base.Unfocus();
            InputManager.Instance.ButtonPressed -= this.ButtonPressed;
            InputManager.Instance.AxisPressed -= this.AxisPressed;
        }

        protected override void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    this.cursorGraphic.Dispose();
                    foreach (MenuPanel menuPanel in this.panelList)
                    {
                        menuPanel.Dispose();
                    }
                }
                AudioManager.Instance.Unuse(this.sfxCursorX);
                AudioManager.Instance.Unuse(this.sfxCursorY);
                AudioManager.Instance.Unuse(this.sfxConfirm);
                AudioManager.Instance.Unuse(this.sfxCancel);
                base.Dispose(disposing);
            }
        }

        private const int PANEL_WIDTH = 288;

        private const int PANEL_HEIGHT = 33;

        private bool isInitialized;

        private VioletSound sfxCursorX;

        private VioletSound sfxCursorY;

        private VioletSound sfxConfirm;

        private VioletSound sfxCancel;

        private IDictionary<int, SaveProfile> profileList;

        private IndexedColorGraphic cursorGraphic;

        private readonly IList<MenuPanel> panelList;

        private int selectedIndex;
    }
}

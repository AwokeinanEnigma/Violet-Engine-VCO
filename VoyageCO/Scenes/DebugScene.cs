using Rufini.Strings;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System;
using System.IO;
using System.Reflection;
using VCO.Data;
using VCO.Data.Enemies;
using VCO.GUI;
using VCO.GUI.Modifiers;
using Violet;
using Violet.Audio;
using Violet.Graphics;
using Violet.GUI;
using Violet.Input;
using Violet.Scenes;
using Violet.Scenes.Transitions;

namespace VCO.Scenes
{
    internal class DebugScene : StandardScene
    {
        private readonly IndexedColorGraphic spraypaintIcon;

        public bool canDraw;

        public DebugScene()
        {
            Fonts.LoadFonts(Settings.Locale);
            this.spraypaintIcon = new IndexedColorGraphic(DataHandler.instance.Load("spraypaint.dat"), "default", new Vector2f(160f, 44f), 100);
            this.pipeline.Add(this.spraypaintIcon);
        }

        public override void Draw()
        {
            base.Draw();
            //optionList.Draw();
        }

        public override void Focus()
        {
            base.Focus();

            InputManager.Instance.ButtonPressed += Instance_ButtonPressed;

            ViewManager.Instance.Center = new Vector2f(160f, 90f);
            Engine.ClearColor = Color.Black;
        }

        public uint palette;

        private void Instance_ButtonPressed(InputManager sender, Button b)
        {
            switch (b)
            {
                case Button.One:
                    if (palette != 0) palette--;
                    break;
                case Button.Two:
                    palette++;
                    break;
            }

            Debug.Log($"current palette is {palette}");
        }

        public override void Unfocus()
        {
            base.Unfocus();
        }

        public override void Update()
        {
            base.Update();
            spraypaintIcon.Position = InputManager.GetMousePosition();
           

            if (Mouse.IsButtonPressed(Mouse.Button.Left)) {
                IndexedColorGraphic block = new IndexedColorGraphic(DataHandler.instance.Load("block.dat"), "default", InputManager.GetMousePosition(), 10);
                block.CurrentPalette = palette;
                pipeline.Add(block);
                //block.CopyToTexture();
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    this.spraypaintIcon.Dispose();
                }
            }
            base.Dispose(disposing);
        }


    }
}

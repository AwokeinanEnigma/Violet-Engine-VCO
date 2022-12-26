using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System;
using Violet;
using Violet.Graphics;
using Violet.GUI;
using Violet.Input;
using VCO.Scenes;

namespace VCO
{
    internal class ArtScene : StandardScene
    {
        private readonly IndexedColorGraphic spraypaintIcon;
        private uint palette = 1;
        private uint layer;
        private TextRegion text;

        private readonly IndexedColorGraphic block;
        private readonly IndexedColorGraphic buzz;

        public ArtScene()
        {
            FontData data = new FontData();
            this.spraypaintIcon = new IndexedColorGraphic(DataHandler.instance.Load("spraypaint.dat"), "default", new Vector2f(160f, 44f), int.MaxValue);
            this.buzz = new IndexedColorGraphic(DataHandler.instance.Load("FungalWizard.dat"), "front", new Vector2f(180f, 44f), 100);
            this.block = new IndexedColorGraphic(DataHandler.instance.Load("block.dat"), "default", new Vector2f(buzz.Position.X - buzz.Origin.X - 2, 44f), int.MaxValue, 2);

            this.pipeline.Add(this.spraypaintIcon);
            this.pipeline.Add(this.block);
            this.pipeline.Add(this.buzz);
            clock.Restart();

            text = new TextRegion(new Vector2f(5, 0), int.MaxValue, data, $"Palette: {palette}          ");
            pipeline.Add(text);
            layer = 0;
        }



        public override void Focus()
        {
            base.Focus();
            InputManager.Instance.ButtonPressed += Instance_ButtonPressed;

            Engine.Window.SetMouseCursorVisible(false);

            ViewManager.Instance.Center = new Vector2f(160f, 90f);
            Engine.ClearColor = Color.Black;
        }

        public bool canMove;

        private void Instance_ButtonPressed(InputManager sender, Button b)
        {
            //if (palette + 1 < ((IndexedTexture)spraypaintIcon.Texture).PaletteCount )
            {
                switch (b)
                {
                    case Button.One:
                        if (palette != 1) palette--;
                        break;
                    case Button.Two:
                        palette++;
                        break;
                    case Button.Tilde:
                        canMove = true;
                        break;
                }
                //spraypaintIcon.CurrentPalette = palette;
                if (palette != 6)
                {
                    text.Text = $"Palette: {(int)palette}";
                }
                else
                {
                    text.Text = $"Eraser";
                }
                Debug.Log($"palette goes {palette}");
            }
        }

        Clock clock = new Clock();

        public override void Update()
        {
            base.Update();
            spraypaintIcon.Position = InputManager.GetMousePosition();

            float elapsedTime = clock.ElapsedTime.AsSeconds();
            if (elapsedTime >= 0.1f && canMove)
            {
                block.Position += new Vector2f(1, 0);
                clock.Restart();
            }
            // Check if the sprite has moved off the width of the background image


            if (Mouse.IsButtonPressed(Mouse.Button.Left) && Engine.Window.HasFocus())
            {
                IndexedColorGraphic block = new IndexedColorGraphic(DataHandler.instance.Load("block.dat"), "default", InputManager.GetMousePosition(), (int)layer);
                pipeline.Add(block);

                block.CurrentPalette = palette;
                layer++;
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

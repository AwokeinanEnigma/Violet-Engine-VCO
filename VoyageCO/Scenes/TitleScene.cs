using Rufini.Strings;
using SFML.Graphics;
using SFML.System;
using System;
using System.IO;
using System.Reflection;
using VCO.Battle.Background;
using VCO.Data;
using VCO.Data.Config;
using VCO.Data.Enemies;
using VCO.GUI;
using VCO.GUI.Modifiers;
using VCO.Scenes.Transitions;
using Violet;
using Violet.Audio;
using Violet.Graphics;
using Violet.GUI;
using Violet.Input;
using Violet.Scenes;
using Violet.Scenes.Transitions;

namespace VCO.Scenes
{
    internal class TitleScene : StandardScene
    {
        public BattleBackgroundRenderable background;
        public TitleScene()
        {
            Fonts.LoadFonts(Settings.Locale);

            if (ConfigReader.Instance.DebugMapName != null)
            {
                // 	Console.Write("THE DEBUG MAP IS REALLLLL");
            }

            string[] items;
            if (File.Exists("sav.dat"))
            {
                this.canContinue = true;
                items = new string[]
                {
                    "Map Test",
                    "New Game",
                    "Continue",
                    "Options",
                    "Quit"
                };
            }
            else
            {
                this.canContinue = false;
                items = new string[]
                {
                    "Map Test",
                    "New Game",
                    "Options",
                    "Quit"
                };
            }
            this.optionList = new ScrollingList(new Vector2f(32f, 50f), 9000, items, 5, 16f, 80f, DataHandler.instance.Load("cursor.dat"))
            {
                ShowSelectionRectangle = true,
                UseHighlightTextColor = false,
                ShowArrows = true,
                ShowCursor = true
            };
            optionList.Depth = 1100;
            this.pipeline.Add(this.optionList);
            this.background = new BattleBackgroundRenderable(DataHandler.instance.Load($"title.xml"), 10);
            pipeline.Add(background);
            this.titleImage = new IndexedColorGraphic(DataHandler.instance.Load("logo.dat"), "title", new Vector2f(160f, 90), 1000);

            ConvexShape shpae = new ConvexShape();
            shpae.SetPointCount(3);
            shpae.SetPoint(0, new Vector2f(1, 2));
            shpae.SetPoint(1, new Vector2f(25, 166));
            shpae.SetPoint(2, new Vector2f(23, 73));
            ShapeGraphic graphic = new ShapeGraphic(shpae, new Vector2f(0,0), new Vector2f(160, 90), new Vector2f(10, 10), 9999);
            graphic.FillColor = Color.Blue;
            graphic.OutlineColor = Color.Red;
            pipeline.Add(graphic);

            Version version = Assembly.GetEntryAssembly().GetName().Version;
            this.versionText = new TextRegion(new Vector2f(2f, 164f), 101, Fonts.Main, string.Format("{0}.{1} {2} {3} {4}", new object[]
            {
                version.Major,
                version.Minor,
                version.Build,
                version.Revision,
                StringFile.Instance.Get("AUX.symbols.alpha")
            }))
            {
                Color = new Color(byte.MaxValue, byte.MaxValue, byte.MaxValue, 128)
            };
            this.pipeline.Add(this.titleImage);
            this.pipeline.Add(this.versionText);

            //		this.mod = new GraphicTranslator(this.titleImage, new Vector2f(160f, 36f), 30);
            this.sfxCursorY = AudioManager.Instance.Use(DataHandler.instance.Load("cursory.wav"), AudioType.Sound);
            this.sfxConfirm = AudioManager.Instance.Use(DataHandler.instance.Load("confirm.wav"), AudioType.Sound);
            this.sfxCancel = AudioManager.Instance.Use(DataHandler.instance.Load("cancel.wav"), AudioType.Sound);
        }

        private void AxisPressed(InputManager sender, Vector2f axis)
        {
            if (axis.Y < -0.1f)
            {
                if (this.optionList.SelectPrevious())
                {
                    this.sfxCursorY.Play();
                    return;
                }
            }
            else if (axis.Y > 0.1f && this.optionList.SelectNext())
            {
                this.sfxCursorY.Play();
            }
        }

        public override void Draw()
        {
            base.Draw();
            //DrawBehind = true;

            //optionList.Draw();
        }

        private void ButtonPressed(InputManager sender, Button b)
        {
            if (b > Button.Start)
            {
                if (b != Button.F1)
                {
                    switch (b)
                    {
                        case Button.One:
                            goto IL_46;
                        case Button.Two:
                            Console.WriteLine("purple");
                            pipeline.Remove(background);
                            this.background = new BattleBackgroundRenderable(DataHandler.instance.Load($"title.xml"), 10);
                            pipeline.Add(background);
                            //SceneManager.Instance.Transition = new ColorFadeTransition(0.25f, Color.Black);
                            //SceneManager.Instance.Push(new TextTestScene());
                            return;
                        case Button.Three:
                            Console.WriteLine("blue");
                            pipeline.Remove(background);
                            this.background = new BattleBackgroundRenderable(DataHandler.instance.Load($"title_blue.xml"), 10);
                            pipeline.Add(background);
                            //			SceneManager.Instance.Transition = new ColorFadeTransition(0.25f, Color.Black);
                            //			SceneManager.Instance.Push(new AUXTestScene());
                            return;
                        case Button.Four:
                            Console.WriteLine("green");
                            pipeline.Remove(background);
                            this.background = new BattleBackgroundRenderable(DataHandler.instance.Load($"title_green.xml"), 10);
                            pipeline.Add(background);

                            //				SceneManager.Instance.Transition = new ColorFadeTransition(0.25f, Color.Black);
                            //			SceneManager.Instance.Push(new SaveScene(SaveScene.Location.Belring, SaveFileManager.Instance.CurrentProfile));
                            return;
                        case Button.Five:
                            //			SceneManager.Instance.Transition = new ColorFadeTransition(0.25f, Color.Black);
                            //			SceneManager.Instance.Push(new EnemyTestScene());
                            return;
                        case Button.Six:
                            {
                                PartyManager.Instance.Clear();
                                //		PartyManager.Instance.Add(CharacterType.Travis);
                                //		PartyManager.Instance.Add(CharacterType.Zack);
                                //		PartyManager.Instance.Add(CharacterType.Floyd);
                                PartyManager.Instance.Add(CharacterType.Meryl);
                                //		PartyManager.Instance.Add(CharacterType.Leo);
                                //Demiurge Filament
                                EnemyData dat = EnemyFile.Instance.GetEnemyData("Demiurge Filament");
                                EnemyData dat2 = EnemyFile.Instance.GetEnemyData("Infested Legs");
                                EnemyData[] data = new EnemyData[3]
                                {
                                    dat, dat, dat2
                                };

                                BattleScene scenea = new BattleScene(data, true);
                                SceneManager.Instance.Transition = new IrisTransition(3);
                                SceneManager.Instance.Push(scenea);
                                break;
                            }
                        case Button.Seven:
                            PartyManager.Instance.Clear();
                            PartyManager.Instance.Add(CharacterType.Travis);
                            PartyManager.Instance.Add(CharacterType.Zack);
                            PartyManager.Instance.Add(CharacterType.Floyd);
                            PartyManager.Instance.Add(CharacterType.Meryl);
                            PartyManager.Instance.Add(CharacterType.Leo);
                            BattleScene scene = new BattleScene(new EnemyData[3] { EnemyFile.Instance.GetEnemyData("Hermit Can"), EnemyFile.Instance.GetEnemyData("Modern Mind"), EnemyFile.Instance.GetEnemyData("Snagtagious Froog") }, true);
                            SceneManager.Instance.Push(scene);
                            break;
                        default:
                            return;
                    }
                    //Console.WriteLine("{0:x}\t{1}", Hash.Get("Hometown Strut"), "Hometown Strut");
                    //Console.WriteLine("{0:x}\t{1}", Hash.Get("Hometown Laze"), "Hometown Laze");
                    //Console.WriteLine("{0:x}\t{1}", Hash.Get("A House"), "A House");
                    return;
                }
            IL_46:
                SceneManager.Instance.Transition = new ColorFadeTransition(0.25f, Color.Black);
                PartyManager.Instance.Clear();
                PartyManager.Instance.AddAll(new CharacterType[]
                {
                    CharacterType.Travis,
                    CharacterType.Floyd,
                    CharacterType.Meryl
                });
                OverworldScene newScene = new OverworldScene("debug_room.dat", new Vector2f(256f, 128f), 6, false, false, false);
                SceneManager.Instance.Push(newScene);
                return;
            }
            if (b != Button.A && b != Button.Start)
            {
                return;
            }
            this.DoSelection();
        }

        private void DoSelection()
        {
            int num = this.optionList.SelectedIndex;
            if (!this.canContinue && num > 1)
            {
                num++;
            }
            switch (num)
            {
                case 0:
                    this.sfxConfirm.Play();
                    SceneManager.Instance.Transition = new ColorFadeTransition(0.25f, Color.Black);
                    SceneManager.Instance.Push(new MapTestSetupScene());
                    return;
                case 1:
                    this.sfxConfirm.Play();
                    SceneManager.Instance.Transition = new ColorFadeTransition(0.25f, Color.Black);
                    SceneManager.Instance.Push(new NamingScene());
                    return;
                case 2:
                    this.sfxConfirm.Play();
                    SceneManager.Instance.Transition = new ColorFadeTransition(0.25f, Color.Black);
                    SceneManager.Instance.Push(new ProfilesScene());
                    return;
                case 3:
                    this.sfxConfirm.Play();
                    SceneManager.Instance.Transition = new ColorFadeTransition(0.25f, Color.Black);
                    SceneManager.Instance.Push(new OptionsScene());
                    return;
                case 4:
                    this.sfxConfirm.Play();
                    SceneManager.Instance.Pop();
                    return;
                default:
                    return;
            }
        }

        public override void Focus()
        {
            base.Focus();
            ViewManager.Instance.Center = new Vector2f(160f, 90f);
            Engine.ClearColor = Color.Black;

            AudioManager.Instance.SetBGM(DataHandler.instance.Load("test.mp3"));
            AudioManager.Instance.BGM.Play();

            InputManager.Instance.AxisPressed += this.AxisPressed;
            InputManager.Instance.ButtonPressed += this.ButtonPressed;
        }

        public override void Unfocus()
        {
            base.Unfocus();
            InputManager.Instance.AxisPressed -= this.AxisPressed;
            InputManager.Instance.ButtonPressed -= this.ButtonPressed;
        }

        public override void Update()
        {
            base.Update();
            //this.mod.Update();
        }

        protected override void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    this.titleImage.Dispose();
                    this.optionList.Dispose();
                    this.versionText.Dispose();
                }
                AudioManager.Instance.Unuse(this.sfxCursorY);
                AudioManager.Instance.Unuse(this.sfxConfirm);
                AudioManager.Instance.Unuse(this.sfxCancel);
            }
            base.Dispose(disposing);
        }

        private readonly TextRegion versionText;

        private readonly ScrollingList optionList;

        private readonly IndexedColorGraphic titleImage;

        private readonly VioletSound sfxCursorY;

        private readonly VioletSound sfxConfirm;

        private readonly VioletSound sfxCancel;

        private readonly bool canContinue;
    }
}

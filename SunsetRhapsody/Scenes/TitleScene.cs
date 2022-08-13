﻿using System;
using System.IO;
using System.Reflection;
using Violet;
using Violet.Audio;
using Violet.Graphics;
using Violet.GUI;
using Violet.Input;
using Violet.Scenes;
using Violet.Scenes.Transitions;
using SunsetRhapsody.Data;
using SunsetRhapsody.Data.Enemies;
using SunsetRhapsody.GUI;
using SunsetRhapsody.GUI.Modifiers;
using SunsetRhapsody.Scenes.Transitions;
using Rufini.Strings;
using SFML.Graphics;
using SFML.System;

namespace SunsetRhapsody.Scenes
{
	internal class TitleScene : StandardScene
	{
		public TitleScene()
		{
			Fonts.LoadFonts(Settings.Locale);
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
			this.optionList = new ScrollingList(new Vector2f(32f, 80f), 0, items, 5, 16f, 80f, Paths.GRAPHICS + "cursor.dat");
			this.optionList.ShowSelectionRectangle = false;
			this.optionList.UseHighlightTextColor = false;
			this.pipeline.Add(this.optionList);
			this.titleImage = new IndexedColorGraphic(Paths.GRAPHICS + "title.dat", "title", new Vector2f(160f, 44f), 100);
			Version version = Assembly.GetEntryAssembly().GetName().Version;
			this.versionText = new TextRegion(new Vector2f(2f, 164f), 0, Fonts.Main, string.Format("{0}.{1} {2} {3} {4}", new object[]
			{
				version.Major,
				version.Minor,
				version.Build,
				version.Revision,
				StringFile.Instance.Get("psi.symbols.alpha")
			}));
			this.versionText.Color = new Color(byte.MaxValue, byte.MaxValue, byte.MaxValue, 128);
			this.pipeline.Add(this.titleImage);
			this.pipeline.Add(this.versionText);
			this.mod = new GraphicTranslator(this.titleImage, new Vector2f(160f, 36f), 30);
			this.sfxCursorY = AudioManager.Instance.Use(Paths.SFXMENU + "cursory.wav", AudioType.Sound);
			this.sfxConfirm = AudioManager.Instance.Use(Paths.SFXMENU + "confirm.wav", AudioType.Sound);
			this.sfxCancel = AudioManager.Instance.Use(Paths.SFXMENU + "cancel.wav", AudioType.Sound);
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
						/*case Button.Two:
							SceneManager.Instance.Transition = new ColorFadeTransition(0.25f, Color.Black);
							SceneManager.Instance.Push(new TextTestScene());
							return;*/
						case Button.Three:
							SceneManager.Instance.Transition = new ColorFadeTransition(0.25f, Color.Black);
							SceneManager.Instance.Push(new PsiTestScene());
							return;
						case Button.Four:
							SceneManager.Instance.Transition = new ColorFadeTransition(0.25f, Color.Black);
							SceneManager.Instance.Push(new SaveScene(SaveScene.Location.Belring, SaveFileManager.Instance.CurrentProfile));
							return;
						case Button.Five:
							SceneManager.Instance.Transition = new ColorFadeTransition(0.25f, Color.Black);
							SceneManager.Instance.Push(new EnemyTestScene());
							return;
						case Button.Six:
							{
								/*
								List<PsiData> allPsiData = PsiFile.Instance.GetAllPsiData();
								using (List<PsiData>.Enumerator enumerator = allPsiData.GetEnumerator())
								{
									while (enumerator.MoveNext())
									{
										PsiData psiData = enumerator.Current;
										Console.Write(psiData.QualifiedName);
										Console.Write(" ");
										for (int i = 0; i < psiData.Symbols.Length; i++)
										{
											Console.Write(psiData.GetSymbol(i));
											if (i < psiData.Symbols.Length - 1)
											{
												Console.Write(", ");
											}
										}
										Console.WriteLine();
									}
									return;
								}*/
								break;
							}
						case Button.Seven:
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
			//AudioManager.Instance.SetBGM(Paths.AUDIO + "test.mp3");
			//AudioManager.Instance.BGM.Play();
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
			this.mod.Update();
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

		private TextRegion versionText;

		private ScrollingList optionList;

		private IndexedColorGraphic titleImage;

		private VioletSound sfxCursorY;

		private VioletSound sfxConfirm;

		private VioletSound sfxCancel;

		private IGraphicModifier mod;

		private bool canContinue;
	}
}

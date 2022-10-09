using System;
using System.Collections.Generic;
using System.Linq;
using Violet.Graphics;
using Violet.GUI;
using Violet.Input;
using Violet.Utility;
using SunsetRhapsody.Battle;
using SunsetRhapsody.Data;
using SunsetRhapsody.AUX;
using Rufini.Strings;
using SFML.Graphics;
using SFML.System;

namespace SunsetRhapsody.GUI.OverworldMenu
{
	internal class AUXMenu : MenuPanel
	{
		public AUXMenu() : base(ViewManager.Instance.FinalTopLeft + AUXMenu.PANEL_POSITION, AUXMenu.PANEL_SIZE, 0)
		{
			Console.Write("create");

			RectangleShape rectangleShape = new RectangleShape(new Vector2f(1f, AUXMenu.PANEL_SIZE.Y * 0.6f));
			rectangleShape.FillColor = AUXMenu.DIVIDER_COLOR;
			this.vertDivider = new ShapeGraphic(rectangleShape, new Vector2f(AUXMenu.PANEL_SIZE.X * 0.33f, AUXMenu.PANEL_SIZE.Y * 0.3f), VectorMath.Truncate(rectangleShape.Size / 2f), rectangleShape.Size, 1);
			base.Add(this.vertDivider);
			RectangleShape rectangleShape2 = new RectangleShape(new Vector2f(AUXMenu.PANEL_SIZE.X, 1f));
			rectangleShape2.FillColor = AUXMenu.DIVIDER_COLOR;
			this.horizDivider = new ShapeGraphic(rectangleShape2, new Vector2f(AUXMenu.PANEL_SIZE.X * 0.5f, AUXMenu.PANEL_SIZE.Y * 0.66f), VectorMath.Truncate(rectangleShape2.Size / 2f), rectangleShape2.Size, 1);
			base.Add(this.horizDivider);
			CharacterType[] array = PartyManager.Instance.ToArray();
			this.tabs = new IndexedColorGraphic[array.Length];
			this.tabLabels = new TextRegion[array.Length];
			uint num = Settings.WindowFlavor * 2U;
			int num2 = 0;
			for (int i = 0; i < array.Length; i++)
			{
				if (AUXManager.Instance.CharacterHasAUX(array[i]))
				{
					this.tabs[num2] = new IndexedColorGraphic(Paths.GRAPHICS + "pause.dat", (num2 == this.selectedTab) ? "firsttag" : "tag", new Vector2f(-8f, -7f) + new Vector2f(50f * (float)num2, 0f), (num2 == this.selectedTab) ? 1 : -2);
					this.tabs[num2].CurrentPalette = ((num2 == this.selectedTab) ? num : (num + 1U));
					base.Add(this.tabs[num2]);
					this.tabLabels[num2] = new TextRegion(new Vector2f(-4f, -21f) + new Vector2f(50f * (float)num2, 0f), (num2 == this.selectedTab) ? 2 : -1, Fonts.Main, CharacterNames.GetName(array[i]));
					this.tabLabels[num2].Color = ((num2 == this.selectedTab) ? AUXMenu.ACTIVE_TAB_TEXT_COLOR : AUXMenu.INACTIVE_TAB_TEXT_COLOR);
					base.Add(this.tabLabels[num2]);
					num2++;
				}
			}
			Array.Resize<IndexedColorGraphic>(ref this.tabs, num2);
			Array.Resize<TextRegion>(ref this.tabLabels, num2);
			this.AUXTypeList = new ScrollingList(new Vector2f(8f, 0f), 0, AUXMenu.AUX_TYPE_STRINGS, 4, 14f, 50f, AUXMenu.CURSOR_FILE);
			base.Add(this.AUXTypeList);
			this.selectedList = this.AUXTypeList;
			this.SetupAUXList();
			this.descriptionText = new TextRegion(new Vector2f(8f, (float)((int)(AUXMenu.PANEL_SIZE.Y * 0.66f) + 4)), 0, Fonts.Main, this.GetDescription());
			base.Add(this.descriptionText);
		}

		private string GetDescription()
		{
			string text = null;
			if (this.selectedList != this.AUXTypeList && this.selectedList == this.AUXList)
			{
				string arg = this.AUXList.SelectedItem.Replace(" ", "").ToLower();
				string str = string.Format("{0}{1}", arg, this.selectedLevel + 1);
				text = StringFile.Instance.Get("AUXDesc." + str).Value;
			}
			if (text == null)
			{
				text = string.Empty;
			}
			return text;
		}

		private void ChangeSelectedLevel(int newLevel)
		{
			this.levelList[this.selectedLevel].ShowCursor = false;
			this.selectedLevel = newLevel;
			this.levelList[this.selectedLevel].ShowCursor = true;
		}

		private void UpdateDescription()
		{
			string description = this.GetDescription();
			this.descriptionText.Reset(description, 0, description.Length);
		}

		private void SelectAUXList()
		{
			this.AUXTypeList.ShowSelectionRectangle = false;
			this.AUXTypeList.ShowCursor = false;
			this.AUXTypeList.Focused = false;
			this.selectedList = this.AUXList;
			this.UpdateDescription();
			this.selectedList.ShowSelectionRectangle = true;
			this.selectedList.Focused = true;
			for (int i = 0; i < this.levelList.Length; i++)
			{
				this.levelList[i].ShowSelectionRectangle = true;
				this.levelList[i].Focused = true;
			}
		}

		private void SelectAUXTypeList()
		{
			if (this.AUXList != null)
			{
				this.AUXList.SelectedIndex = 0;
				this.AUXList.ShowSelectionRectangle = false;
				this.AUXList.Focused = false;
				for (int i = 0; i < this.levelList.Length; i++)
				{
					this.levelList[i].SelectedIndex = 0;
					this.levelList[i].ShowSelectionRectangle = false;
					this.levelList[i].Focused = false;
				}
			}
			this.selectedList = this.AUXTypeList;
			this.UpdateDescription();
			this.selectedList.ShowSelectionRectangle = true;
			this.selectedList.ShowCursor = true;
			this.selectedList.Focused = true;
			Console.Write("list");

		}

		public override void AxisPressed(Vector2f axis)
		{
			if (axis.Y < 0f)
			{
				int selectedIndex = this.selectedList.SelectedIndex;
				this.selectedList.SelectPrevious();
				if (selectedIndex != this.selectedList.SelectedIndex)
				{
					if (this.selectedList == this.AUXTypeList)
					{
						this.SetupAUXList();
					}
					else if (this.selectedList == this.AUXList)
					{
						for (int i = 0; i < this.levelList.Length; i++)
						{
							this.levelList[i].SelectPrevious();
						}
						while (this.levelList[this.selectedLevel].SelectedItem.Length == 0)
						{
							this.ChangeSelectedLevel(this.selectedLevel - 1);
						}
					}
					this.UpdateDescription();
					return;
				}
			}
			else if (axis.Y > 0f)
			{
				int selectedIndex2 = this.selectedList.SelectedIndex;
				this.selectedList.SelectNext();
				if (selectedIndex2 != this.selectedList.SelectedIndex)
				{
					if (this.selectedList == this.AUXTypeList)
					{
						this.SetupAUXList();
					}
					else if (this.selectedList == this.AUXList)
					{
						for (int j = 0; j < this.levelList.Length; j++)
						{
							this.levelList[j].SelectNext();
						}
						while (this.levelList[this.selectedLevel].SelectedItem.Length == 0)
						{
							this.ChangeSelectedLevel(this.selectedLevel - 1);
						}
					}
					this.UpdateDescription();
					return;
				}
			}
			else if (axis.X != 0f)
			{
				if (this.selectedList == this.AUXTypeList)
				{
					if (axis.X > 0f)
					{
						if (this.AUXList != null)
						{
							this.SelectAUXList();
							return;
						}
					}
					else if (axis.X < 0f)
					{
						return;
					}
				}
				else if (this.selectedList == this.AUXList)
				{
					if (axis.X < 0f)
					{
						if (this.selectedLevel <= 0)
						{
							this.SelectAUXTypeList();
							return;
						}
						this.ChangeSelectedLevel(this.selectedLevel - 1);
						this.UpdateDescription();
						return;
					}
					else if (axis.X > 0f)
					{
						int num = Math.Min(this.levelList.Length - 1, this.selectedLevel + 1);
						if (this.levelList[num].SelectedItem.Length > 0)
						{
							this.ChangeSelectedLevel(num);
							this.UpdateDescription();
						}
					}
				}
			}
		}

		private void SetupAUXList()
		{
			CharacterType[] array = PartyManager.Instance.ToArray();
			CharacterType characterType = array[this.selectedTab];
			IEnumerable<IAUX> collection;
			switch (this.AUXTypeList.SelectedIndex)
			{
			case 1:
				collection = AUXManager.Instance.GetCharacterAssistAUX(characterType).Cast<IAUX>();
				break;
			case 2:
				collection = AUXManager.Instance.GetCharacterDefenseAUX(characterType).Cast<IAUX>();
				break;
			case 3:
				collection = AUXManager.Instance.GetCharacterOtherAUX(characterType).Cast<IAUX>();
				break;
			default:
				collection = AUXManager.Instance.GetCharacterOffenseAUX(characterType).Cast<IAUX>();
				break;
			}
			this.AUXItemList = new List<IAUX>(collection);
			if (this.AUXList != null)
			{
				base.Remove(this.AUXList);
				this.AUXList.Dispose();
				this.AUXList = null;
			}
			if (this.levelList != null)
			{
				for (int i = 0; i < this.levelList.Length; i++)
				{
					if (this.levelList[i] != null)
					{
						base.Remove(this.levelList[i]);
						this.levelList[i].Dispose();
						this.levelList[i] = null;
					}
				}
			}
			else
			{
				this.levelList = new ScrollingList[4];
			}
			if (this.AUXItemList.Count > 0)
			{
				Console.Write("AUX>0");
				StatSet stats = CharacterStats.GetStats(characterType);
				string[] array2 = new string[this.AUXItemList.Count];
				string[][] array3 = new string[4][];
				for (int j = 0; j < array3.Length; j++)
				{
					array3[j] = new string[array2.Length];
				}
				for (int k = 0; k < array2.Length; k++)
				{
					array2[k] = this.AUXItemList[k].aux.QualifiedName;
					for (int l = 0; l < array3.Length; l++)
					{
						if (l < this.AUXItemList[k].aux.Symbols.Length && this.AUXItemList[k].aux.Symbols[l] <= stats.Level)
						{
							Console.Write(AUXMenu.AUX_LEVEL_STRINGS[l]);
							array3[l][k] = AUXMenu.AUX_LEVEL_STRINGS[l];
						}
						else
						{
							array3[l][k] = AUXMenu.AUX_LEVEL_STRINGS[l];
						}
					}
				}
				this.AUXList = new ScrollingList(new Vector2f(AUXMenu.PANEL_SIZE.X * 0.33f + 8f, 0f), 1, array2, 5, 14f, AUXMenu.PANEL_SIZE.X * 0.66f - 2f, AUXMenu.CURSOR_FILE);
				this.AUXList.ShowSelectionRectangle = false;
				this.AUXList.ShowCursor = false;
				this.AUXList.Focused = false;
				base.Add(this.AUXList);
				for (int m = 0; m < this.levelList.Length; m++)
				{
					this.levelList[m] = new ScrollingList(new Vector2f(AUXMenu.PANEL_SIZE.X * 0.33f + 80f + (float)(16 * m), 0f), 1, array3[m], 5, 14f, 1f, AUXMenu.CURSOR_FILE);
					this.levelList[m].ShowSelectionRectangle = false;
					this.levelList[m].ShowCursor = (m == 0);
					this.levelList[m].Focused = false;
				//	levelList.[m]
					base.Add(this.levelList[m]);
				}
			}
		}

		private void SelectTab(int index)
		{
			if (index < 0)
			{
				this.selectedTab = this.tabs.Length - 1;
			}
			else if (index >= this.tabs.Length)
			{
				this.selectedTab = 0;
			}
			else
			{
				this.selectedTab = index;
			}
			for (int i = 0; i < this.tabs.Length; i++)
			{
				this.tabs[i].CurrentPalette = ((i == this.selectedTab) ? 0U : 1U);
				this.tabs[i].Depth = ((i == this.selectedTab) ? 1 : -2);
				this.tabLabels[i].Color = ((i == this.selectedTab) ? AUXMenu.ACTIVE_TAB_TEXT_COLOR : AUXMenu.INACTIVE_TAB_TEXT_COLOR);
				this.tabLabels[i].Depth = ((i == this.selectedTab) ? 2 : -1);
			}
			this.SelectAUXTypeList();
			this.SetupAUXList();
		}

		public override object ButtonPressed(Button button)
		{
			object result = null;
			if (button == Button.A)
			{
				if (this.selectedList == this.AUXTypeList)
				{
					this.SelectAUXList();
				}
				else if (this.selectedList == this.AUXList)
				{
					result = new Tuple<IAUX, int>(this.AUXItemList[this.AUXList.SelectedIndex], this.selectedLevel);
				}
			}
			else if (button == Button.B)
			{
				if (this.selectedList == this.AUXTypeList)
				{
					result = -1;
				}
				else if (this.selectedList == this.AUXList)
				{
					this.SelectAUXTypeList();
				}
			}
			else if (button == Button.L)
			{
				this.SelectTab(this.selectedTab - 1);
			}
			else if (button == Button.R)
			{
				this.SelectTab(this.selectedTab + 1);
			}
			return result;
		}

		public override void Focus()
		{
		}

		public override void Unfocus()
		{
		}

		public const int PANEL_DEPTH = 0;

		public const float TAB_WIDTH = 50f;

		public const int MAX_SUPPORTED_PARTY_MEMBERS = 4;

		private const string FILE = "pause.dat";

		private const string FRONT_TAG = "firsttag";

		private const string TAG = "tag";

		public static readonly Vector2f PANEL_POSITION = MainMenu.PANEL_POSITION + new Vector2f(MainMenu.PANEL_SIZE.X + 20f, 13f);

		public static readonly Vector2f PANEL_SIZE = new Vector2f(320f - AUXMenu.PANEL_POSITION.X - 20f, 99f);

		public static readonly Color ACTIVE_TAB_TEXT_COLOR = Color.Black;

		public static readonly Color INACTIVE_TAB_TEXT_COLOR = new Color(65, 80, 79);

		public static readonly Color DIVIDER_COLOR = new Color(128, 140, 138);

		private static readonly string CURSOR_FILE = Paths.GRAPHICS + "realcursor.dat";

		private static readonly string[] AUX_TYPE_STRINGS = new string[]
		{
			"Offense", //StringFile.Instance.Get("AUX.offense").Value,
			"Recovery",//StringFile.Instance.Get("AUX.recovery").Value,
			"Support",//StringFile.Instance.Get("AUX.support").Value,
			"Ecifircas", //StringFile.Instance.Get("AUX.other").Value
		};

		private static readonly string[] AUX_LEVEL_STRINGS = new string[]
		{
			"α",
			"β",
			"γ",
			"Ω"
		};

		private ShapeGraphic horizDivider;

		private ShapeGraphic vertDivider;

		private IndexedColorGraphic[] tabs;

		private TextRegion[] tabLabels;

		private int selectedTab;

		private ScrollingList AUXTypeList;

		private ScrollingList AUXList;

		private ScrollingList selectedList;

		private ScrollingList[] levelList;

		private int selectedLevel;

		private List<IAUX> AUXItemList;

		private TextRegion descriptionText;
	}
}

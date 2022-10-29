using System;
using System.Collections.Generic;
using Violet.Actors;
using Violet.Graphics;
using Violet.GUI;
using VCO.Data;
using VCO.AUX;
using Rufini.Strings;
using SFML.Graphics;
using SFML.System;

namespace VCO.GUI
{

	internal class SectionedAUXBox : Actor
	{
		public IEnumerable<OffenseAUX> OffenseAUXItems
		{
			get
			{
				return this.offenseAUXItems;
			}
			set
			{
				this.offenseAUXItems = new List<OffenseAUX>(value);
			}
		}

		public IEnumerable<DefensiveAUX> DefenseAUXItems
		{
			get
			{
				return this.defenseAUXItems;
			}
			set
			{
				this.defenseAUXItems = new List<DefensiveAUX>(value);
			}
		}

		public IEnumerable<AssistiveAUX> AssistAUXItems
		{
			get
			{
				return this.assistAUXItems;
			}
			set
			{
				this.assistAUXItems = new List<AssistiveAUX>(value);
			}
		}

		public IEnumerable<OtherAUX> OtherAUXItems
		{
			get
			{
				return this.otherAUXItems;
			}
			set
			{
				this.otherAUXItems = new List<OtherAUX>(value);
			}
		}

		public SectionedAUXBox(RenderPipeline pipeline, int depth, float lineHeight)
		{
			this.pipeline = pipeline;
			this.currentSelection = 0;
			this.currentTopLevelSelection = 0;
			this.currentSelectionLevel = 0;
			this.firstVisibleIndex = 0;
			this.lastVisibleIndex = 2;
			this.states = new RenderStates(BlendMode.Alpha);
			this.visible = false;
			this.depth = depth;
			this.lineHeight = lineHeight;
			this.AUXTypes = new List<TextRegion>(4);
			this.activeAUXList = new List<TextRegion>();
			this.activeAlphaList = new List<TextRegion>();
			this.activeBetaList = new List<TextRegion>();
			this.activeGammaList = new List<TextRegion>();
			this.activeOmegaList = new List<TextRegion>();
			this.windowPosition = new Vector2f(40f, 0f);
			this.window = new WindowBox(Settings.WindowStyle, Settings.WindowFlavor, this.windowPosition, new Vector2f(240f, 3f * lineHeight + 16f), 32766);
			this.selectorFillColor = UIColors.HighlightColor;

			//RectangleShape rectangleShape = new RectangleShape(new Vector2f(48f, lineHeight  ));
			//rectangleShape.FillColor = this.selectorFillColor;
			RectangleShape rectangleShape = new RectangleShape(new Vector2f(48,/* 11 * 1.3f - ScrollingList.SELECT_RECT_OFFSET.Y * 2f*/ lineHeight) - ScrollingList.SELECT_RECT_SIZE_OFFSET);
			rectangleShape.FillColor = UIColors.HighlightColor;

			this.selectorBox = new ShapeGraphic(rectangleShape, default(Vector2f), default(Vector2f), rectangleShape.Size, 32767);
			RectangleShape rectangleShape2 = new RectangleShape(new Vector2f(2f, 3f * lineHeight - 2f));
			rectangleShape2.FillColor = new Color(byte.MaxValue, byte.MaxValue, byte.MaxValue, 70);


			this.separator = new ShapeGraphic(rectangleShape2, new Vector2f(102.399994f, lineHeight - 8f), default(Vector2f), rectangleShape2.Size, 32767);
			this.cursor = new IndexedColorGraphic(Paths.GRAPHICS + "realcursor.dat", "right", default(Vector2f), 32767);
			this.nucursor = new IndexedColorGraphic(Paths.GRAPHICS + "realcursor.dat", "right", default(Vector2f), 32767);
			this.states = new RenderStates(BlendMode.Alpha);
			//pipeline.Add(cursor);
		}


		internal OffenseAUX SelectOffenseAUX()
		{

			return this.offenseAUXItems.Find((OffenseAUX s) => s.aux.QualifiedName == this.activeAUXList[this.currentSelection].Text);
		}

		internal AssistiveAUX SelectAssistAUX()
		{
			return this.assistAUXItems.Find((AssistiveAUX s) => s.aux.QualifiedName == this.activeAUXList[this.currentSelection].Text);
		}

		internal DefensiveAUX SelectDefenseAUX()
		{
			return this.defenseAUXItems.Find((DefensiveAUX s) => s.aux.QualifiedName == this.activeAUXList[this.currentSelection].Text);
		}

		internal OtherAUX SelectOtherAUX()
		{
			return this.otherAUXItems.Find((OtherAUX s) => s.aux.QualifiedName == this.activeAUXList[this.currentSelection].Text);
		}

		internal bool InTypeSelection()
		{
			return this.currentSelectionLevel == 0;
		}
		public AUXType SelectedAUXType()
		{
			string text;
			if ((text = this.AUXTypes[this.currentTopLevelSelection].Text) != null)
			{
				if (text == "Offense")
				{
					return AUXType.Offense;
				}
				if (text == "Recovery")
				{
					return AUXType.Assist;
				}
				if (text == "Support")
				{
					return AUXType.Defense;
				}
				if (text == "Other")
				{
					return AUXType.Other;
				}
			}
			throw new MissingMemberException("No AUX Type selected");
		}

		internal int SelectedLevel()
		{
			return this.currentSelectionLevel - 1;
		}

		internal void Reset()
		{
			if (this.offenseAUXItems != null)
			{
				this.offenseAUXItems.Clear();
			}
			if (this.defenseAUXItems != null)
			{
				this.defenseAUXItems.Clear();
			}
			if (this.assistAUXItems != null)
			{
				this.assistAUXItems.Clear();
			}
			if (this.otherAUXItems != null)
			{
				this.otherAUXItems.Clear();
			}
			this.currentSelection = 0;
			this.currentSelectionLevel = 0;
			this.currentTopLevelSelection = 0;
		}

		internal void Show()
		{
			if (!this.visible)
			{
				this.visible = true;
				this.pipeline.Add(this.window);
				if (this.offenseAUXItems.Count != 0)
				{
					this.AddAUXType(StringFile.Instance.Get("psi.offense").Value);
				}
				if (this.assistAUXItems.Count != 0)
				{
					this.AddAUXType(StringFile.Instance.Get("psi.recovery").Value);
				}
				if (this.defenseAUXItems.Count != 0)
				{
					this.AddAUXType(StringFile.Instance.Get("psi.support").Value);
				}
				if (this.otherAUXItems.Count != 0)
				{
					this.AddAUXType("Ecifircas");
				}
				foreach (TextRegion renderable in this.AUXTypes)
				{
					this.pipeline.Add(renderable);
				}
				if (this.AUXTypes.Count != 0)
				{
					this.UpdateActiveAbilityList();
					this.UpdateSelectorBox();
					this.UpdateCursor();
					this.pipeline.Add(this.selectorBox);
					this.pipeline.Add(this.cursor);
					this.pipeline.Add(this.nucursor);
				}
				this.pipeline.Add(this.separator);
			}
		}

		internal void Hide()
		{
			if (this.visible)
			{
				this.visible = false;
				this.pipeline.Remove(this.window);
				this.ClearListFromPipeline<TextRegion>(this.AUXTypes);
				this.ClearListFromPipeline<TextRegion>(this.activeAUXList);
				this.ClearListFromPipeline<TextRegion>(this.activeAlphaList);
				this.ClearListFromPipeline<TextRegion>(this.activeBetaList);
				this.ClearListFromPipeline<TextRegion>(this.activeGammaList);
				this.ClearListFromPipeline<TextRegion>(this.activeOmegaList);
				this.pipeline.Remove(this.selectorBox);
				this.pipeline.Remove(this.cursor);
				this.pipeline.Remove(this.separator);
				this.pipeline.Remove(nucursor);
				this.AUXTypes.Clear();
				this.activeAUXList.Clear();
				this.pipeline.Target.Clear();
			}
		}


		internal void SelectDown()
		{
			if (this.currentSelectionLevel == 0)
			{
				this.AUXTypes[this.currentTopLevelSelection].Color = Color.White;
				this.currentTopLevelSelection = (this.currentTopLevelSelection + 1) % this.AUXTypes.Count;
				this.currentSelection = 0;
				this.firstVisibleIndex = 0;
				this.lastVisibleIndex = 2;
				this.UpdateActiveAbilityList();
			}
			else if (this.currentSelectionLevel >= 1)
			{
				this.currentSelection = (this.currentSelection + 1) % this.activeAUXList.Count;
				if (this.currentSelection > this.lastVisibleIndex)
				{
					this.lastVisibleIndex = this.currentSelection;
					this.firstVisibleIndex = this.lastVisibleIndex - 3 + 1;
					this.UpdateActiveAbilityList();
				}
				else if (this.currentSelection < this.firstVisibleIndex)
				{
					this.firstVisibleIndex = this.currentSelection;
					this.lastVisibleIndex = this.currentSelection + 3 - 1;
					this.UpdateActiveAbilityList();
				}
				if (this.currentSelectionLevel == 2)
				{
					if (this.activeBetaList[this.currentSelection] == null && this.activeOmegaList[this.currentSelection] == null)
					{
						this.currentSelectionLevel = 1;
					}
				}
				else if (this.currentSelectionLevel == 3)
				{
					if (this.activeBetaList[this.currentSelection] == null)
					{
						this.currentSelectionLevel = 1;
					}
					else if (this.activeGammaList[this.currentSelection] == null)
					{
						this.currentSelectionLevel = 2;
					}
				}
				else if (this.activeBetaList[this.currentSelection] == null)
				{
					this.currentSelectionLevel = 1;
				}
				else if (this.activeGammaList[this.currentSelection] == null)
				{
					this.currentSelectionLevel = 2;
				}
				else if (this.activeOmegaList[this.currentSelection] == null)
				{
					this.currentSelectionLevel = 3;
				}
			}
			this.UpdateSelectorBox();
		}

		internal void SelectUp()
		{
			if (this.currentSelectionLevel == 0)
			{
				this.AUXTypes[this.currentTopLevelSelection].Color = Color.White;
				this.currentTopLevelSelection--;
				if (this.currentTopLevelSelection < 0)
				{
					this.currentTopLevelSelection = this.AUXTypes.Count - 1;
				}
				this.currentSelection = 0;
				this.firstVisibleIndex = 0;
				this.lastVisibleIndex = 2;
				this.UpdateActiveAbilityList();
			}
			else if (this.currentSelectionLevel >= 1)
			{
				this.currentSelection--;
				if (this.currentSelection < 0)
				{
					this.currentSelection = this.activeAUXList.Count - 1;
				}
				if (this.currentSelection < this.firstVisibleIndex)
				{
					this.firstVisibleIndex = this.currentSelection;
					this.lastVisibleIndex = this.currentSelection + 3 - 1;
					this.UpdateActiveAbilityList();
				}
				else if (this.currentSelection > this.lastVisibleIndex)
				{
					this.lastVisibleIndex = this.currentSelection;
					this.firstVisibleIndex = this.lastVisibleIndex - 3 + 1;
					this.UpdateActiveAbilityList();
				}
				if (this.currentSelectionLevel == 2)
				{
					if (this.activeBetaList[this.currentSelection] == null && this.activeOmegaList[this.currentSelection] == null)
					{
						this.currentSelectionLevel = 1;
					}
				}
				else if (this.currentSelectionLevel == 3)
				{
					if (this.activeBetaList[this.currentSelection] == null)
					{
						this.currentSelectionLevel = 1;
					}
					else if (this.activeGammaList[this.currentSelection] == null)
					{
						this.currentSelectionLevel = 2;
					}
				}
				else if (this.activeBetaList[this.currentSelection] == null)
				{
					this.currentSelectionLevel = 1;
				}
				else if (this.activeGammaList[this.currentSelection] == null)
				{
					this.currentSelectionLevel = 2;
				}
				else if (this.activeOmegaList[this.currentSelection] == null)
				{
					this.currentSelectionLevel = 3;
				}
			}
			this.UpdateSelectorBox();
		}


		internal void SelectRight()
		{
			switch (this.currentSelectionLevel)
			{
				case 0:
					this.AUXTypes[this.currentTopLevelSelection].Color = Color.White;
					this.currentSelectionLevel = 1;
					this.UpdateSelectorBox();
					return;
				case 1:
					if (this.activeBetaList[this.currentSelection] != null || this.activeOmegaList[this.currentSelection] != null)
					{
						this.currentSelectionLevel = 2;
						this.UpdateSelectorBox();
						return;
					}
					break;
				case 2:
					if (this.activeGammaList[this.currentSelection] != null)
					{
						this.currentSelectionLevel = 3;
						this.UpdateSelectorBox();
						return;
					}
					break;
				case 3:
					if (this.activeOmegaList[this.currentSelection] != null)
					{
						this.currentSelectionLevel = 4;
						this.UpdateSelectorBox();
					}
					break;
				default:
					return;
			}
		}

		internal void SelectLeft()
		{
			if (this.currentSelectionLevel > 0)
			{
				this.currentSelectionLevel--;
				this.UpdateSelectorBox();
			}
		}

		private void AddAUXType(string name)
		{
			this.AUXTypes.Add(new TextRegion(new Vector2f(this.windowPosition.X + 12f, this.windowPosition.Y + 5f + this.lineHeight * (float)this.AUXTypes.Count), 32768, Fonts.Main, name));
		}

		private void AddAUXForCurrentSelection()
		{
			string text;
			if ((text = this.AUXTypes[this.currentTopLevelSelection].Text) != null)
			{
				if (text == "Offense")
				{
					this.AddAUXAbilityFromList<OffenseAUX>(this.offenseAUXItems);
					return;
				}
				if (text == "Recovery")
				{
					this.AddAUXAbilityFromList<AssistiveAUX>(this.assistAUXItems);
					return;
				}
				if (text == "Support")
				{

					this.AddAUXAbilityFromList<DefensiveAUX>(this.defenseAUXItems);
					return;
				}
				if (text == "Ecifircas")
				{

					this.AddAUXAbilityFromList<OtherAUX>(this.otherAUXItems);
					return;
				}
				if (!(text == "Other"))
				{
					return;
				}
				this.AddAUXAbilityFromList<OtherAUX>(this.otherAUXItems);
			}
		}

		private void AddAUXAbilityFromList<T>(List<T> AUXList) where T : IAUX
		{
			for (int i = 0; i < AUXList.Count; i++)
			{
				IAUX AUX = AUXList[i];
				if (AUX.aux.Symbols[0] <= this.MaxLevel)
				{
					if (i < this.firstVisibleIndex || i > this.lastVisibleIndex)
					{
						this.activeAUXList.Add(null);
						this.activeAlphaList.Add(null);
						this.activeBetaList.Add(null);
						this.activeGammaList.Add(null);
						this.activeOmegaList.Add(null);
					}
					else
					{
						/*	private static readonly string[] AUX_LEVEL_STRINGS = new string[]
	{
		"α",
		"β",
		"γ",
		"Ω"
	};*/

						this.activeAlphaList.Add(new TextRegion(new Vector2f(200f, this.windowPosition.Y + 5f + this.lineHeight * (float)(this.activeAUXList.Count - this.firstVisibleIndex)), 32768, Fonts.Main, "α"));
						if (AUX.aux.Symbols.Length == 4)
						{
							if (AUX.aux.Symbols[1] < this.MaxLevel)
							{
								this.activeBetaList.Add(new TextRegion(new Vector2f(220f, this.windowPosition.Y + 5f + this.lineHeight * (float)(this.activeAUXList.Count - this.firstVisibleIndex)), 32768, Fonts.Main, "β"));
								if (AUX.aux.Symbols[2] < this.MaxLevel)
								{
									this.activeGammaList.Add(new TextRegion(new Vector2f(240f, this.windowPosition.Y + 5f + this.lineHeight * (float)(this.activeAUXList.Count - this.firstVisibleIndex)), 32768, Fonts.Main, "γ"));
									if (AUX.aux.Symbols[3] < this.MaxLevel)
									{
										this.activeOmegaList.Add(new TextRegion(new Vector2f(260f, this.windowPosition.Y + 5f + this.lineHeight * (float)(this.activeAUXList.Count - this.firstVisibleIndex)), 32768, Fonts.Main, "Ω"));
									}
									else
									{
										this.activeOmegaList.Add(null);
									}
								}
								else
								{
									this.activeGammaList.Add(null);
									this.activeOmegaList.Add(null);
								}
							}
							else
							{
								this.activeBetaList.Add(null);
								this.activeGammaList.Add(null);
								this.activeOmegaList.Add(null);
							}
						}
						else if (AUX.aux.Symbols.Length == 2)
						{
							if (AUX.aux.Symbols[1] < this.MaxLevel)
							{
								this.activeOmegaList.Add(new TextRegion(new Vector2f(220f, this.windowPosition.Y + 5f + this.lineHeight * (float)(this.activeAUXList.Count - this.firstVisibleIndex)), 32768, Fonts.Main, "Ω"));
							}
							else
							{
								this.activeOmegaList.Add(null);
							}
							this.activeBetaList.Add(null);
							this.activeGammaList.Add(null);
						}
						this.activeAUXList.Add(new TextRegion(new Vector2f(120f, this.windowPosition.Y + 5f + this.lineHeight * (float)(this.activeAUXList.Count - this.firstVisibleIndex)), 32768, Fonts.Main, AUX.aux.QualifiedName));
					}
				}
			}
		}

		private void UpdateSelectorBox()
		{
			if (this.currentSelectionLevel == 0)
			{
				this.nucursor.Position = new Vector2f(this.AUXTypes[this.currentTopLevelSelection].Position.X, //- 2f,
					this.AUXTypes[this.currentTopLevelSelection].Position.Y + 5f);

				this.selectorBox.Position = new Vector2f(this.AUXTypes[this.currentTopLevelSelection].Position.X - 1f, this.AUXTypes[this.currentTopLevelSelection].Position.Y - 1f);
				this.AUXTypes[this.currentTopLevelSelection].Color = Color.Black;
				this.cursor.Visible = false;
				return;
			}
			if (this.currentSelectionLevel == 1)
			{
				this.cursor.Visible = true;

				try
				{


					this.cursor.Position = new Vector2f(this.activeAlphaList[this.currentSelection].Position.X - 3f,
						this.activeAlphaList[this.currentSelection].Position.Y + 8f);
				}
				catch (Exception e)
				{
					this.selectorBox.Position = new Vector2f(this.AUXTypes[this.currentTopLevelSelection].Position.X - 1f, this.AUXTypes[this.currentTopLevelSelection].Position.Y + 3f);
					this.AUXTypes[this.currentTopLevelSelection].Color = Color.Black;
					this.cursor.Visible = false;
					currentSelectionLevel = 0;
				}

				return;
			}
			if (this.currentSelectionLevel == 2)
			{
				if (this.activeBetaList[this.currentSelection] != null)
				{
					this.cursor.Position = new Vector2f(this.activeBetaList[this.currentSelection].Position.X - 3f, this.activeBetaList[this.currentSelection].Position.Y + 8f);
					return;
				}
				this.cursor.Position = new Vector2f(this.activeOmegaList[this.currentSelection].Position.X - 3f, this.activeOmegaList[this.currentSelection].Position.Y + 8f);
				return;
			}
			else
			{
				if (this.currentSelectionLevel == 3)
				{
					this.cursor.Position = new Vector2f(this.activeGammaList[this.currentSelection].Position.X - 3f, this.activeGammaList[this.currentSelection].Position.Y + 8f);
					return;
				}
				this.cursor.Position = new Vector2f(this.activeOmegaList[this.currentSelection].Position.X - 3f, this.activeOmegaList[this.currentSelection].Position.Y + 8f);
				return;
			}
		}
		private void UpdateCursor()
		{
			int num = this.currentSelectionLevel;
		}

		private void UpdateActiveAbilityList()
		{
			this.ClearListFromPipeline<TextRegion>(this.activeAUXList);
			this.ClearListFromPipeline<TextRegion>(this.activeAlphaList);
			this.ClearListFromPipeline<TextRegion>(this.activeBetaList);
			this.ClearListFromPipeline<TextRegion>(this.activeGammaList);
			this.ClearListFromPipeline<TextRegion>(this.activeOmegaList);
			this.AddAUXForCurrentSelection();
			for (int i = this.firstVisibleIndex; i < Math.Min(this.activeAUXList.Count, this.lastVisibleIndex + 1); i++)
			{
				this.pipeline.Add(this.activeAUXList[i]);
				this.pipeline.Add(this.activeAlphaList[i]);
				if (this.activeBetaList[i] != null)
				{
					this.pipeline.Add(this.activeBetaList[i]);
				}
				if (this.activeGammaList[i] != null)
				{
					this.pipeline.Add(this.activeGammaList[i]);
				}
				if (this.activeOmegaList[i] != null)
				{
					this.pipeline.Add(this.activeOmegaList[i]);
				}
			}
		}

		private void ClearListFromPipeline<T>(List<T> list) where T : Renderable
		{
			foreach (T t in list)
			{
				if (t != null)
				{
					this.pipeline.Remove(t);
				}
			}
			list.Clear();
		}

		private const int SCROLL_LIMIT = 3;

		private readonly WindowBox window;

		private readonly ShapeGraphic selectorBox;

		private readonly Vector2f windowPosition;

		private readonly ShapeGraphic separator;

		private readonly IndexedColorGraphic cursor;
		private readonly IndexedColorGraphic nucursor;

		private int depth;

		private float lineHeight;

		private bool visible;

		private int currentSelection;

		private int currentTopLevelSelection;

		private int currentSelectionLevel;

		private int firstVisibleIndex;

		private int lastVisibleIndex;

		private RenderStates states;

		private Color selectorFillColor;

		private List<TextRegion> AUXTypes;

		private List<TextRegion> activeAUXList;

		private List<TextRegion> activeAlphaList;

		private List<TextRegion> activeBetaList;

		private List<TextRegion> activeGammaList;

		private List<TextRegion> activeOmegaList;

		private List<OffenseAUX> offenseAUXItems;

		private List<DefensiveAUX> defenseAUXItems;

		private List<AssistiveAUX> assistAUXItems;

		private List<OtherAUX> otherAUXItems;

		public int MaxLevel;

		protected readonly RenderPipeline pipeline;
	}
}

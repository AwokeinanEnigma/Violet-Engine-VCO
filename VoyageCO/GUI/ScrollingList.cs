using SFML.Graphics;
using SFML.System;
using System;
using VCO.Data;
using Violet.Graphics;
using Violet.GUI;
using Violet.Utility;

namespace VCO.GUI
{
    internal class ScrollingList : Renderable
    {
        #region Properties
        public int SelectedIndex
        {
            get => this.selectedIndex;
            set => this.Select(value);
        }

        public string SelectedItem => this.items[this.selectedIndex];

        public bool Enabled
        {
            get => this.enabled;
            set => this.enabled = value;
        }
        public bool ShowArrows
        {
            get => this.showArrows;
            set => this.showArrows = value;
        }

        public bool ShowSelectionRectangle
        {
            get => this.showSelectRect;
            set
            {
                this.showSelectRect = value;
                this.UpdateCursor();
            }
        }
        public bool UseHighlightTextColor
        {
            get => this.useHighlightTextColor;
            set
            {
                this.useHighlightTextColor = value;
                this.UpdateCursor();
            }
        }
        public bool ShowCursor
        {
            get => this.showCursor;
            set
            {
                this.showCursor = value;
                this.UpdateCursor();
            }
        }
        public bool Focused
        {
            get => this.focused;
            set
            {
                this.focused = value;
                this.UpdateCursor();
                this.UpdateScrollers();
            }
        }
        public int Count => this.items.Length;

        public override Vector2f Position
        {
            get => this.position;
            set
            {
                this.position = value;
                for (int i = 0; i < this.texts.Length; i++)
                {
                    this.texts[i].Position = new Vector2f(this.position.X, this.position.Y + this.lineHeight * i);
                }
                this.UpdateCursor();
                this.upArrow.Position = this.position + new Vector2f(this.width, 0f);
                this.downArrow.Position = this.position + new Vector2f(this.width, this.lineHeight * displayCount + 1f);
            }
        }
        #endregion

        #region Fields
        private const int CURSOR_MARGIN = 1;
        
        public static readonly Vector2f SELECT_RECT_OFFSET = new Vector2f(-2f, 0f);
        public static readonly Vector2f SELECT_RECT_SIZE_OFFSET = new Vector2f(-2f, 0f);
        private static readonly Color FOCUSED_TEXT_COLOR = Color.White;
        private static readonly Color UNFOCUSED_TEXT_COLOR = new Color(128, 140, 138);
        
        private readonly string[] items;
        
        private readonly int displayCount;
        private int selectedIndex;
        private int topIndex;

        private readonly float lineHeight;
        private readonly float width;
        private readonly TextRegion[] texts;

        private readonly IndexedColorGraphic cursor;
        private readonly IndexedColorGraphic upArrow;
        private readonly IndexedColorGraphic downArrow;

        private readonly ShapeGraphic selectRectangle;

        private bool enabled;
        private bool enabledOnHide;
        private bool showArrows;
        private bool showSelectRect = true;
        private bool showCursor = true;
        private bool useHighlightTextColor = true;
        private bool focused = true;

        private readonly int cursorOffset;
#endregion

        public ScrollingList(Vector2f position, int depth, string[] items, int displayCount, float lineHeight, float width, string cursorGraphic)
        {
            if (items == null)
            {
                throw new ArgumentNullException("List item array cannot be null.");
            }
            if (items.Length == 0)
            {
                throw new ArgumentException("List item array cannot be empty.");
            }
            this.position = position;
            this.origin = VectorMath.ZERO_VECTOR;
            this.items = items;
            this.displayCount = displayCount;
            this.lineHeight = lineHeight;
            this.width = width;
            this.size = new Vector2f(this.width, this.lineHeight * this.displayCount);
            this.showArrows = true;
            this.showCursor = true;
            this.enabled = true;
            for (int i = 0; i < this.items.Length; i++)
            {
                if (this.items[i] == null)
                {
                    this.items[i] = string.Empty;
                }
            }
            int num = Math.Min(items.Length, displayCount);
            this.texts = new TextRegion[num];
            for (int j = 0; j < num; j++)
            {
                this.texts[j] = new TextRegion(new Vector2f(position.X, position.Y + lineHeight * j), depth, Fonts.Main, items[j]);
            }
            this.cursor = new IndexedColorGraphic(cursorGraphic, "right", this.texts[0].Position, depth);
            this.upArrow = new IndexedColorGraphic(cursorGraphic, "up", position + new Vector2f(width, 0f), depth);
            this.downArrow = new IndexedColorGraphic(cursorGraphic, "down", position + new Vector2f(width, lineHeight * displayCount + 1f), depth);
            RectangleShape rectangleShape = new RectangleShape(new Vector2f(this.width, 11 * 1.3f - ScrollingList.SELECT_RECT_OFFSET.Y * 2f) - ScrollingList.SELECT_RECT_SIZE_OFFSET)
            {
                FillColor = UIColors.HighlightColor
            };
            this.selectRectangle = new ShapeGraphic(rectangleShape, this.texts[0].Position + ScrollingList.SELECT_RECT_OFFSET, VectorMath.ZERO_VECTOR, rectangleShape.Size, this.depth - 1)
            {
                Visible = this.showSelectRect
            };
            this.cursorOffset = (Fonts.Main.WHeight - (int)this.lineHeight) / 2;
            this.UpdateCursor();
            this.UpdateScrollers();
        }
        private void SetVisibility(bool visible)
        {
            for (int i = 0; i < this.texts.Length; i++)
            {
                this.texts[i].Visible = visible;
            }
            this.cursor.Visible = visible;
            this.downArrow.Visible = visible;
            this.upArrow.Visible = visible;
            this.selectRectangle.Visible = (visible && this.showSelectRect);
        }
        public void Hide()
        {
            this.enabledOnHide = this.enabled;
            this.enabled = false;
            this.SetVisibility(false);
        }
        public void Show()
        {
            this.enabled = this.enabledOnHide;
            this.SetVisibility(true);
            this.UpdateScrollers();
        }
        public bool SelectPrevious()
        {
            if (this.enabled && this.selectedIndex - 1 >= 0)
            {
                this.selectedIndex--;
                if (this.selectedIndex < this.topIndex)
                {
                    this.topIndex--;
                    this.UpdateDisplayTexts();
                    this.UpdateScrollers();
                }
                this.UpdateCursor();
                return true;
            }
            return false;
        }
        public bool SelectNext()
        {
            if (this.enabled && this.selectedIndex + 1 < this.items.Length)
            {
                this.selectedIndex++;
                if (this.selectedIndex > this.topIndex + this.displayCount - 1)
                {
                    this.topIndex++;
                    this.UpdateDisplayTexts();
                    this.UpdateScrollers();
                }
                this.UpdateCursor();
                return true;
            }
            return false;
        }
        private void Select(int i)
        {
            this.selectedIndex = Math.Min(this.items.Length - 1, Math.Max(0, i));
            this.topIndex = Math.Min(this.selectedIndex, Math.Max(0, this.items.Length - this.displayCount));
            this.UpdateDisplayTexts();
            this.UpdateScrollers();
            this.UpdateCursor();
        }
        public void ChangeItem(int index, string newValue)
        {
            if (index >= 0 && index < this.items.Length)
            {
                this.items[index] = newValue;
                this.UpdateDisplayTexts();
                return;
            }
            throw new ArgumentException("Item index out of range.");
        }
        private void UpdateDisplayTexts()
        {
            for (int i = 0; i < this.displayCount; i++)
            {
                int num = this.topIndex + i;
                if (num < this.items.Length)
                {
                    string text = this.items[num];
                    this.texts[i].Reset(text, 0, text.Length);
                }
                else if (i < this.texts.Length)
                {
                    this.texts[i].Reset(string.Empty, 0, 0);
                }
            }
        }
        private void UpdateCursor()
        {
            this.cursor.Visible = (this.focused && this.showCursor);
            this.cursor.Position = new Vector2f(this.position.X - 1f, (this.position.Y + this.lineHeight * (this.selectedIndex - this.topIndex) + 11 - this.cursor.Size.Y / 2f) + -1);
            //Console.WriteLine($"LineWeight is '{lineHeight}' & selectedindexfloat is '{this.selectedIndex - this.topIndex}' & WHeight is {(float)Fonts.Main.WHeight}");
            //Console.WriteLine($"Visible is '{cursor.Visible}' & position is '{cursor.Position}' & depth is {cursor.Depth}");
            for (int i = 0; i < this.texts.Length; i++)
            {
                this.texts[i].Color = (this.focused ? ScrollingList.FOCUSED_TEXT_COLOR : ScrollingList.UNFOCUSED_TEXT_COLOR);
            }
            if (this.showSelectRect)
            {
                this.selectRectangle.Position = this.texts[this.selectedIndex - this.topIndex].Position + ScrollingList.SELECT_RECT_OFFSET;
            }
            if (this.useHighlightTextColor)
            {
                this.texts[this.selectedIndex - this.topIndex].Color = new Color(255, 89, 209);
            }
        }
        private void UpdateScrollers()
        {
            bool visible = this.upArrow.Visible;
            bool visible2 = this.downArrow.Visible;
            this.upArrow.Visible = (this.showArrows && this.focused && this.topIndex > 0);
            this.downArrow.Visible = (this.showArrows && this.focused && this.topIndex < this.items.Length - this.displayCount);
            if (this.upArrow.Visible && !visible && this.downArrow.Visible)
            {
                this.upArrow.Frame = this.downArrow.Frame;
            }
            if (this.downArrow.Visible && !visible2 && this.upArrow.Visible)
            {
                this.downArrow.Frame = this.upArrow.Frame;
            }
        }
        public override void Draw(RenderTarget target)
        {
            if (this.selectRectangle.Visible && this.showSelectRect)
            {
                this.selectRectangle.Draw(target);
            }
            for (int i = 0; i < this.texts.Length; i++)
            {
                if (this.texts[i].Visible)
                {
                    this.texts[i].Draw(target);
                }
            }
            if (this.cursor.Visible)
            {
                this.cursor.Draw(target);
            }
            if (this.downArrow.Visible)
            {
                this.downArrow.Draw(target);
            }
            if (this.upArrow.Visible)
            {
                this.upArrow.Draw(target);
            }
        }
        protected override void Dispose(bool disposing)
        {
            if (!this.disposed && disposing)
            {
                for (int i = 0; i < this.texts.Length; i++)
                {
                    this.texts[i].Dispose();
                }
                this.cursor.Dispose();
                this.downArrow.Dispose();
                this.upArrow.Dispose();
            }
            this.disposed = true;
        }
    }
}

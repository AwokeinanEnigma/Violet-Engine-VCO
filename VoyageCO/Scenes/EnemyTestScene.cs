using SFML.System;
using System;
using System.Collections.Generic;
using System.IO;
using VCO.Data.Enemies;
using VCO.GUI;
using Violet.Graphics;
using Violet.Input;
using Violet.Scenes;
using Violet.Utility;

namespace VCO.Scenes
{
    internal class EnemyTestScene : StandardScene
    {
        private void Initialize()
        {
            if (this.isInitialized)
            {
                return;
            }
            this.enemyData = EnemyFile.Instance.GetAllEnemyData();
            string[] array = new string[this.enemyData.Count];
            for (int i = 0; i < array.Length; i++)
            {
                string stringQualifiedName = this.enemyData[i].PlayerFriendlyName;
                array[i] = stringQualifiedName;
            }
            this.enemyList = new ScrollingList(new Vector2f(16f, 6f), 0, array, 12, Fonts.Main.LineHeight, 128f, DataHandler.instance.Load("cursor.dat"));
            this.pipeline.Add(this.enemyList);
            this.SelectList(this.enemyList);
            this.isInitialized = true;
        }
        private void SelectList(ScrollingList list)
        {
            if (this.selectedList != null)
            {
                this.selectedList.Focused = false;
                this.selectedList.ShowSelectionRectangle = false;
                this.selectedList.UseHighlightTextColor = false;
            }
            this.selectedList = list;
            this.selectedList.Focused = true;
            this.selectedList.ShowSelectionRectangle = true;
            this.selectedList.UseHighlightTextColor = true;
        }
        private void SetEnemySprite(string spriteName)
        {
            string text = DataHandler.instance.Load(spriteName + ".dat");
            if (this.enemySprite != null)
            {
                this.pipeline.Remove(this.enemySprite);
                this.enemySprite.Dispose();
                this.enemySprite = null;
            }
            if (File.Exists(text))
            {
                this.enemySprite = new IndexedColorGraphic(text, "front", new Vector2f(232f, 45f), 0);
                this.enemySprite.Origin = VectorMath.Truncate(this.enemySprite.Size / 2f);
                this.pipeline.Add(this.enemySprite);
            }
        }
        private void SetEnemyInfo(EnemyData data)
        {
            this.SetEnemySprite(data.SpriteName);
            if (this.infoList != null)
            {
                this.pipeline.Remove(this.infoList);
                this.infoList.Dispose();
                this.infoList = null;
            }
            string[] array = new string[25];
            int num = 0;
            array[num++] = string.Format("AI Name: {0}", data.AIName);
            array[num++] = string.Format("EXP: {0}", data.Experience);
            //array[num++] = string.Format("Reward: ${0}", data.Reward);
            array[num++] = string.Format("BBG: {0}", data.BackgroundName);
            array[num++] = string.Format("BGM: {0}", data.MusicName);
            array[num++] = string.Format("Options: {0}", data.Options);
            //array[num++] = string.Format("Mover Type: {0}", data.MoverType);
            array[num++] = string.Format("Level: {0}", data.Level);
            array[num++] = string.Format("HP: {0}", data.HP);
            array[num++] = string.Format("PP: {0}", data.PP);
            array[num++] = string.Format("Offense: {0}", data.Offense);
            array[num++] = string.Format("Defense: {0}", data.Defense);
            array[num++] = string.Format("Speed: {0}", data.Speed);
            array[num++] = string.Format("Guts: {0}", data.Guts);
            array[num++] = string.Format("IQ: {0}", data.IQ);
            array[num++] = string.Format("Luck: {0}", data.Luck);
            array[num++] = string.Format("Immunities: {0}", data.Immunities);
            array[num++] = string.Format("Electric Mod: {0:0.00}", data.ModifierElectric);
            array[num++] = string.Format("Explosive Mod: {0:0.00}", data.ModifierExplosive);
            array[num++] = string.Format("Fire Mod: {0:0.00}", data.ModifierFire);
            array[num++] = string.Format("Ice Mod: {0:0.00}", data.ModifierIce);
            array[num++] = string.Format("Nausea Mod: {0:0.00}", data.ModifierNausea);
            array[num++] = string.Format("Physical Mod: {0:0.00}", data.ModifierPhysical);
            array[num++] = string.Format("Poison Mod: {0:0.00}", data.ModifierPoison);
            array[num++] = string.Format("Water Mod: {0:0.00}", data.ModifierWater);
            this.infoList = new ScrollingList(new Vector2f(160f, 90f), 10, array, 6, Fonts.Main.LineHeight, 144f, DataHandler.instance.Load("cursor.dat"));
            this.pipeline.Add(this.infoList);
            this.SelectList(this.infoList);
        }
        private void ButtonPressed(InputManager sender, Button b)
        {
            if (this.selectedList == this.enemyList)
            {
                if (b == Button.A)
                {
                    Console.WriteLine(this.enemyData[this.enemyList.SelectedIndex].QualifiedName);
                    this.SetEnemyInfo(this.enemyData[this.enemyList.SelectedIndex]);
                    return;
                }
                if (b == Button.B)
                {
                    SceneManager.Instance.Pop();
                    return;
                }
            }
            else if (this.selectedList == this.infoList && b == Button.B)
            {
                this.SelectList(this.enemyList);
            }
        }
        private void AxisPressed(InputManager sender, Vector2f axis)
        {
            if (axis.Y < 0f)
            {
                this.selectedList.SelectPrevious();
            }
            else if (axis.Y > 0f)
            {
                this.selectedList.SelectNext();
            }
            if (axis.X > 0f)
            {
                if (this.infoList != null)
                {
                    this.SelectList(this.infoList);
                    return;
                }
            }
            else if (axis.X < 0f)
            {
                this.SelectList(this.enemyList);
            }
        }
        public override void Focus()
        {
            base.Focus();
            this.Initialize();
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
            if (!this.disposed && disposing)
            {
                this.enemyList.Dispose();
            }
            base.Dispose(disposing);
        }
        private bool isInitialized;
        private List<EnemyData> enemyData;
        private ScrollingList selectedList;
        private ScrollingList enemyList;
        private ScrollingList infoList;
        private IndexedColorGraphic enemySprite;
    }
}

﻿using SFML.System;
using System.Collections.Generic;
using System.IO;
using Violet.Graphics;
using Violet.Input;

namespace VCO.Scenes
{
    internal class AUXTestScene : StandardScene
    {
        public AUXTestScene()
        {
            this.animations = new List<MultipartAnimation>();
            IEnumerable<string> enumerable = Directory.EnumerateFiles(Paths.AUX_GRAPHICS);
            foreach (string resource in enumerable)
            {
                MultipartAnimation multipartAnimation = new MultipartAnimation(resource, new Vector2f(160f, 90f), 0.5f, 0)
                {
                    Visible = false
                };
                multipartAnimation.OnAnimationComplete += this.AnimationComplete;
                this.pipeline.Add(multipartAnimation);
                this.animations.Add(multipartAnimation);
            }
            this.animIndex = 0;
            this.animations[this.animIndex].Visible = true;
        }

        private void AnimationComplete(MultipartAnimation anim)
        {
            anim.Visible = false;
        }

        private void ButtonPressed(InputManager sender, Button b)
        {
            if (b == Button.A)
            {
                this.animations[this.animIndex].Visible = false;
                this.animIndex = (this.animIndex + 1) % this.animations.Count;
                this.animations[this.animIndex].Visible = true;
                this.animations[this.animIndex].Reset();
            }
        }

        public override void Focus()
        {
            base.Focus();
            InputManager.Instance.ButtonPressed += this.ButtonPressed;
        }

        public override void Update()
        {
            base.Update();
        }

        public override void Unfocus()
        {
            base.Unfocus();
            InputManager.Instance.ButtonPressed -= this.ButtonPressed;
        }

        protected override void Dispose(bool disposing)
        {
            if (!this.disposed && disposing)
            {
                foreach (MultipartAnimation multipartAnimation in this.animations)
                {
                    multipartAnimation.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        private readonly List<MultipartAnimation> animations;

        private int animIndex;
    }
}

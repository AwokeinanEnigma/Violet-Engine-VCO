using SFML.Graphics;
using SFML.System;
using VCO.Data;
using Violet.Graphics;

namespace VCO.GUI.NamingMenu
{
    internal class NamingCharacter : Renderable
    {
        public CharacterType Character
        {
            get => this.character;
            set => this.character = value;
        }
        public NamingCharacter(CharacterType initialCharacter, int depth)
        {
            this.visible = true;
            this.state = NamingCharacter.State.WalkIn;
            this.character = initialCharacter;
            this.depth = depth;
            this.SetSprite(this.character);
            this.shadowGraphic = new IndexedColorGraphic(DataHandler.instance.Load("shadow.dat"), ShadowSize.GetSubsprite(this.graphic.Size), this.position, this.depth - 1);
        }
        public void SwitchCharacters(CharacterType newCharacter)
        {
            this.nextCharacter = newCharacter;
            this.state = NamingCharacter.State.TurnToLeft;
            this.timer = 0;
        }
        private void SetSprite(CharacterType newCharacter)
        {
            this.character = newCharacter;
            this.position = new Vector2f(-30f, 46f);
            if (this.graphic != null)
            {
                this.graphic.Dispose();
            }
            this.graphic = new IndexedColorGraphic(CharacterGraphics.GetFile(this.character), "walk east", this.position, this.depth);
            this.size = this.graphic.Size;
            this.origin = this.graphic.Origin;
        }
        public void Update()
        {
            this.previousPosition = this.position;
            switch (this.state)
            {
                case NamingCharacter.State.WalkIn:
                    this.WalkIn();
                    break;
                case NamingCharacter.State.TurnToFront:
                    this.TurnToFront();
                    break;
                case NamingCharacter.State.Idle:
                    this.Idle();
                    break;
                case NamingCharacter.State.TurnToLeft:
                    this.TurnToLeft();
                    break;
                case NamingCharacter.State.WalkOut:
                    this.WalkOut();
                    break;
            }
            if (this.previousPosition != this.position)
            {
                this.shadowGraphic.Position = this.position;
                this.graphic.Position = this.position;
            }
        }
        private void WalkIn()
        {
            if (this.position.X < 70f)
            {
                this.position.X = this.position.X + 1f;
                return;
            }
            this.position.X = 70f;
            this.timer = 0;
            this.state = NamingCharacter.State.TurnToFront;
        }
        private void TurnToFront()
        {
            this.timer++;
            if (this.timer == 1)
            {
                this.graphic.SetSprite("walk southeast");
                return;
            }
            if (this.timer == 5)
            {
                this.graphic.SetSprite("walk south");
                return;
            }
            if (this.timer == 10)
            {
                this.timer = 0;
                this.state = NamingCharacter.State.Idle;
            }
        }
        private void Idle()
        {
            if (this.timer < 180)
            {
                this.timer++;
                return;
            }
            if (this.timer == 180)
            {
                this.graphic.SetSprite("smoke", true);
                this.graphic.OnAnimationComplete += this.graphic_OnAnimationComplete;
                this.timer++;
            }
        }
        private void graphic_OnAnimationComplete(AnimatedRenderable graphic)
        {
            this.timer = 0;
            this.graphic.SetSprite("walk south", true);
            this.graphic.OnAnimationComplete -= this.graphic_OnAnimationComplete;
        }
        private void TurnToLeft()
        {
            this.timer++;
            if (this.timer == 1)
            {
                this.graphic.SetSprite("walk southwest");
                return;
            }
            if (this.timer == 5)
            {
                this.graphic.SetSprite("walk west");
                return;
            }
            if (this.timer == 10)
            {
                this.timer = 0;
                this.state = NamingCharacter.State.WalkOut;
            }
        }
        private void WalkOut()
        {
            if (this.position.X > -30f)
            {
                this.position.X = this.position.X - 1f;
                return;
            }
            this.position.X = -30f;
            if (this.character != this.nextCharacter)
            {
                this.SetSprite(this.nextCharacter);
                this.state = NamingCharacter.State.WalkIn;
                return;
            }
            this.state = NamingCharacter.State.Wait;
        }
        public override void Draw(RenderTarget target)
        {
            this.shadowGraphic.Draw(target);
            this.graphic.Draw(target);
        }
        private const int STAND_Y = 46;
        private const int STAND_X = 70;
        private const int EXIT_X = -30;
        private const float WALK_SPEED = 1f;
        private NamingCharacter.State state;
        private readonly Graphic shadowGraphic;
        private IndexedColorGraphic graphic;
        private CharacterType character;
        private CharacterType nextCharacter;
        private Vector2f previousPosition;
        private int timer;
        private enum State
        {
            WalkIn,
            TurnToFront,
            Idle,
            TurnToLeft,
            WalkOut,
            Wait
        }
    }
}

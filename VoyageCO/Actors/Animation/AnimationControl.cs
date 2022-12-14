using SFML.System;
using System;
using System.Collections.Generic;
using VCO.Overworld;
using Violet.Graphics;
using Violet.Utility;

namespace VCO.Actors.Animation
{
    internal class AnimationControl
    {
        public bool Overriden => this.isOverriden;

        private IndexedColorGraphic graphic;
        
        private int previousDirection;
        
        private Vector2f previousVelocity;
        
        private AnimationType previousStance;
        
        private AnimationType defaultStance;
        
        private readonly Dictionary<AnimationType, byte> counts;
        
        private bool hasStand;
        
        private bool hasWalk;
        
        private bool hasRun;
        
        private bool hasCrouch;
        
        private bool hasDead;
        
        private bool hasIdle;
        
        private bool hasTalk;
        
        private bool hasBlink;
        
        private bool isOverriden;
        
        private string overrideSubsprite;

        private AnimationType animationType;

        public AnimationControl(IndexedColorGraphic graphic, int initialDirection)
        {
            this.graphic = graphic;
            this.counts = new Dictionary<AnimationType, byte>();
            this.previousVelocity = VectorMath.ZERO_VECTOR;
            this.previousDirection = initialDirection;
            this.previousStance = AnimationType.INVALID;
            this.animationType = (AnimationType)263;
            this.overrideSubsprite = string.Empty;
            this.Initialize();
        }
        private void Initialize()
        {
            foreach (SpriteDefinition spriteDefinition in ((IndexedTexture)this.graphic.Texture).GetSpriteDefinitions())
            {
                AnimationType animationType = AnimationNames.GetAnimationType(spriteDefinition.Name.ToLowerInvariant());
                if (animationType != AnimationType.INVALID)
                {
                    AnimationType key = animationType & AnimationType.STANCE_MASK;
                    if (this.defaultStance == AnimationType.INVALID)
                        this.defaultStance = key;
                    if (this.counts.ContainsKey(key))
                    {
                        Dictionary<AnimationType, byte> counts;
                        AnimationType index;
                        (counts = this.counts)[index = key] = (byte)(counts[index] + 1U);
                    }
                    else
                        this.counts.Add(key, 1);
                }
            }
            this.hasStand = this.counts.ContainsKey(AnimationType.STAND);
            this.hasWalk = this.counts.ContainsKey(AnimationType.WALK);
            this.hasRun = this.counts.ContainsKey(AnimationType.RUN);
            this.hasCrouch = this.counts.ContainsKey(AnimationType.CROUCH);
            this.hasDead = this.counts.ContainsKey(AnimationType.DEAD);
            this.hasIdle = this.counts.ContainsKey(AnimationType.IDLE);
            this.hasTalk = this.counts.ContainsKey(AnimationType.TALK);
            this.hasBlink = this.counts.ContainsKey(AnimationType.BLINK);
        }
        public void ChangeGraphic(IndexedColorGraphic graphic)
        {
            this.graphic = graphic;
            this.counts.Clear();
            this.animationType = (AnimationType)263;
            this.overrideSubsprite = string.Empty;
            this.Initialize();
        }
        public void OverrideSubsprite(string subsprite)
        {
            this.isOverriden = true;
            this.overrideSubsprite = subsprite;
            this.graphic.SpeedModifier = 1f;
        }
        public void ClearOverride()
        {
            this.isOverriden = false;
            this.overrideSubsprite = string.Empty;
        }
        private AnimationType GetDirectionPart(Vector2f velocity, int direction, int dirCount)
        {
            int num = direction;
            if (dirCount == 4 && num % 2 == 1)
            {
                num = ((num > 4) ? 6 : 2);
            }
            AnimationType result;
            switch (num)
            {
                case 0:
                    result = AnimationType.EAST;
                    break;
                case 1:
                    result = AnimationType.NORTHEAST;
                    break;
                case 2:
                    result = AnimationType.NORTH;
                    break;
                case 3:
                    result = AnimationType.NORTHWEST;
                    break;
                case 4:
                    result = AnimationType.WEST;
                    break;
                case 5:
                    result = AnimationType.SOUTHWEST;
                    break;
                case 6:
                    result = AnimationType.SOUTH;
                    break;
                case 7:
                    result = AnimationType.SOUTHEAST;
                    break;
                default:
                    result = AnimationType.SOUTH;
                    break;
            }
            return result;
        }
        private void DetermineSubsprite(float velocityMagnitude, int direction, AnimationContext context)
        {
            AnimationType animationType = AnimationType.SOUTH;
            AnimationType animationType2;
            if (context.IsTalk)
            {
                animationType2 = AnimationType.TALK;
            }
            else if (context.IsDead)
            {
                animationType2 = AnimationType.DEAD;
            }
            else if (context.IsCrouch)
            {
                animationType2 = AnimationType.CROUCH;
            }
            else if (context.IsNauseous)
            {
                animationType2 = AnimationType.NAUSEA;
            }
            else if (velocityMagnitude >= 2f)
            {
                TerrainType terrainType = context.TerrainType;
                if (terrainType == TerrainType.Ocean)
                {
                    animationType2 = AnimationType.SWIM;
                }
                else
                {
                    animationType2 = (this.hasRun ? AnimationType.RUN : AnimationType.WALK);
                }
            }
            else if (velocityMagnitude > 0f)
            {
                TerrainType terrainType2 = context.TerrainType;
                if (terrainType2 == TerrainType.Ocean)
                {
                    animationType2 = AnimationType.SWIM;
                }
                else
                {
                    animationType2 = AnimationType.WALK;
                }
            }
            else
            {
                TerrainType terrainType3 = context.TerrainType;
                if (terrainType3 == TerrainType.Ocean)
                {
                    animationType2 = AnimationType.FLOAT;
                }
                else
                {
                    animationType2 = AnimationType.STAND;
                }
            }
            if (!this.counts.ContainsKey(animationType2) || this.counts[animationType2] == 0)
            {
                animationType2 = this.defaultStance;
            }
            if (animationType2 != AnimationType.INVALID)
            {
                animationType = this.GetDirectionPart(context.Velocity, direction, this.counts[animationType2]);
            }
            this.animationType = (animationType2 | animationType);
        }
        public void UpdateSubsprite(AnimationContext context)
        {
            bool flag = false;
            float num = VectorMath.Magnitude(context.Velocity);
            int num3;
            if (num > 0f)
            {
                double num2 = Math.Atan2(-context.Velocity.Y, context.Velocity.X);
                num3 = (int)Math.Floor(num2 / 0.7853981633974483);
                if (num3 < 0)
                {
                    num3 += 8;
                }
            }
            else
            {
                num3 = context.SuggestedDirection;
                flag = (!context.IsDead && !context.IsCrouch && !context.IsTalk);
            }
            bool reset = false;
            string @string;
            if (!this.isOverriden)
            {
                this.previousStance = (this.animationType & AnimationType.STANCE_MASK);
                this.DetermineSubsprite(num, num3, context);
                @string = AnimationNames.GetString(this.animationType);
                reset = (this.previousStance != (this.animationType & AnimationType.STANCE_MASK));
            }
            else
            {
                @string = this.overrideSubsprite;
            }
            this.graphic.SetSprite(@string, reset);
            if (!this.isOverriden)
            {
                if (flag && (!this.hasStand || this.animationType == AnimationType.NAUSEA))
                {
                    this.graphic.SpeedModifier = 0f;
                    this.graphic.Frame = 0f;
                }
                else
                {
                    this.graphic.SpeedModifier = 1f;
                }
            }
            this.previousVelocity = context.Velocity;
            this.previousDirection = num3;
        }
    }
}

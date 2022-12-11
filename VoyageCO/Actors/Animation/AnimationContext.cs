using SFML.System;
using VCO.Overworld;

namespace VCO.Actors.Animation
{
    internal struct AnimationContext
    {
        public Vector2f Velocity;
        public int SuggestedDirection;
        public TerrainType TerrainType;
        public bool IsDead;
        public bool IsCrouch;
        public bool IsTalk;
        public bool IsNauseous;
    }
}
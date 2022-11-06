using SFML.Graphics;
using SFML.System;
using VCO.Battle.UI;
using Violet.Audio;
using Violet.Graphics;

namespace VCO.Battle.AUXAnimation
{
    #pragma warning disable CS0649 // Field is never assigned to, and will always have its value null
    //REASON: These fields are assigned to, but not always.
    internal struct AUXElement
    {
        public int Timestamp;

        public MultipartAnimation Animation;

        public Vector2f Offset;

        public bool LockToTargetPosition;

        public int PositionIndex;

        public VioletSound Sound;

        public int CardPop;

        public float CardPopSpeed;

        public int CardPopHangtime;

        public BattleCard.SpringMode CardSpringMode;

        public Vector2f CardSpringAmplitude;

        public Vector2f CardSpringSpeed;

        public Vector2f CardSpringDecay;

        public Color? TargetFlashColor;

        public ColorBlendMode TargetFlashBlendMode;

        public int TargetFlashCount;

        public int TargetFlashFrames;

        public Color? SenderFlashColor;

        public ColorBlendMode SenderFlashBlendMode;

        public int SenderFlashCount;

        public int SenderFlashFrames;

        public Color? ScreenDarkenColor;

        public int? ScreenDarkenDepth;
    }
}

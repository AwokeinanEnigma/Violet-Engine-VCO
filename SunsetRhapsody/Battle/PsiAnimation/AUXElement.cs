using System;
using Violet.Audio;
using Violet.Graphics;
using SunsetRhapsody.Battle.UI;
using SFML.Graphics;
using SFML.System;

namespace SunsetRhapsody.Battle.AUXAnimation
{
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

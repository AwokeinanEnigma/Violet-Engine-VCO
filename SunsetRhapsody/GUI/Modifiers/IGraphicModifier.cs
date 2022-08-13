using System;
using Violet.Graphics;

namespace SunsetRhapsody.GUI.Modifiers
{
	internal interface IGraphicModifier
	{
		bool Done { get; }

		Graphic Graphic { get; }

		void Update();
	}
}

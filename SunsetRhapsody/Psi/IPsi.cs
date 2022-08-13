using System;

namespace SunsetRhapsody.Psi
{
	internal interface IPsi
	{
		string Key { get; }

		string Name { get; }

		int Animation { get; set; }

		int[] PP { get; set; }

		int[] Levels { get; set; }
	}
}

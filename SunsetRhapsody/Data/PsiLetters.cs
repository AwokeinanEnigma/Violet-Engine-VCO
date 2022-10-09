﻿using System;
using System.Collections.Generic;

namespace SunsetRhapsody.Data
{
	internal class AUXLetters
	{
		public static char Get(int level)
		{
			char result = '?';
			AUXLetters.letters.TryGetValue(level, out result);
			return result;
		}
		/*	private static readonly string[] AUX_LEVEL_STRINGS = new string[]
{
"α",
"β",
"γ",
"Ω"
};*/
		private static Dictionary<int, char> letters = new Dictionary<int, char>
		{
			{
				0,
				'α'
			},
			{
				1,
				'β'
			},
			{
				2,
				'γ'
			},
			{
				3,
				'Ω'
			}
		};
	}
}

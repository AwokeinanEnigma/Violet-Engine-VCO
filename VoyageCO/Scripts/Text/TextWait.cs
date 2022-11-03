﻿using System;

namespace VCO.Scripts.Text
{
	internal class TextWait : ITextCommand
	{
		public int Position
		{
			get
			{
				return this.position;
			}
			set
			{
				this.position = value;
			}
		}

		public TextWait(int position)
		{
			this.position = position;
		}

		private int position;
	}
}
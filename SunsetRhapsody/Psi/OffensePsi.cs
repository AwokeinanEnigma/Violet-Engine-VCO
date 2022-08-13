using System;

namespace SunsetRhapsody.Psi
{
	public struct OffensePsi : IPsi
	{
		public string Key
		{
			get
			{
				return this._key;
			}
			set
			{
				this._key = value;
			}
		}

		public string Name
		{
			get
			{
				return this._name;
			}
			set
			{
				this._name = value;
			}
		}

		public int Animation
		{
			get
			{
				return this._animIndex;
			}
			set
			{
				this._animIndex = value;
			}
		}

		public int[] PP
		{
			get
			{
				return this._pp;
			}
			set
			{
				this._pp = value;
			}
		}

		public int[] Levels
		{
			get
			{
				return this._levels;
			}
			set
			{
				this._levels = value;
			}
		}

		public OffensePsi(OffensePsi ability)
		{
			this._key = ability._key;
			this._name = ability._name;
			this._animIndex = ability._animIndex;
			this._pp = ability._pp;
			this._levels = ability._levels;
			this.Effect = ability.Effect;
			this.Target = ability.Target;
			this.Special = ability.Special;
		}

		private string _key;

		private string _name;

		private int _animIndex;

		private int[] _pp;

		private int[] _levels;

		public int[][] Effect;

		public string Target;

		public float Special;
	}
}

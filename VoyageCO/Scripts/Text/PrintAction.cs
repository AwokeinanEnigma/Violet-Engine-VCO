using System;

namespace VCO.GUI.Text.PrintActions
{
	internal struct PrintAction
	{
		public PrintActionType Type
		{
			get
			{
				return this.type;
			}
		}

		public object Data
		{
			get
			{
				return this.data;
			}
		}

		public PrintAction(PrintActionType type, params object[] data)
		{
			this.type = type;
			this.data = data;
		}

		public PrintAction(PrintActionType type, object data)
		{
			this.type = type;
			this.data = data;
		}

		private PrintActionType type;

		private object data;
	}
}

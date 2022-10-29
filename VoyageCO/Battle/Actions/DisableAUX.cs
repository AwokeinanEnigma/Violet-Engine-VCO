using System;
using VCO.Utility;

namespace VCO.Battle.Actions
{
	internal class DisableAUX : StatusEffectAction
	{
		public DisableAUX(ActionParams aparams) : base(aparams)
		{
			if (this.effect.TurnsRemaining > 1)
			{
				this.message = string.Format("@{0} can't feel their AUX!", new object[]
				{
					this.senderName
				});
			}
			else
			{
				this.message = string.Format("@{0} felt their AUX return to them!", new object[]
				{
					this.senderName
				});
			}
			this.actionStartSound = this.controller.InterfaceController.PreAUXSound;
		}
	}
}

using System;
using System.Collections.Generic;
using VCO.GUI;
using Violet.Flags;

namespace VCO.Scripts.Actions.Types
{
    internal class TelepathyEndAction : RufiniAction
    {
		public override string Code => "TEEA";
		public TelepathyEndAction()
        {
            this.paramList = new List<ActionParam>();
        }

        public override ActionReturnContext Execute(ExecutionContext context)
        {
			Console.WriteLine("Telepathy time is over!");
			FlagManager.Instance[4] = false;
			if (context.CheckedNPC != null)
			{
				context.CheckedNPC.Untelepathize();
				if (context.TextBox is OverworldTextBox)
				{
					((OverworldTextBox)context.TextBox).SetDimmer(0f);
				}
			}
			return default(ActionReturnContext);
		}
	}
}

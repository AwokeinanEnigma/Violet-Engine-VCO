using System.Collections.Generic;
using VCO.Actors.NPCs;
using VCO.Scripts;
using VCO.Scripts.Actions;
using Violet.Actors;

namespace Rufini.Actions.Types
{
    internal class ChangeSubspriteNPCAction : RufiniAction
    {
        public override string Code => "CSSN";
        public ChangeSubspriteNPCAction()
        {
            this.paramList = new List<ActionParam>
            {
                new ActionParam
                {
                    Name = "name",
                    Type = typeof(string)
                },
                new ActionParam
                {
                    Name = "sub",
                    Type = typeof(string)
                }
            };
        }

        public override ActionReturnContext Execute(ExecutionContext context)
        {
            string name = base.GetValue<string>("name");
            string value = base.GetValue<string>("sub");
            NPC npc = (NPC)context.ActorManager.Find((Actor x) => x is NPC && ((NPC)x).Name == name);
            if (npc != null)
            {
                if (value.Length > 0)
                {
                    npc.OverrideSubsprite(value);
                }
                else
                {
                    npc.ClearOverrideSubsprite();
                }
            }
            return default(ActionReturnContext);
        }
    }
}

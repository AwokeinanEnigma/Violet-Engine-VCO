using System.Collections.Generic;
using VCO.Actors.NPCs;
using VCO.Scripts;
using VCO.Scripts.Actions;
using Violet.Actors;

namespace Rufini.Actions.Types
{
    internal class EntityDeleteAction : RufiniAction
    {
        public EntityDeleteAction()
        {
            this.paramList = new List<ActionParam>
            {
                new ActionParam
                {
                    Name = "name",
                    Type = typeof(string)
                }
            };
        }

        public override ActionReturnContext Execute(ExecutionContext context)
        {
            string name = base.GetValue<string>("name");
            NPC npc = (NPC)context.ActorManager.Find((Actor x) => x is NPC && ((NPC)x).Name == name);
            if (npc != null)
            {
                context.ActorManager.Remove(npc);
                context.CollisionManager.Remove(npc);
            }
            return default(ActionReturnContext);
        }
    }
}

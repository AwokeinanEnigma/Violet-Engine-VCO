﻿using System.Collections.Generic;
using VCO.Actors.NPCs;
using VCO.Scripts;
using VCO.Scripts.Actions;
using Violet.Actors;

namespace Rufini.Actions.Types
{
    internal class HopNPCAction : RufiniAction
    {
        public HopNPCAction()
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
                    Name = "h",
                    Type = typeof(int)
                },
                new ActionParam
                {
                    Name = "col",
                    Type = typeof(bool)
                }
            };
        }

        public override ActionReturnContext Execute(ExecutionContext context)
        {
            string name = base.GetValue<string>("name");
            int value = base.GetValue<int>("h");
            base.GetValue<bool>("col");
            NPC npc = (NPC)context.ActorManager.Find((Actor n) => n is NPC && ((NPC)n).Name == name);
            if (npc != null)
            {
                npc.HopFactor = value;
            }
            return default(ActionReturnContext);
        }
    }
}

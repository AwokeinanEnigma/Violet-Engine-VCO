using System.Collections.Generic;
using VCO.Battle;
using VCO.Data;
using VCO.Scripts;
using VCO.Scripts.Actions;
using VCO.Scripts.Actions.ParamTypes;

namespace Rufini.Actions.Types
{
    internal class StatSetAction : RufiniAction
    {
        public StatSetAction()
        {
            this.paramList = new List<ActionParam>
            {
                new ActionParam
                {
                    Name = "char",
                    Type = typeof(RufiniOption)
                },
                new ActionParam
                {
                    Name = "stat",
                    Type = typeof(RufiniOption)
                },
                new ActionParam
                {
                    Name = "val",
                    Type = typeof(int)
                }
            };
        }

        public override ActionReturnContext Execute(ExecutionContext context)
        {
            CharacterType option = (CharacterType)base.GetValue<RufiniOption>("char").Option;
            int option2 = base.GetValue<RufiniOption>("stat").Option;
            int value = base.GetValue<int>("val");
            StatSet stats = CharacterStats.GetStats(option);
            switch (option2)
            {
                case 0:
                    stats.HP = value;
                    break;
                case 1:
                    stats.PP = value;
                    break;
                case 2:
                    stats.MaxHP = value;
                    break;
                case 3:
                    stats.MaxPP = value;
                    break;
                case 4:
                    stats.Offense = value;
                    break;
                case 5:
                    stats.Defense = value;
                    break;
                case 6:
                    stats.Speed = value;
                    break;
                case 7:
                    stats.Guts = value;
                    break;
                case 8:
                    stats.IQ = value;
                    break;
                case 9:
                    stats.Luck = value;
                    break;
                case 10:
                    stats.Meter = value;
                    break;
                case 11:
                    stats.Level = value;
                    break;
            }
            CharacterStats.SetStats(option, stats);
            return default(ActionReturnContext);
        }
    }
}

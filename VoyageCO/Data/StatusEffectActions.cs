using System;
using System.Collections.Generic;
using VCO.Battle;
using VCO.Battle.Actions;

namespace VCO.Data
{
    internal class StatusEffectActions
    {
        public static Type Get(StatusEffect effect)
        {
            Type result = null;
            if (StatusEffectActions.types.ContainsKey(effect))
            {
                result = StatusEffectActions.types[effect];
            }
            return result;
        }

        private static Dictionary<StatusEffect, Type> types = new Dictionary<StatusEffect, Type>
        {
            {
                StatusEffect.Talking,
                typeof(TalkStatusEffectAction)
            },
            {
                StatusEffect.DisableAUX,
                typeof(DisableAUX)
            },
            {
                StatusEffect.Infection,
                typeof(InfectionStatusEffectAction)
            }
        };
    }
}

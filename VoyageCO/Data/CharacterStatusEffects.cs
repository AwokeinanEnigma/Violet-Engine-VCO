using System.Collections.Generic;
using VCO.Battle;

namespace VCO.Data
{
    internal static class CharacterStatusEffects
    {
        public static void AddPersistentEffects(CharacterType character, IEnumerable<StatusEffect> effects)
        {
            foreach (StatusEffect statusEffect in effects)
            {
                if (CharacterStatusEffects.PERSISTENT_EFFECTS.Contains(statusEffect))
                {
                    CharacterStatusEffects.AddStatusEffect(character, statusEffect);
                }
            }
        }
        public static void AddStatusEffect(CharacterType character, StatusEffect effect)
        {
            if (!CharacterStatusEffects.EFFECT_DICT.TryGetValue(character, out ISet<StatusEffect> set))
            {
                set = new HashSet<StatusEffect>();
                CharacterStatusEffects.EFFECT_DICT.Add(character, set);
            }
            if (!set.Contains(effect))
            {
                set.Add(effect);
            }
        }
        public static void RemoveStatusEffect(CharacterType character, StatusEffect effect)
        {
            if (CharacterStatusEffects.EFFECT_DICT.TryGetValue(character, out ISet<StatusEffect> set) && set.Contains(effect))
            {
                set.Remove(effect);
            }
        }
        public static bool HasStatusEffect(CharacterType character, StatusEffect effect)
        {
            bool result = false;
            if (CharacterStatusEffects.EFFECT_DICT.TryGetValue(character, out ISet<StatusEffect> set))
            {
                foreach (StatusEffect statusEffect in set)
                {
                    if (statusEffect == effect)
                    {
                        result = true;
                        break;
                    }
                }
            }
            return result;
        }
        private static readonly ISet<StatusEffect> PERSISTENT_EFFECTS = new HashSet<StatusEffect>
        {
            StatusEffect.Diamondized,
            StatusEffect.Mushroomized,
            StatusEffect.Nausea,
            StatusEffect.Paralyzed,
            StatusEffect.Poisoned,
            StatusEffect.Possessed,
            StatusEffect.Sunstroke,
            StatusEffect.Unconscious
        };
        private static readonly Dictionary<CharacterType, ISet<StatusEffect>> EFFECT_DICT = new Dictionary<CharacterType, ISet<StatusEffect>>();
    }
}

using System.Collections.Generic;

namespace Violet.Utility
{
    public static class DictionaryExtensions
    {
        /// <summary>
        /// Finds a key by its value
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="W"></typeparam>
        /// <param name="dict"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        public static T KeyByValue<T, W>(this Dictionary<T, W> dict, W val)
        {
            T key = default;
            foreach (KeyValuePair<T, W> pair in dict)
            {
                if (EqualityComparer<W>.Default.Equals(pair.Value, val))
                {
                    key = pair.Key;
                    break;
                }
            }
            return key;
        }

        /// <summary>
        /// Replaces the value of a key in a dictionary.
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="dictionary"></param>
        /// <param name="key">The key for the value</param>
        /// <param name="value">The value to replace the existing value of the key with</param>
        public static void AddReplace<K, V>(this Dictionary<K, V> dictionary, K key, V value)
        {
            if (dictionary.ContainsKey(key))
            {
                dictionary[key] = value;
                return;
            }
            dictionary.Add(key, value);
        }
    }
}

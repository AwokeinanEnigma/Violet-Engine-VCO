using fNbt;
using System;
using System.Collections.Generic;

namespace VCO.Data
{
    internal static class CharacterNames
    {
        public static string GetName(CharacterType character)
        {
            if (!CharacterNames.names.ContainsKey(character))
            {
                return Enum.GetName(typeof(CharacterType), character);
            }
            return CharacterNames.names[character];
        }

        public static void SetName(CharacterType character, string name)
        {
            if (CharacterNames.names.ContainsKey(character))
            {
                CharacterNames.names[character] = name;
                return;
            }
            CharacterNames.names.Add(character, name);
        }

        public static void LoadFromNBT(NbtList nameListTag)
        {
            if (nameListTag != null && nameListTag.TagType == NbtTagType.Compound)
            {
                foreach (NbtTag nbtTag in nameListTag)
                {
                    NbtCompound nbtCompound = (NbtCompound)nbtTag;
                    bool flag = true;
                    flag &= nbtCompound.TryGet<NbtInt>("k", out NbtInt nbtInt);
                    flag &= nbtCompound.TryGet<NbtString>("v", out NbtString nbtString);
                    if (flag)
                    {
                        CharacterType intValue = (CharacterType)nbtInt.IntValue;
                        string stringValue = nbtString.StringValue;
                        CharacterNames.SetName(intValue, stringValue);
                    }
                }
            }
        }

        public static NbtTag ToNBT()
        {
            IList<NbtCompound> list = new List<NbtCompound>();
            foreach (KeyValuePair<CharacterType, string> keyValuePair in CharacterNames.names)
            {
                list.Add(new NbtCompound
                {
                    new NbtInt("k", (int)keyValuePair.Key),
                    new NbtString("v", keyValuePair.Value)
                });
            }
            return new NbtList("names", list);
        }

        public const string NBT_TAG_NAME = "names";

        private static readonly Dictionary<CharacterType, string> names = new Dictionary<CharacterType, string>
        {
            {
                CharacterType.Travis,
                "Travis"
            },
            {
                CharacterType.Zack,
                "Zack"
            },
            {
                CharacterType.Meryl,
                "Sean"
            },
            {
                CharacterType.Floyd,
                "Floyd"
            },
            {
                CharacterType.Leo,
                "Leo"
            },
            {
                CharacterType.Renee,
                "Renee"
            }
        };
    }
}

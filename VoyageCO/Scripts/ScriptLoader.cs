using fNbt;
using System.Collections.Generic;
using System.Linq;
using VCO.Scripts.Actions;

namespace VCO.Scripts
{
    internal class ScriptLoader
    {
        public static RufiniScript? Load(string name)
        {
            NbtFile nbtFile = new NbtFile(DataHandler.instance.Load("script.dat"));
            NbtCompound rootTag = nbtFile.RootTag;
            NbtTag nbtTag = rootTag.Get<NbtTag>(name);
            ICollection<NbtTag> collection = null;
            RufiniScript? result = null;
            if (nbtTag != null)
            {
                if (nbtTag is NbtList)
                {
                    collection = (NbtList)nbtTag;
                    int count = ((NbtList)nbtTag).Count;
                }
                else if (nbtTag is NbtCompound)
                {
                    collection = (NbtCompound)nbtTag;
                    int count2 = ((NbtCompound)nbtTag).Count;
                }
                RufiniAction[] array = new RufiniAction[collection.Count<NbtTag>()];
                int num = 0;
                foreach (NbtTag nbtTag2 in collection)
                {
                    if (nbtTag2 != null && nbtTag2 is NbtCompound)
                    {
                        array[num++] = ActionFactory.FromNbt((NbtCompound)nbtTag2);
                    }
                }
                result = new RufiniScript?(new RufiniScript
                {
                    Name = name,
                    Actions = array
                });
            }
            return result;
        }
    }
}

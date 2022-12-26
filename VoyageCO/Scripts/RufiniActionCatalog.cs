using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using VCO.Scripts.Actions;
using Violet;

namespace VCO.Scripts
{
    // t is self
    // t2 is the type of catalog
    internal abstract class InstancedCatalog<T, T2> : Catalog<T2> where T : InstancedCatalog<T, T2>
    {
        public static T instance { get; private set; }

        public static T2 StaticGet(string name)
        {
            return instance.Get(name);
        }

        public InstancedCatalog()
        {
            if (instance != null) throw new InvalidOperationException("Singleton class \"" + typeof(T).Name + "\" was instantiated twice!");
            instance = this as T;
        }
    }

    internal abstract class Catalog<T>
    {
        protected Catalog()
        {
            InitializeCatalog();
        }

        public virtual void InitializeCatalog()
        {
            CollectEntries();
        }

        protected abstract void CollectEntries();

        public abstract T Get(string name);
    }


    internal class RufiniActionCatalog : InstancedCatalog<RufiniActionCatalog, RufiniAction>
    {
        protected List<RufiniAction> actions;

        private RufiniAction lastAction;

        protected override void CollectEntries()
        {
            Debug.Log("1");
            actions = new List<RufiniAction>();

            IEnumerable<Type> rufiniActions = Assembly.GetExecutingAssembly().GetTypes().Where(type => !type.IsAbstract && type.IsSubclassOf(typeof(RufiniAction)));

            foreach (Type  rufini in rufiniActions)
            {
                // Don't initialize the action yet, we'll have stuff do that on it's own.
                // This just serves as a thing to hold the types.
                RufiniAction item = (RufiniAction)FormatterServices.GetUninitializedObject(rufini );

                if (lastAction != null && item.Code == lastAction.Code) {
                    Debug.LogWarning($"Duplicate RufiniAction codes, {item.Code}. Original: {lastAction.GetType().Name} -> Duplicate {item.GetType().Name}");
                    continue;
                }

                actions.Add(item);
                lastAction = item;
            }
        }


        public override RufiniAction Get(string code)
        {
            RufiniAction action = actions.Find(x => x.Code == code);


            if (action == null) {
                Debug.LogError($"Can't locate specific RufiniAction with code '{code}'!", false);
                return null;
            }
            return action;
        }
    }
}

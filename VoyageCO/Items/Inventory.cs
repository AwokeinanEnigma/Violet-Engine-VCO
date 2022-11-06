using System.Collections.Generic;

namespace VCO.Items
{
    internal class Inventory
    {
        public int MaxItems => this.max;

        public int Count => this.items.Count;

        public bool Full => this.Count >= this.MaxItems;

        public Item this[int index] => this.Get(index);

        public Inventory(int max)
        {
            this.max = max;
            this.items = new List<Item>(max);
        }

        public void Add(Item item)
        {
            this.items.Add(item);
        }

        public void Remove(Item item)
        {
            this.items.Remove(item);
        }

        public void Remove(Item item, int count)
        {
            int num = 0;
            foreach (Item item2 in this.items)
            {
                if (item2 == item)
                {
                    this.items.Remove(item2);
                    num++;
                    if (num > count)
                    {
                        break;
                    }
                }
            }
        }

        public void RemoveAll(Item item)
        {
            this.items.RemoveAll((Item x) => x == item);
        }

        public void RemoveAt(int index)
        {
            this.items.RemoveAt(index);
        }

        public Item Get(int index)
        {
            if (index >= 0 && index < this.Count)
            {
                return this.items[index];
            }
            return null;
        }

        private readonly int max;

        private readonly List<Item> items;
    }
}

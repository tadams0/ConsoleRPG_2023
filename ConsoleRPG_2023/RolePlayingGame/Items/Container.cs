using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleRPG_2023.RolePlayingGame.Items
{
    /// <summary>
    /// Defines a container which holds <see cref="Item"/>s.
    /// </summary>
    public class Container
    {
        /// <summary>
        /// Gets the number of items within the container.
        /// </summary>
        public int Count
        {
            get { return items.Count; }
        }

        private List<Item> items = new List<Item>();

        public Container() 
        { 
        
        }

        public void AddItem(Item item)
        {
            if (item == null)
            {
                int test = 0;
                test++;
            }
            items.Add(item);
        }

        public bool RemoveItem(Item item)
        {
            bool result = items.Remove(item);
            return result;
        }

        public void AddRange(IEnumerable<Item> newItems)
        {
            items.AddRange(newItems);
        }

        public List<Item> GetItems()
        {
            return new List<Item>(items);
        }

        public void Clear()
        {
            items.Clear();
        }


    }
}

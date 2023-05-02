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

        private List<Item> items = new List<Item>();

        public Container() 
        { 
        
        }

        public void AddItem(Item item)
        {
            items.Add(item);
        }

        public bool RemoveItem(Item item)
        {
            bool result = items.Remove(item);
            return result;
        }

        public List<Item> GetItems()
        {
            return new List<Item>(items);
        }


    }
}

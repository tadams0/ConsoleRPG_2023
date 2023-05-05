using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleRPG_2023.RolePlayingGame.Items
{
    public class Item : GameObject
    {
        /// <summary>
        /// The way the item is used.
        /// </summary>
        public ItemUseType ItemType { get; set; } = ItemUseType.None;

        /// <summary>
        /// The category tht the item fits within.
        /// </summary>
        public ItemCategoryType Category { get; set; } = ItemCategoryType.None;

        public string Name { get; set; } = "Item";

        public string Description { get; set; } = "nondescript";

        /// <summary>
        /// A basic noun description of the item. e.g. "Scroll", "Bread", "Potion", "Coif", "Helmet", "Boots", "Toy", etc..
        /// </summary>
        public string Noun { get; set; } = "missing";

        /// <summary>
        /// The owner of the item.
        /// </summary>
        public Character Owner { get; set; } = null;

        public Item() 
        { 
        
        }

        /// <summary>
        /// Does a shallow clone of the item.
        /// </summary>
        /// <returns>A new instance of an item.</returns>
        public virtual Item Clone()
        {
            Item clone = new Item()
            {
                Name = Name,
                Description = Description,
                Owner = Owner,
                Category = Category,
                Noun = Noun,
                ItemType = ItemType
            };

            return clone;
        }


    }
}

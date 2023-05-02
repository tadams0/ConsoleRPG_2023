using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleRPG_2023.RolePlayingGame.Items
{
    public class Item : GameObject
    {

        public ItemUseType ItemType { get; set; } = ItemUseType.None;

        public string Name { get; set; }

        public string Description { get; set; }

        public Item() 
        { 
        
        }


    }
}

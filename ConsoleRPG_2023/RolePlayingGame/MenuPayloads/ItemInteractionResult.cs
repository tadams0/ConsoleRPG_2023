using ConsoleRPG_2023.RolePlayingGame.Items;
using ConsoleRPG_2023.RolePlayingGame.Maps;
using ConsoleRPG_2023.RolePlayingGame.Maps.MapObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleRPG_2023.RolePlayingGame
{
    public class ItemInteractionResult : MapObjectInteractionResult
    {

        /// <summary>
        /// The item being picked up.
        /// </summary>
        public Item Item { get; set; }

        /// <summary>
        /// The location the item is being picked up from. e.g. "ground", "table", "chair", "grate"
        /// </summary>
        public string LocationDescription { get; set; }


        public ItemInteractionResult(Map map, MapObject mapObject, Character character, Item item, string locationDescription) 
            : base(map, mapObject, character)
        { 
            Item = item;
            LocationDescription = locationDescription;
        }

    }
}

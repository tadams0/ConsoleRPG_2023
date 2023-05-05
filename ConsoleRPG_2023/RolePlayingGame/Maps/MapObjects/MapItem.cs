using ConsoleRPG_2023.RolePlayingGame.Items;
using ConsoleRPG_2023.RolePlayingGame.Renderers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleRPG_2023.RolePlayingGame.Maps.MapObjects
{
    public class MapItem : MapObject
    {
        private static ItemRenderer itemRenderer = new ItemRenderer();

        public string Name
        {
            get { return Item.Name; }
        }

        /// <summary>
        /// The item that this instance is tied to.
        /// </summary>
        public Item Item
        {
            get { return item; }
        }

        /// <summary>
        /// A simple description of where the item is. e.g. "on the ground", "between the chairs", "under a rock", "wedged in the crevice of the cave wall"
        /// </summary>
        public string LocationDescription
        {
            get; set;
        } = "on the ground";

        private Item item;

        public MapItem(Item item)
        {
            this.item = item;
        }

        public override MapObjectInteractionResult Interact(Map map, Character character)
        {
            ItemInteractionResult result = new ItemInteractionResult(map, this, character, item, LocationDescription);
            result.InteractionMessage = string.Empty;
            result.Action = "ItemPickupMenu";
            return result;
        }

        public override string ToString()
        {
            return itemRenderer.GetContainerDisplay(item, Console.WindowWidth);
        }
    }
}

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
    /// <summary>
    /// Defines a class that holds payload data for the inventory menu.
    /// </summary>
    public class ContainerInteractionResult : MapObjectInteractionResult
    {
        /// <summary>
        /// The container to be displayed.
        /// </summary>
        public Container ViewingContainer { get; set; }

        /// <summary>
        /// The container that items can be taken into. Leave as null if viewing an inventory that cannot have items taken from.
        /// </summary>
        public Container IntoContainer { get; set; }

        /// <summary>
        /// The simple description of the into container.
        /// </summary>
        public string IntoContainerDescription { get; set; } = string.Empty;

        /// <summary>
        /// The simple description of the viewing container.
        /// </summary>
        public string ViewingContainerDescription { get; set; } = "[No Description Found]";

        /// <summary>
        /// The location the item is being picked up from. e.g. "ground", "table", "chair", "grate"
        /// </summary>
        public string LocationDescription { get; set; }

        public ContainerInteractionResult(Map map, MapObject mapObject, Character character) 
            : base(map, mapObject, character)
        {
            if (mapObject is MapContainer)
            {
                MapContainer container = (MapContainer)mapObject;
                ViewingContainerDescription = container.ContainerDescription;
            }
        }

    }
}

using ConsoleRPG_2023.RolePlayingGame.Maps.MapObjects;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleRPG_2023.RolePlayingGame.Maps
{
    /// <summary>
    /// Defines an object that can reside within a map.
    /// </summary>
    public class MapObject : GameObject
    {

        /// <summary>
        /// The x position of the player within the map.
        /// </summary>
        public long X { get; set; }

        /// <summary>
        /// The y position of the player within the map.
        /// </summary>
        public long Y { get; set; }


        /// <summary>
        /// Interacts with the object.
        /// </summary>
        /// <param name="map">The map that this object is tied to.</param>
        /// <returns>The action taht the map object wants to execute.</returns>
        public virtual MapObjectInteractionResult Interact(Map map)
        {
            MapObjectInteractionResult defaultResult = new MapObjectInteractionResult(map, this);
            defaultResult.InteractionMessage = "Nothing happens.";
            return defaultResult;
        }

    }
}

using ConsoleRPG_2023.RolePlayingGame.Effects;
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
        /// Gets the list of active effects in this map.
        /// </summary>
        public List<ActiveEffect> ActiveEffects
        {
            get { return activeEffects; }
        }

        /// <summary>
        /// A list of active effects on this <see cref="MapObject"/> instance.
        /// </summary>
        private List<ActiveEffect> activeEffects = new List<ActiveEffect>();

        /// <summary>
        /// Interacts with the object.
        /// </summary>
        /// <param name="map">The map that this object is tied to.</param>
        /// <param name="character">The character, if any that triggered the interaction. Can be null if no character did the interaction.</param>
        /// <returns>The action that the map object wants to execute.</returns>
        public virtual MapObjectInteractionResult Interact(Map map, Character character)
        {
            MapObjectInteractionResult defaultResult = new MapObjectInteractionResult(map, this, character);
            defaultResult.InteractionMessage = "Nothing happens.";
            return defaultResult;
        }

    }
}

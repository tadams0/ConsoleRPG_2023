using ConsoleRPG_2023.RolePlayingGame.Maps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleRPG_2023.RolePlayingGame.Effects
{
    /// <summary>
    /// Defines a class which holds the functions for <see cref="Effect"/> instances.
    /// </summary>
    public static class EffectFunctionManager
    {
        /// <summary>
        /// Note that the integer parameter acts as the counter for how far along the duration of the effect is. e.g. the first run will be 1. The second will be 2, and so on until the duration is reached.
        /// <br/>The map is the map location where the effect occured.
        /// <br/>The PointL is the world position within the map that the effect was activated.
        /// </summary>
        private static List<Action<ActiveEffect, GameState, Map>> actionList = new List<Action<ActiveEffect, GameState, Map>>();

        /// <summary>
        /// A mapping to convert the upper variant of an action name to its corresponding id.
        /// </summary>
        private static Dictionary<string, int> nameToIdMapping = new Dictionary<string, int>();

        /// <summary>
        /// Adds the given action to the manager under the given name. An id is generated and returned.
        /// </summary>
        /// <param name="action">The action being added to the manager.</param>
        /// <param name="name">The name of the action to add. This will be converted to upper invariant so case so not matter.</param>
        /// <returns>A newly generated id which the given action now uses.</returns>
        public static int AddAction(Action<ActiveEffect, GameState, Map> action, string name)
        {
            actionList.Add(action);
            int id = actionList.Count - 1;
            name = name.ToUpperInvariant();
            nameToIdMapping[name] = id;

            return id;
        }

        /// <summary>
        /// Retrieves the effect action by id.
        /// </summary>
        public static Action<ActiveEffect, GameState, Map> GetAction(int id)
        {
            return actionList[id];
        }

        /// <summary>
        /// Retrieves the effect action by name. Case insensitive.
        /// </summary>
        public static Action<ActiveEffect, GameState, Map> GetAction(string name)
        {
            name = name.ToUpperInvariant();
            int id = nameToIdMapping[name];
            return actionList[id];
        }

        /// <summary>
        /// Retrieves the id assoaciated with the given name.
        /// </summary>
        public static int GetActionId(string name)
        {
            name = name.ToUpperInvariant();
            return nameToIdMapping[name];
        }

    }
}

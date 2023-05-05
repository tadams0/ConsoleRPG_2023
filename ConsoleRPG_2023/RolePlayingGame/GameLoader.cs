using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleRPG_2023.RolePlayingGame
{
    /// <summary>
    /// Defines a class used to load game data.
    /// </summary>
    public static class GameLoader
    {

        public static void Load(GameState state)
        {
            LoadItemTemplates(state);
        }

        private static void LoadItemTemplates(GameState gameState)
        {
            //At some point it'd be nice to have a serialized list of the item templates.
            //But for now we'll just hard code them here.

        }

    }
}

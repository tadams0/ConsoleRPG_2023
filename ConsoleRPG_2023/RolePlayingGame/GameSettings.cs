using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleRPG_2023.RolePlayingGame
{
    /// <summary>
    /// Defines a class which holds all relavent game settings.
    /// </summary>
    public class GameSettings
    {
        /// <summary>
        /// Gets whether or not to skip the intro.
        /// </summary>
        public bool SkipIntro { get; set; } = true;

        /// <summary>
        /// Gets whether or not to auto populate a name for the player when starting a new game.
        /// </summary>
        public bool AutoPopulateNameOnNewGame { get; set; } = true;

        /// <summary>
        /// Gets whether or not to use the map rendering optimizations. In most cases this will speed up screen refreshes, but there may be few cases in complex scenes that causes it to slow things down
        /// or render things incorrectly.
        /// </summary>
        public bool MapRenderOptimizations { get; set; } = true;

        /// <summary>
        /// Gets or sets the default map rendering height in characters.
        /// </summary>
        public int MapViewHeight { get; set; } = (int)(Console.WindowHeight * 0.5);

        /// <summary>
        /// Gets or sets the default map rendering width in characters (Monospaced).
        /// </summary>
        public int MapViewWidth { get; set; } = Console.WindowWidth;

        /// <summary>
        /// Gets or sets the default dungeon view height in characters.
        /// </summary>
        public int DungeonViewHeight { get; set; } = (int)(Console.WindowHeight * 0.5);

        /// <summary>
        /// Gets or sets the default dungeon view width in characters (Monospaced).
        /// </summary>
        public int DungeonViewWidth { get; set; } = Console.WindowWidth;


    }
}

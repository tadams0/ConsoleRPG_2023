using ConsoleRPG_2023.RolePlayingGame.Maps;
using ConsoleRPG_2023.RolePlayingGame.Renderers;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleRPG_2023.RolePlayingGame
{
    /// <summary>
    /// A class which contains all information regarding the current game state.
    /// </summary>
    public class GameState
    {
        public Character PlayerCharacter { get; set; }

        public HUD PlayerHud { get; set; }

        public ProceduralWorldMap WorldMap { get; set; }

        public MapRenderer MapRenderer { get; set; }

    }
}

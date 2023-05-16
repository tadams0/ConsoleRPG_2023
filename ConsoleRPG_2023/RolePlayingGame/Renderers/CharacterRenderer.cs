using ConsoleRPG_2023.RolePlayingGame.Maps;
using ConsoleRPG_2023.RolePlayingGame.Text;
using CustomConsoleColors;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleRPG_2023.RolePlayingGame.Renderers
{
    /// <summary>
    /// A <see cref="MapObjectRenderer"/> specialized for rendering <see cref="Character"/> instances.
    /// </summary>
    public class CharacterRenderer : MapObjectRenderer
    {
        public static readonly string playerDisplayChar = "☻";
        public static readonly string characterDisplayChar = "☺";

        private static readonly ConsoleColor defaultCharacterColor = ConsoleColor.Gray;

        public CharacterRenderer() 
        {
            renderedType = typeof(Character);
        }

        public override string GetDisplayCharacter(MapObject obj, Tile tile, GameState state)
        {
            if (state.PlayerCharacter == obj)
            {
                return playerDisplayChar;
            }

            return characterDisplayChar;
        }

        public override ConsoleColor GetDisplayColor(MapObject obj, Tile tile, GameState state)
        {
            Character character = obj as Character;
            ConsoleColor color = Helper.GetRaceColor(character.Race);

            //Check if the color of the character is the same as the tile (they'd appear invisible).
            //if (color == state.Map.TileTypeToColor(tile.TileType))
            //{
            //    color = Helper.GetAlternativeRaceColor(character.Race);
            //}

            return color;
        }

        public override string GetForegroundColor(MapObject obj, Tile tile, GameState state)
        {
            Character character = obj as Character;
            ConsoleColor color = Helper.GetRaceColor(character.Race);
            return ConsoleColorToForegroundSequenceColor(color);
        }

    }
}

using ConsoleRPG_2023.RolePlayingGame.Maps;
using ConsoleRPG_2023.RolePlayingGame.Maps.MapObjects;
using ConsoleRPG_2023.RolePlayingGame.Text;
using CustomConsoleColors;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace ConsoleRPG_2023.RolePlayingGame.Renderers
{
    /// <summary>
    /// A <see cref="MapObjectRenderer"/> specialized for rendering <see cref="HotSpring"/> instances.
    /// </summary>
    public class HotSpringRenderer : MapObjectRenderer
    {
        public static readonly string hotSpringCharacter = "H";

        private static readonly ConsoleColor defaultSpringConsoleColor = ConsoleColor.DarkYellow;
        private static readonly Color defaultSpringColor = Color.FromArgb(195,200,110);

        public HotSpringRenderer() 
        {
            renderedType = typeof(HotSpring);
        }

        public override string GetDisplayCharacter(MapObject obj, Tile tile, GameState state)
        {
            return hotSpringCharacter;
        }

        public override ConsoleColor GetDisplayColor(MapObject obj, Tile tile, GameState state)
        {
            return defaultSpringConsoleColor;
        }

        public override string GetForegroundColor(MapObject obj, Tile tile, GameState state)
        {
            return VirtualConsoleSequenceBuilder.GetColorForegroundSequence(defaultSpringColor);
        }

    }
}

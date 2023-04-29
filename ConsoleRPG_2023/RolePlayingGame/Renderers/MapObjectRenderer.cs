using ConsoleRPG_2023.RolePlayingGame.Maps;
using ConsoleRPG_2023.RolePlayingGame.Text;
using CustomConsoleColors;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace ConsoleRPG_2023.RolePlayingGame.Renderers
{
    public class MapObjectRenderer
    {
        public static readonly string UnkownMapObjectDisplayChar = "?";
        public static readonly ConsoleColor UnknownMapObjectColor = ConsoleColor.Red;
        public static readonly Color UnknownMapObjectForegroundColor = Color.Red;

        /// <summary>
        /// Gets the type of object that this renderer renders.
        /// </summary>
        public Type RenderedType
        {
            get { return renderedType; }
        }

        protected Type renderedType = typeof(MapObjectRenderer);

        public MapObjectRenderer() 
        { 
        }

        public virtual string GetDisplayCharacter(MapObject obj, Tile tile, GameState state)
        {
            return UnkownMapObjectDisplayChar;
        }

        public virtual ConsoleColor GetDisplayColor(MapObject obj, Tile tile, GameState state)
        {
            return UnknownMapObjectColor;
        }

        public virtual string GetForegroundColor(MapObject obj, Tile tile, GameState state)
        {
            return VirtualConsoleSequenceBuilder.GetColorForegroundSequence(UnknownMapObjectForegroundColor);
        }

        protected string ConsoleColorToForegroundSequenceColor(ConsoleColor consoleColor)
        {
            Color c = ConsoleColorScheme.GetConsoleColor(consoleColor);

            return VirtualConsoleSequenceBuilder.GetColorForegroundSequence(c);
        }

    }
}

﻿using ConsoleRPG_2023.RolePlayingGame.Dungeons;
using ConsoleRPG_2023.RolePlayingGame.Maps;
using ConsoleRPG_2023.RolePlayingGame.Text;
using CustomConsoleColors;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace ConsoleRPG_2023.RolePlayingGame.Renderers
{
    /// <summary>
    /// Defines a class speciallized to render <see cref="MapEntranceObj"/> instances.
    /// </summary>
    public class MapEntranceRenderer : MapObjectRenderer
    {
        public static readonly string defaultDisplayChar = "D";

        private static readonly Color defaultColor = Color.Black;

        public MapEntranceRenderer() 
        {
            renderedType = typeof(MapEntranceObj);
        }

        public override string GetDisplayCharacter(MapObject obj, Tile tile, GameState state)
        {
            return defaultDisplayChar;
        }

        public override ConsoleColor GetDisplayColor(MapObject obj, Tile tile, GameState state)
        {
            return ConsoleColor.Black;
        }

        public override string GetForegroundColor(MapObject obj, Tile tile, GameState state)
        {
            Color c = Color.Black;
            return VirtualConsoleSequenceBuilder.GetColorForegroundSequence(c);
        }

    }
}

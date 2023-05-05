using ConsoleRPG_2023.RolePlayingGame.Items;
using ConsoleRPG_2023.RolePlayingGame.Maps;
using ConsoleRPG_2023.RolePlayingGame.Maps.MapObjects;
using CustomConsoleColors;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleRPG_2023.RolePlayingGame.Renderers
{
    public class MapContainerRenderer : MapObjectRenderer
    {
        private static string defaultDisplayChar = "C";

        private static readonly Color defaultColor = Color.FromArgb(230,220,30);

        public MapContainerRenderer() 
        { 
            renderedType = typeof(MapContainer);
        }


        public override string GetDisplayCharacter(MapObject obj, Tile tile, GameState state)
        {
            MapContainer container = obj as MapContainer;
            if (container.ContainerDescription.IndexOf("Barrel", StringComparison.InvariantCultureIgnoreCase) >= 0)
            {
                return "B";
            }
            else if (container.ContainerDescription.IndexOf("Sack", StringComparison.InvariantCultureIgnoreCase) >= 0)
            {
                return "S";
            }
            else if (container.ContainerDescription.IndexOf("Chest", StringComparison.InvariantCultureIgnoreCase) >= 0)
            {//If the container type includes "chest" in the name.
                return defaultDisplayChar;
            }

            return defaultDisplayChar;
        }

        public override ConsoleColor GetDisplayColor(MapObject obj, Tile tile, GameState state)
        {
            return ConsoleColor.DarkYellow;
        }

        public override string GetForegroundColor(MapObject obj, Tile tile, GameState state)
        {
            Color c = defaultColor;
            return VirtualConsoleSequenceBuilder.GetColorForegroundSequence(c);
        }


    }
}

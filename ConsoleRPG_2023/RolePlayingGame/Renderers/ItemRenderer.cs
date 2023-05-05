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
    public class ItemRenderer : MapObjectRenderer
    {
        private static string defaultDisplayChar = "i";

        private static readonly Color defaultColor = Color.Yellow;

        public ItemRenderer() 
        { 
            renderedType = typeof(MapItem);
        }


        public override string GetDisplayCharacter(MapObject obj, Tile tile, GameState state)
        {
            return defaultDisplayChar;
        }

        public override ConsoleColor GetDisplayColor(MapObject obj, Tile tile, GameState state)
        {
            return ConsoleColor.Yellow;
        }

        public override string GetForegroundColor(MapObject obj, Tile tile, GameState state)
        {
            Color c = defaultColor;
            return VirtualConsoleSequenceBuilder.GetColorForegroundSequence(c);
        }

        public virtual string GetContainerDisplay(Item item, int maxSize)
        {
            Color backgroundColor = Color.Black;
            Color foregroundColor = Color.Violet;

            string backgroundColorStr = VirtualConsoleSequenceBuilder.GetColorBackgroundSequence(backgroundColor);
            string foregroundColorStr = VirtualConsoleSequenceBuilder.GetColorForegroundSequence(foregroundColor);


            string displayName = item.Name;
            if (displayName.Length > maxSize)
            {
                if (displayName.Length > 3)
                {//Add periods to signify there is more.
                    displayName = displayName.Substring(0, maxSize - 2);
                    displayName += "..";
                }
                else
                {
                    displayName = displayName.Substring(0, maxSize);
                }
            }
            return backgroundColorStr + foregroundColorStr + displayName + VirtualConsoleSequenceBuilder.Default;
        }


    }
}

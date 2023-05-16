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
    /// <summary>
    /// A <see cref="MapObjectRenderer"/> specialized for rendering <see cref="MapItem"/> instances.
    /// </summary>
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

        /// <summary>
        /// Gets the expected display for the given <see cref="Item"/> instance when viewed within a container.
        /// <br/>Note that this has color bleed and will bleed over into the proceeding text.
        /// </summary>
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
            return backgroundColorStr + foregroundColorStr + displayName;
        }
        
        /// <summary>
        /// Gets the expected display for the given <see cref="Item"/> instance when viewed within a container.
        /// <br/>Note that this restores colors to default and prevents color bleed.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="maxSize"></param>
        /// <returns></returns>
        public virtual string GetContainerDisplayNoBleed(Item item, int maxSize)
        {
            string s = GetContainerDisplay(item, maxSize);
            return s + VirtualConsoleSequenceBuilder.Default;
        }


    }
}

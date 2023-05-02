using ConsoleRPG_2023.RolePlayingGame.Items;
using CustomConsoleColors;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleRPG_2023.RolePlayingGame.Renderers
{
    public class ItemRenderer<T> where T : Item
    {
        private static string defaultWorldDisplay = "i";

        private static Random testRandom = new Random();

        public ItemRenderer() 
        { 
        
        }

        public virtual string GetContainerDisplay(T item, int maxSize)
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

        public virtual Color GetWorldDisplayColor(T item)
        {
            return Color.Red;
        }

        public virtual string GetWorldDisplay(T item)
        {
            Color c = GetWorldDisplayColor(item);
            string color = VirtualConsoleSequenceBuilder.GetColorForegroundSequence(c);
            return color + defaultWorldDisplay;
        }

    }
}

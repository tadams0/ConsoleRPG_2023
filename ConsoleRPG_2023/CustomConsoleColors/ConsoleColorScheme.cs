using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomConsoleColors
{
    public class ConsoleColorScheme
    {
        /// <summary>
        /// Creates a new array containing the legacy 16 console colors.
        /// </summary>
        public static Color[] LegacyDefaultColors
        {
            get { return legacyDefaultColors.ToArray(); }
        }

        /// <summary>
        /// Creates a new array containing the new 16 console colors.
        /// </summary>
        public static Color[] NewDefaultColors
        {
            get { return newDefaultColors.ToArray(); }
        }

        private static readonly Color[] legacyDefaultColors = new Color[16];
        private static readonly Color[] newDefaultColors = new Color[16];

        static ConsoleColorScheme()
        {
            legacyDefaultColors[(int)ConsoleColor.Black] = Color.FromArgb(0, 0, 0);
            legacyDefaultColors[(int)ConsoleColor.DarkBlue] = Color.FromArgb(0, 0, 128);
            legacyDefaultColors[(int)ConsoleColor.DarkGreen] = Color.FromArgb(0, 128, 0);
            legacyDefaultColors[(int)ConsoleColor.DarkCyan] = Color.FromArgb(0, 128, 128);
            legacyDefaultColors[(int)ConsoleColor.DarkRed] = Color.FromArgb(128, 0, 0);
            legacyDefaultColors[(int)ConsoleColor.DarkMagenta] = Color.FromArgb(128, 0, 128);
            legacyDefaultColors[(int)ConsoleColor.DarkYellow] = Color.FromArgb(128, 128, 0);
            legacyDefaultColors[(int)ConsoleColor.Gray] = Color.FromArgb(192, 192, 192);
            legacyDefaultColors[(int)ConsoleColor.DarkGray] = Color.FromArgb(128, 128, 128);
            legacyDefaultColors[(int)ConsoleColor.Blue] = Color.FromArgb(0, 0, 255);
            legacyDefaultColors[(int)ConsoleColor.Green] = Color.FromArgb(0, 255, 0);
            legacyDefaultColors[(int)ConsoleColor.Cyan] = Color.FromArgb(0, 255, 255);
            legacyDefaultColors[(int)ConsoleColor.Red] = Color.FromArgb(255, 0, 0);
            legacyDefaultColors[(int)ConsoleColor.Magenta] = Color.FromArgb(255, 0, 255);
            legacyDefaultColors[(int)ConsoleColor.Yellow] = Color.FromArgb(255, 255, 0);
            legacyDefaultColors[(int)ConsoleColor.White] = Color.FromArgb(255, 255, 255);

            newDefaultColors[(int)ConsoleColor.Black] = Color.FromArgb(12, 12, 12);
            newDefaultColors[(int)ConsoleColor.DarkBlue] = Color.FromArgb(0, 58, 218);
            newDefaultColors[(int)ConsoleColor.DarkGreen] = Color.FromArgb(19, 161, 14);
            newDefaultColors[(int)ConsoleColor.DarkCyan] = Color.FromArgb(58, 150, 221);
            newDefaultColors[(int)ConsoleColor.DarkRed] = Color.FromArgb(197, 15, 31);
            newDefaultColors[(int)ConsoleColor.DarkMagenta] = Color.FromArgb(136, 23, 152);
            newDefaultColors[(int)ConsoleColor.DarkYellow] = Color.FromArgb(193, 156, 0);
            newDefaultColors[(int)ConsoleColor.Gray] = Color.FromArgb(204, 204, 204);
            newDefaultColors[(int)ConsoleColor.DarkGray] = Color.FromArgb(118, 118, 118);
            newDefaultColors[(int)ConsoleColor.Blue] = Color.FromArgb(59, 120, 255);
            newDefaultColors[(int)ConsoleColor.Green] = Color.FromArgb(22, 198, 12);
            newDefaultColors[(int)ConsoleColor.Cyan] = Color.FromArgb(97, 214, 214);
            newDefaultColors[(int)ConsoleColor.Red] = Color.FromArgb(231, 72, 86);
            newDefaultColors[(int)ConsoleColor.Magenta] = Color.FromArgb(180, 0, 158);
            newDefaultColors[(int)ConsoleColor.Yellow] = Color.FromArgb(249, 241, 165);
            newDefaultColors[(int)ConsoleColor.White] = Color.FromArgb(242, 242, 242);
        }

        /// <summary>
        /// Retireves the <see cref="Color"/> value of the given <see cref="ConsoleColor"/>.
        /// </summary>
        public static Color GetConsoleColor(ConsoleColor c, bool useLegacyColors = false)
        {
            if (useLegacyColors)
            {
                return legacyDefaultColors[(int)c];
            }
            else
            {
                return newDefaultColors[(int)c];
            }
        }

        /// <summary>
        /// Gets a new array containing all the colors contained in this color scheme.
        /// </summary>
        public Color[] Colors
        {
            get { return colors.ToArray(); }
        }

        private Color[] colors = new Color[16];

        public ConsoleColorScheme() 
        {
            this.colors = newDefaultColors.ToArray();
        }

        public ConsoleColorScheme(Color[] colors)
        {
            this.colors = colors;
        }

        /// <summary>
        /// Sets the given color 
        /// </summary>
        /// <param name="color">The color to replace.</param>
        /// <param name="newColor">The new color taking its place.</param>
        public void SetColor(ConsoleColor color, Color newColor)
        {
            colors[(int)color] = newColor;
        }


    }



}

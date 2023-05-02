using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CustomConsoleColors
{
    /// <summary>
    /// Defines a class used to build out console sequence commands.
    /// <br/> They can be used like so: Console.Write(VirtualConsoleSequenceBuilder.Italics)
    /// </summary>
    public static class VirtualConsoleSequenceBuilder
    {
        private static readonly string ansiMatcherRegex = @"[\u001b\u009b][[()#;?]*(?:[0-9]{1,4}(?:;[0-9]{0,4})*)?[0-9A-ORZcf-nqry=><]";

        /// <summary>
        /// Returns all attributes to the default state prior to modification
        /// </summary>
        public static string Default
        {
            get { return "\x1b[" + 0 + "m"; }
        }

        /// <summary>
        /// Applies brightness/intensity flag to foreground color
        /// </summary>
        public static string BoldBright
        {
            get { return "\x1b[" + 1 + "m"; }
        }

        /// <summary>
        /// Removes brightness/intensity flag from foreground color
        /// </summary>
        public static string NoBoldBright
        {
            get { return "\x1b[" + 22 + "m"; }
        }

        /// <summary>
        /// Adds underline
        /// </summary>
        public static string Underline
        {
            get { return "\x1b[" + 4 + "m"; }
        }

        /// <summary>
        /// Adds a bold underline
        /// </summary>
        public static string BoldUnderline
        {
            get { return "\x1b[" + 21 + "m"; }
        }

        /// <summary>
        /// Removes underline
        /// </summary>
        public static string NoUnderline
        {
            get { return "\x1b[" + 24 + "m"; }
        }

        /// <summary>
        /// Adds italics to the foreground
        /// </summary>
        public static string Italics
        {
            get { return "\x1b[" + 3 + "m"; }
        }

        /// <summary>
        /// Adds italics to the foreground
        /// </summary>
        public static string NoItalics
        {
            get { return "\x1b[" + 23 + "m"; }
        }

        /// <summary>
        /// Swaps foreground and background colors
        /// </summary>
        public static string Invert
        {
            get { return "\x1b[" + 7 + "m"; }
        }

        /// <summary>
        /// Returns foreground/background to normal
        /// </summary>
        public static string Revert
        {
            get { return "\x1b[" + 27 + "m"; }
        }

        /// <summary>
        /// Swaps foreground and background colors
        /// </summary>
        public static string Negative
        {
            get { return "\x1b[" + 7 + "m"; }
        }

        /// <summary>
        /// Returns foreground/background to normal
        /// </summary>
        public static string Positive
        {
            get { return "\x1b[" + 27 + "m"; }
        }

        /// <summary>
        /// Applies non-bold/bright black to foreground
        /// </summary>
        public static string ForegroundBlack
        {
            get { return "\x1b[" + 30 + "m"; }
        }

        /// <summary>
        /// Applies non-bold/bright red to foreground
        /// </summary>
        public static string ForegroundRed
        {
            get { return "\x1b[" + 31 + "m"; }
        }

        /// <summary>
        /// Applies non-bold/bright green to foreground
        /// </summary>
        public static string ForegroundGreen
        {
            get { return "\x1b[" + 32 + "m"; }
        }

        /// <summary>
        /// Applies non-bold/bright yellow to foreground
        /// </summary>
        public static string ForegroundYellow
        {
            get { return "\x1b[" + 33 + "m"; }
        }

        /// <summary>
        /// Applies non-bold/bright blue to foreground
        /// </summary>
        public static string ForegroundBlue
        {
            get { return "\x1b[" + 34 + "m"; }
        }

        /// <summary>
        /// Applies non-bold/bright magenta to foreground
        /// </summary>
        public static string ForegroundMagenta
        {
            get { return "\x1b[" + 35 + "m"; }
        }

        /// <summary>
        /// Applies non-bold/bright cyan to foreground
        /// </summary>
        public static string ForegroundCyan
        {
            get { return "\x1b[" + 36 + "m"; }
        }

        /// <summary>
        /// Applies non-bold/bright white to foreground
        /// </summary>
        public static string ForegroundWhite
        {
            get { return "\x1b[" + 37 + "m"; }
        }

        /// <summary>
        /// Applies extended color value to the foreground (see details below)
        /// </summary>
        public static string ForegroundExtended
        {
            get { return "\x1b[" + 38 + "m"; }
        }


        /// <summary>
        /// Applies only the foreground portion of the defaults (see 0)
        /// </summary>
        public static string ForegroundDefault
        {
            get { return "\x1b[" + 39 + "m"; }
        }


        /// <summary>
        /// Applies non-bold/bright black to background
        /// </summary>
        public static string BackgroundBlack
        {
            get { return "\x1b[" + 40 + "m"; }
        }

        /// <summary>
        /// Applies non-bold/bright red to background
        /// </summary>
        public static string BackgroundRed
        {
            get { return "\x1b[" + 41 + "m"; }
        }

        /// <summary>
        /// Applies non-bold/bright green to background
        /// </summary>
        public static string BackgroundGreen
        {
            get { return "\x1b[" + 42 + "m"; }
        }

        /// <summary>
        /// Applies non-bold/bright yellow to background
        /// </summary>
        public static string BackgroundYellow
        {
            get { return "\x1b[" + 43 + "m"; }
        }

        /// <summary>
        /// Applies non-bold/bright blue to background
        /// </summary>
        public static string BackgroundBlue
        {
            get { return "\x1b[" + 44 + "m"; }
        }

        /// <summary>
        /// Applies non-bold/bright magenta to background
        /// </summary>
        public static string BackgroundMagenta
        {
            get { return "\x1b[" + 45 + "m"; }
        }

        /// <summary>
        /// Applies non-bold/bright cyan to background
        /// </summary>
        public static string BackgroundCyan
        {
            get { return "\x1b[" + 46 + "m"; }
        }
        /// <summary>
        /// Applies non-bold/bright white to background
        /// </summary>
        public static string BackgroundWhite
        {
            get { return "\x1b[" + 47 + "m"; }
        }

        /// <summary>
        /// Applies extended color value to the background (see details below)
        /// </summary>
        public static string BackgroundExtended
        {
            get { return "\x1b[" + 48 + "m"; }
        }

        /// <summary>
        /// Applies only the background portion of the defaults (see 0)
        /// </summary>
        public static string BackgroundDefault
        {
            get { return "\x1b[" + 49 + "m"; }
        }

        /// <summary>
        /// Applies bold/bright black to foreground
        /// </summary>
        public static string BrightForegroundBlack
        {
            get { return "\x1b[" + 90 + "m"; }
        }

        /// <summary>
        /// Applies bold/bright red to foreground
        /// </summary>
        public static string BrightForegroundRed
        {
            get { return "\x1b[" + 91 + "m"; }
        }

        /// <summary>
        /// Applies bold/bright green to foreground
        /// </summary>
        public static string BrightForegroundGreen
        {
            get { return "\x1b[" + 92 + "m"; }
        }

        /// <summary>
        /// Applies bold/bright yellow to foreground
        /// </summary>
        public static string BrightForegroundYellow
        {
            get { return "\x1b[" + 93 + "m"; }
        }

        /// <summary>
        /// Applies bold/bright blue to foreground
        /// </summary>
        public static string BrightForegroundBlue
        {
            get { return "\x1b[" + 94 + "m"; }
        }

        /// <summary>
        /// Applies bold/bright magenta to foreground
        /// </summary>
        public static string BrightForegroundMagenta
        {
            get { return "\x1b[" + 95 + "m"; }
        }

        /// <summary>
        /// Applies bold/bright cyan to foreground
        /// </summary>
        public static string BrightForegroundCyan
        {
            get { return "\x1b[" + 96 + "m"; }
        }

        /// <summary>
        /// Applies bold/bright white to foreground
        /// </summary>
        public static string BrightForegroundWhite
        {
            get { return "\x1b[" + 97 + "m"; }
        }

        /// <summary>
        /// Applies bold/bright black to background
        /// </summary>
        public static string BrightBackgroundBlack
        {
            get { return "\x1b[" + 100 + "m"; }
        }

        /// <summary>
        /// Applies bold/bright red to background
        /// </summary>
        public static string BrightBackgroundRed
        {
            get { return "\x1b[" + 101 + "m"; }
        }

        /// <summary>
        /// Applies bold/bright green to background
        /// </summary>
        public static string BrightBackgroundGreen
        {
            get { return "\x1b[" + 102 + "m"; }
        }

        /// <summary>
        /// Applies bold/bright yellow to background
        /// </summary>
        public static string BrightBackgroundYellow
        {
            get { return "\x1b[" + 103 + "m"; }
        }

        /// <summary>
        /// Applies bold/bright blue to background
        /// </summary>
        public static string BrightBackgroundBlue
        {
            get { return "\x1b[" + 104 + "m"; }
        }

        /// <summary>
        /// Applies bold/bright magenta to background
        /// </summary>
        public static string BrightBackgroundMagenta
        {
            get { return "\x1b[" + 105 + "m"; }
        }

        /// <summary>
        /// Applies bold/bright cyan to background
        /// </summary>
        public static string BrightBackgroundCyan
        {
            get { return "\x1b[" + 106 + "m"; }
        }

        /// <summary>
        /// Applies bold/bright white to background
        /// </summary>
        public static string BrightBackgroundWhite
        {
            get { return "\x1b[" + 107 + "m"; }
        }

        /// <summary>
        /// Applies a blinking animation to text.
        /// </summary>
        public static string Blinking
        {
            get { return "\x1b[" + 5 + "m"; }
        }

        /// <summary>
        /// Applies strikethrough to the foreground.
        /// </summary>
        public static string StrikeThrough
        {
            get { return "\x1b[" + 9 + "m"; }
        }

        /// <summary>
        /// Removes strikethrough to the foreground.
        /// </summary>
        public static string RemoveStrikeThrough
        {
            get { return "\x1b[" + 29 + "m"; }
        }

        /// <summary>
        /// Gets a color sequence string based on the given color that changes the foreground color.
        /// </summary>
        /// <param name="c">The given color.</param>
        /// <returns></returns>
        public static string GetColorForegroundSequence(Color c)
        {
            return $"\x1b[38;2;{c.R};{c.G};{c.B}m";
        }

        /// <summary>
        /// Gets a color sequence string based on the given color that changes the foreground color.
        /// </summary>
        /// <param name="c">The given color.</param>
        /// <returns></returns>
        public static string GetColorBackgroundSequence(Color c)
        {
            return $"\x1b[48;2;{c.R};{c.G};{c.B}m";
        }

        /// <summary>
        /// Strips instances of ansi codes from the given string.
        /// </summary>
        /// <param name="str">The string to strip ansi codes or sequences from.</param>
        /// <returns>Returns the text remaining of the input string with the ansi codes removed.</returns>
        public static string StripAnsiCodes(string str)
        {
            string result = Regex.Replace(str, ansiMatcherRegex, "");
            return result;
        }

        /// <summary>
        /// Gets a specific ansi code with the given number.
        /// </summary>
        public static string GetAnsiCode(int index)
        {
             return "\x1b[" + index + "m";
        }
    }
}

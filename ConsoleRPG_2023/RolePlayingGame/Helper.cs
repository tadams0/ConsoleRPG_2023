using ConsoleRPG_2023.RolePlayingGame.Text;
using CustomConsoleColors;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace ConsoleRPG_2023.RolePlayingGame
{
    /// <summary>
    /// A static class containing various helpful methods that can be accessed anywhere.
    /// </summary>
    public static class Helper
    {

        public static readonly string ActionExit = "Exit";
        public static readonly string ActionNone = "None";
        public static readonly string ActionBackOrReturn = "Return";

        public static readonly char BoxCharacter = '█';

        public static readonly string NewLineString = "\n";

        /// <summary>
        /// Tag for clearing all text color.
        /// </summary>
        public static readonly string DefaultColorTag = "{color:None}";

        /// <summary>
        /// Tag for clearing all formats afterwards.
        /// </summary>
        public static readonly string DefaultNoneTag = "{None}";

        public static readonly string TagTextColor = "color";
        public static readonly string TagBackgroundColor = "background";
        public static readonly string TagMsPerChar = "mschar";
        public static readonly string TagMsDelay = "msdelay";
        public static readonly string TagMsDelayAfter = "msafter";

        public static readonly ConsoleColor defaultColor = ConsoleColor.Gray;
        public static readonly ConsoleColor defaultBackgroundColor = ConsoleColor.Black;

        public static readonly ConsoleColor defaultSpeechColor = ConsoleColor.White;
        public static readonly ConsoleColor defaultSpeechBackgroundColor = ConsoleColor.DarkGray;

        public static readonly string GameTitle = "[The Tech Demo]";

        /// <summary>
        /// Checks if two strings equal each other ignoring case sensitivity.
        /// </summary>
        public static bool StringEquals(string a, string b)
        {
            return a.ToUpperInvariant().Equals(b.ToUpperInvariant());
        }

        public static InputResult GetInput(bool singleKeyPress = false, int maxSize = 100, bool allowWhiteSpace = true)
        {
            string input = null;
            ConsoleKeyInfo keyInfo = new ConsoleKeyInfo();

            bool validInput = false;

            if (allowWhiteSpace)
            {
                while (!validInput)
                {
                    if (singleKeyPress)
                    {
                        keyInfo = Console.ReadKey(true);
                        input = keyInfo.KeyChar.ToString();
                    }
                    else
                    {
                        input = Console.ReadLine();
                    }

                    if (input.Length > maxSize)
                    {
                        Helper.WriteFormattedString("Your input cannot have more than " + Helper.FormatString(maxSize.ToString(), ConsoleColor.Red) + " characters!");
                    }
                    else
                    {
                        validInput = true;
                    }
                }

                return new InputResult(input, keyInfo);
            }
            else
            {
                while (!validInput)
                {
                    if (singleKeyPress)
                    {
                        keyInfo = Console.ReadKey(true);
                        input = keyInfo.KeyChar.ToString();
                    }
                    else
                    {
                        input = Console.ReadLine();
                    }

                    if (input.Length > maxSize)
                    {
                        Helper.WriteFormattedString("Please type something with less than " + Helper.FormatString(maxSize.ToString(), ConsoleColor.Red) + " characters!");
                    }
                    else if (!string.IsNullOrWhiteSpace(input))
                    {
                        validInput = true;
                    }
                    else
                    {
                        Console.WriteLine("Please type something other than spaces or white space.");
                    }
                }

                return new InputResult(input, keyInfo);
            }
        }
        public static InputResult GetInputOnlyValid(bool trimStrings, bool caseSensitive, params string[] validInputs)
        {
            bool validInput = false;

            while (!validInput)
            {
                string input = Console.ReadLine();

                string modifiedInput = input;
                if (trimStrings)
                {
                    modifiedInput = modifiedInput.Trim();
                }

                if (!caseSensitive)
                {
                    modifiedInput = modifiedInput.ToUpperInvariant();
                }

                string fullValidInputListString = "";
                for (int i = 0; i < validInputs.Length; i++)
                {
                    if (fullValidInputListString != "")
                    {
                        fullValidInputListString += "/";
                    }

                    string validInputString = validInputs[i];
                    fullValidInputListString += validInputString;

                    if (trimStrings)
                    {
                        validInputString = validInputString.Trim();
                    }

                    if (!caseSensitive)
                    {
                        validInputString = validInputString.ToUpperInvariant();
                    }

                    if (validInputString == modifiedInput)
                    {
                        return new InputResult(input);
                    }
                }

                Console.WriteLine("Please select an option from the following: " + fullValidInputListString);
            }

            //Should never make it here.
            return null;
        }

        public static InputResult GetInputNumericOnly(string errorMessage = "That input was not a valid number. Please try again.")
        {
            bool invalidInput = true;
            InputResult inputResult;

            while (invalidInput)
            {
                inputResult = GetInput();

                if (inputResult.IsNumeric)
                {
                    return inputResult;
                }
                else
                {
                    Console.WriteLine(errorMessage);
                }
            }

            return null;
        }

        public static InputResult GetInputWholeNumberOnly(int maxSize = 100, bool singleKeyPress = false, string errorMessage = "That input was not a whole number. Please try again.")
        {
            bool invalidInput = true;
            InputResult inputResult;

            while (invalidInput)
            {
                inputResult = GetInput(singleKeyPress, maxSize, false);

                if (inputResult.IsNumeric && inputResult.NumberOfDecimalPlaces == 0)
                {
                    return inputResult;
                }
                else
                {
                    Console.WriteLine(errorMessage);
                }
            }

            return null;
        }

        public static InputResult GetInputWholeNumberWithRange(int minRange, int maxRange, string errorMessage = "That input was not a whole number. Please try again.")
        {
            bool invalidInput = true;
            InputResult inputResult;

            while (invalidInput)
            {
                inputResult = GetInput();

                if (inputResult.IsNumeric && GetDecimalPlaces(inputResult.Numeric) == 0)
                {
                    if (inputResult.Numeric >= minRange && inputResult.Numeric <= maxRange)
                    {
                        return inputResult;
                    }
                    else
                    {
                        Console.WriteLine($"The input was not in the range of {minRange}-{maxRange}");
                    }
                }
                else
                {
                    Console.WriteLine(errorMessage);
                }
            }

            return null;
        }


        /// <summary>
        /// Converts a <see cref="Race"/> type to its display name.
        /// </summary>
        /// <param name="race"></param>
        /// <returns></returns>
        public static string GetRaceName(Race race)
        {
            string raceName = race.ToString();

            raceName = raceName.Replace("_", " ");

            return raceName;
        }

        /// <summary>
        /// Gets an array containing all races of the game.
        /// </summary>
        /// <returns></returns>
        public static Race[] GetAllRaces()
        {
            return (Race[])Enum.GetValues(typeof(Race));
        }
        
        /// <summary>
        /// Gets an array containing all playable races.
        /// </summary>
        /// <returns></returns>
        public static Race[] GetPlayableRaces()
        {
            List<Race> races = new List<Race>();

            races.Add(Race.Human);
            races.Add(Race.Orc);
            races.Add(Race.Dwarf);
            races.Add(Race.High_Elf);

            return races.ToArray();
        }

        public static ConsoleColor GetRaceColor(Race race)
        {
            switch (race)
            {
                default:
                case Race.Human: return ConsoleColor.White;
                case Race.Orc: return ConsoleColor.DarkGreen;
                case Race.Dwarf: return ConsoleColor.DarkRed;
                case Race.High_Elf: return ConsoleColor.DarkYellow;
            }
        }

        public static ConsoleColor GetAlternativeRaceColor(Race race)
        {
            switch (race)
            {
                default:
                case Race.Human: return ConsoleColor.Gray;
                case Race.Orc: return ConsoleColor.Green;
                case Race.Dwarf: return ConsoleColor.Red;
                case Race.High_Elf: return ConsoleColor.Yellow;
            }
        }

        /// <summary>
        /// Writes a string formatted with meta tags to the console.
        /// </summary>
        /// <param name="message">The given string.</param>
        public static void WriteFormattedString(string message, bool newLine = true)
        {
            TextWriter writer = new TextWriter();

            writer.WriteFormattedText(message, newLine);
        }

        /// <summary>
        /// Gets the size in characters that the formatted string would appear on the display.
        /// </summary>
        public static int GetFormattedStringDisplaySize(string formattedString)
        {
            TextWriter writer = new TextWriter();
            return writer.CountDisplaySize(formattedString);
        }

        public static string FormatString(string text, Color foregroundColor)
        {
            string colorTag = VirtualConsoleSequenceBuilder.GetColorForegroundSequence(foregroundColor);
            return colorTag + text + VirtualConsoleSequenceBuilder.Default;
        }

        public static string FormatString(string text, Color foregroundColor, Color backgroundColor)
        {
            string colorTag = VirtualConsoleSequenceBuilder.GetColorForegroundSequence(foregroundColor);
            colorTag += VirtualConsoleSequenceBuilder.GetColorBackgroundSequence(backgroundColor);
            return colorTag + text + VirtualConsoleSequenceBuilder.Default;
        }

        public static string FormatString(string text, ConsoleColor color)
        {
            string colorTag = $"{{{BuildFormatTag(color.ToString(), Helper.TagTextColor)}}}";

            //Would look something like [color:DARK_YELLOW]This is some text[None]
            return colorTag + text + DefaultNoneTag;
        }
        public static string FormatString(string text, ConsoleColor color, ConsoleColor backgroundColor)
        {
            //Should look something like this: {color:DarkYellow|Background:Cyan}
            string colorTag = $"{{{BuildFormatTag(color.ToString(), Helper.TagTextColor)}|{BuildFormatTag(backgroundColor.ToString(), Helper.TagBackgroundColor)}}}";

            //Would look something like [color:DARK_YELLOW]This is some text[None]
            return colorTag + text + DefaultNoneTag;
        }

        public static string FormatString(string text, ConsoleColor color, ConsoleColor backgroundColor, int msDelay)
        {
            string colorTag = BuildFormatTag(color.ToString(), Helper.TagTextColor);
            string backgroundTag = BuildFormatTag(backgroundColor.ToString(), Helper.TagBackgroundColor);
            string msDelayTag = BuildFormatTag(msDelay.ToString(), Helper.TagMsDelay);

            //Should look something like this: {color:DarkYellow|Background:Cyan}
            string combinedTags = CombineFormatTags(colorTag, backgroundTag, msDelayTag);

            //Would look something like [color:DARK_YELLOW]This is some text[None]
            return combinedTags + text + DefaultNoneTag;
        }
        public static string FormatString(string text, ConsoleColor color, ConsoleColor backgroundColor, int msDelay, int msPerCharacter)
        {
            string colorTag = BuildFormatTag(color.ToString(), Helper.TagTextColor);
            string backgroundTag = BuildFormatTag(backgroundColor.ToString(), Helper.TagBackgroundColor);
            string msDelayTag = BuildFormatTag(msDelay.ToString(), Helper.TagMsDelay);
            string msPerCharacterTag = BuildFormatTag(msPerCharacter.ToString(), Helper.TagMsPerChar);

            string combinedTags = CombineFormatTags(colorTag, backgroundTag, msDelayTag, msPerCharacterTag);

            return combinedTags + text + DefaultNoneTag;
        }
        public static string FormatString(string text, ConsoleColor color, ConsoleColor backgroundColor, int msDelay, int msPerCharacter, int msDelayAfter)
        {
            string colorTag = BuildFormatTag(color.ToString(), Helper.TagTextColor);
            string backgroundTag = BuildFormatTag(backgroundColor.ToString(), Helper.TagBackgroundColor);
            string msDelayTag = BuildFormatTag(msDelay.ToString(), Helper.TagMsDelay);
            string msPerCharacterTag = BuildFormatTag(msPerCharacter.ToString(), Helper.TagMsPerChar);
            string msDelayAfterTag = BuildFormatTag(msDelayAfter.ToString(), Helper.TagMsDelayAfter);

            string combinedTags = CombineFormatTags(colorTag, backgroundTag, msDelayTag, msPerCharacterTag, msDelayAfterTag);

            return combinedTags + text + DefaultNoneTag;
        }
        public static string FormatString(string text, int msDelay, int msPerCharacter, int msDelayAfter)
        {
            string msDelayTag = BuildFormatTag(msDelay.ToString(), Helper.TagMsDelay);
            string msPerCharacterTag = BuildFormatTag(msPerCharacter.ToString(), Helper.TagMsPerChar);
            string msDelayAfterTag = BuildFormatTag(msDelayAfter.ToString(), Helper.TagMsDelayAfter);

            string combinedTags = CombineFormatTags(msDelayTag, msPerCharacterTag, msDelayAfterTag);

            return combinedTags + text + DefaultNoneTag;
        }

        public static string CombineFormatTags(params string[] tags)
        {
            string result = "{";
            for (int i = 0; i < tags.Length; i++)
            {
                result += tags[i];
                if (i != tags.Length - 1)
                {//If we're not on the last iteration of the loop...
                    result += "|";
                }
            }
            result += "}";

            return result;
        }

        public static string BuildFormatTag(string value, string tagType)
        {
            return $"{tagType}:{value}";
        }

        /// <summary>
        /// From: https://stackoverflow.com/questions/13477689/find-number-of-decimal-places-in-decimal-value-regardless-of-culture
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        public static int GetDecimalPlaces(decimal n)
        {
            return n.ToString(System.Globalization.CultureInfo.InvariantCulture)
                        //.TrimEnd('0') uncomment if you don't want to count trailing zeroes
                        .SkipWhile(c => c != '.')
                        .Skip(1)
                        .Count();
            /*
            n = Math.Abs(n); //make sure it is positive.
            n -= (long)n;     //remove the integer part of the number.
            var decimalPlaces = 0;
            while (n > 0)
            {
                decimalPlaces++;
                n *= 10;
                n -= (long)n;
            }
            return decimalPlaces;
            */
        }

        public static Color GetRandomColor(Random r)
        {
            byte red = (byte)r.Next(0, 256);
            byte green = (byte)r.Next(0, 256);
            byte blue = (byte)r.Next(0, 256);
            return Color.FromArgb(red, green, blue);
        }

        /// <summary>
        /// Converts the first character in the given string to an upper case character.
        /// </summary>
        /// <param name="text">The text to be modified.</param>
        /// <returns>The same string with the first character converted to an upper case character.</returns>
        public static string FirstCharToUpper(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return text;

            return text[0].ToString().ToUpperInvariant() + text.Substring(1);
        }

    }
}

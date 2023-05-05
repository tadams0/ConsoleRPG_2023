using CustomConsoleColors;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleRPG_2023.RolePlayingGame
{
    /// <summary>
    /// Defines a class used to display a set list of options on the screen.
    /// </summary>
    public class OptionDisplay
    {

        /// <summary>
        /// The display string built from given options.
        /// </summary>
        public string OptionDisplayString 
        { 
            get { return options; } 
        }

        public int NumberEmptyLinesAfterOption
        {
            get { return numberOfEmptyLinesAfterEachOption; }
            set { numberOfEmptyLinesAfterEachOption = value; }
        }

        public int NumberEmptyLinesBetweenAdditionalInfo
        {
            get { return numberOfEmptyLinesBetweenAdditionalInfo; }
            set { numberOfEmptyLinesBetweenAdditionalInfo = value; }
        }

        /// <summary>
        /// Gets or sets the color of the option number.
        /// <br/>
        /// <br/>
        /// 1) Option
        /// <br/>
        /// ^ the color of that.
        /// </summary>
        public ConsoleColor OptionNumberColor
        {
            get { return optionNumberColor; }
        }

        private string options = "";

        private string newLineString = "\n";

        private string numberSeparator = ")";
        private string numberSpacing = " ";

        private string additionalInfoSpacing = "     ";

        private int numberOfEmptyLinesAfterEachOption = 1;

        private int numberOfEmptyLinesBetweenAdditionalInfo = 1;

        private ConsoleColor optionNumberColor = ConsoleColor.White;

        /// <summary>
        /// A mapping containing the individual options. The key is the number for the option. The value is the string value (excluding separators, numbers, and the spacing).
        /// </summary>
        private Dictionary<int, string> individualOptions = new Dictionary<int, string>();

        /// <summary>
        /// A mapping containing additional information tied to an individual option.
        /// </summary>
        private Dictionary<int, string> optionAdditionalInfo = new Dictionary<int, string>();

        /// <summary>
        /// A mapping containing the option number as the key, with the value being a function delegate returning a <see cref="MenuResult"/> and accepting in an <see cref="InputResult"/>.
        /// </summary>
        private Dictionary<int, Func<InputResult, MenuResult>> optionNumberToDelegate = new Dictionary<int, Func<InputResult, MenuResult>>();

        public OptionDisplay() 
        { 
        }

        /// <summary>
        /// Gets the method callback for the given option number.
        /// </summary>
        /// <param name="number">The given option number.</param>
        /// <returns>Returns the delegate associated with the given number.</returns>
        public Func<InputResult, MenuResult> GetCallback(int number)
        {
            Func<InputResult, MenuResult> callback;
            bool successful = optionNumberToDelegate.TryGetValue(number, out callback);
            return callback;
        }

        /// <summary>
        /// Determines if the given number is a valid option for this menu.
        /// </summary>
        /// <param name="number">The option number.</param>
        /// <returns>Returns true if the given option number exists within the menu.</returns>
        public bool HasOption(int number)
        {
            return individualOptions.ContainsKey(number);
        }

        /// <summary>
        /// Adds a new option to the list of display options.
        /// </summary>
        public void AddOption(string option, int number, Func<InputResult, MenuResult> methodCallback)
        {
            if (options.Length != 0)
            {
                for (int i = 0; i < numberOfEmptyLinesAfterEachOption; i++)
                {
                    options += newLineString;
                }
            }
            options += Helper.FormatString(number.ToString(), optionNumberColor) + numberSeparator + numberSpacing + option;

            individualOptions[number] = option;
            optionNumberToDelegate[number] = methodCallback;
        }

        /// <summary>
        /// Adds a new option to the list of display options.
        /// </summary>
        /// <param name="additionalInfo">A string containing additional information </param>
        public void AddOption(string option, int number, string additionalInfo, Func<InputResult, MenuResult> methodCallback)
        {
            this.AddOption(option, number, methodCallback);

            for (int i = 0; i < numberOfEmptyLinesBetweenAdditionalInfo; i++)
            {
                options += newLineString;
            }

            options += additionalInfoSpacing + additionalInfo;

            optionAdditionalInfo[number] = additionalInfo;
        }

        /// <summary>
        /// Clears all options from the display.
        /// </summary>
        public void RemoveAllOptions()
        {
            options = "";
            individualOptions.Clear();
            optionAdditionalInfo.Clear();
            optionNumberToDelegate.Clear();
        }

        /// <summary>
        /// Writes the options to the console.
        /// </summary>
        public void WriteToConsole()
        {
            Helper.WriteFormattedString(options);
        }

    }
}

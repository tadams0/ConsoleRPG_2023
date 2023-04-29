using ConsoleRPG_2023.RolePlayingGame;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleRPG_2023
{
    /// <summary>
    /// Defines a user input and the data surrounding that input.
    /// </summary>
    public class InputResult
    {
        /// <summary>
        /// Gets a default cancelled input.
        /// </summary>
        /// <returns></returns>
        public static InputResult GetCancelledInput()
        {
            return new InputResult("")
            {
                Canceled = true
            };
        }

        /// <summary>
        /// The numeric value of the input. If this input was not numeric then this value is unreliable.
        /// </summary>
        public decimal Numeric { get; private set; }

        /// <summary>
        /// The numeric value as an integer. If the numeric's real value is above or below the max value of an int, then this will hold the highest or lowest value it can.
        /// </summary>
        public int NumericAsInt { get; private set; }

        /// <summary>
        /// The number of decimal places the numeric value has.
        /// </summary>
        public int NumberOfDecimalPlaces { get; private set; }

        /// <summary>
        /// Determines with the numeric value of the input is a proper integer and falls within its range.
        /// </summary>
        public bool NumericIsInt
        {
            get { return IsNumeric && NumberOfDecimalPlaces == 0 && Numeric < int.MaxValue && Numeric > int.MinValue; }
        }

        /// <summary>
        /// Determines whether the numeric result comes from a successful parse. True if a parse was successful, false if it was not.
        /// </summary>
        public bool IsNumeric { get; private set; }

        /// <summary>
        /// The plain-text version of the input.
        /// </summary>
        public string Text { get; private set; }

        /// <summary>
        /// The console key information related to single press inputs. Will generally be null if a single input is not used.
        /// </summary>
        public ConsoleKeyInfo Key { get; private set; }

        /// <summary>
        /// Determines whether the input result is from a cancelled input.
        /// </summary>
        public bool Canceled { get; private set; } = false;

        private bool ReplaceCommasForNumbers = true;

        public InputResult(ConsoleKeyInfo key)
        {
            Key = key;
            Setup(key.KeyChar.ToString());
        }

        public InputResult(string textResult) 
        {
            Setup(textResult);
        }

        public InputResult(string textResult, ConsoleKeyInfo key)
        {
            Key = key;
            Setup(textResult);
        }

        private void Setup(string textResult)
        {
            Text = textResult;

            //Parse the decimal type from the text.

            string scrubbedNumeric = textResult;

            //Because numeric types like 33,33 is accepted, we must scrub the comma out or replace it with a period.
            if (ReplaceCommasForNumbers)
            {
                scrubbedNumeric = textResult.Replace(",", ".");
            }

            decimal numeric;
            IsNumeric = decimal.TryParse(scrubbedNumeric, out numeric);
            Numeric = numeric;

            if (IsNumeric)
            {
                //Getting the number of decimal places this input has.
                NumberOfDecimalPlaces = Helper.GetDecimalPlaces(numeric);

                //To convert the decimal to an int, we must first cap it's maximum and minimum values to that of an 32-bit integer.

                //Note that the lossy decimal is really only for int or below data type casting.
                //The lossless version will always be kept.
                //Capping the maximum value.
                decimal lossyDecimal = Math.Min(numeric, int.MaxValue);

                //Capping the minimum value
                lossyDecimal = Math.Max(lossyDecimal, int.MinValue);

                //Finally casting it.
                NumericAsInt = (int)lossyDecimal;
            }
        }

    }
}

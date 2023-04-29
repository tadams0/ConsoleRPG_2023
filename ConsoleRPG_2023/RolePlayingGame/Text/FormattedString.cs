using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace ConsoleRPG_2023.RolePlayingGame.Text
{
    /// <summary>
    /// A simple pairing of console color and text.
    /// </summary>
    public class FormattedString
    {
        public ConsoleColor TextColor { get; set; } = Helper.defaultColor;

        public ConsoleColor BackgroundColor { get; set; } = Helper.defaultBackgroundColor;

        public string Text { get; set; } = string.Empty;

        public int MillisecondBeforeWriting { get; set; } = 0;
        public int MillisecondAfterWriting { get; set; } = 0;

        public int MillisecondsBetweenEachCharacter { get; set; } = 0;

        public FormattedString(string text, ConsoleColor textColor, int msBeforeWriting = 0, int msAfterWriting = 0, int msBetweenCharacters = 0)
        {
            TextColor = textColor;
            Text = text;

            MillisecondBeforeWriting = msBeforeWriting;
            MillisecondsBetweenEachCharacter = msBetweenCharacters;
            MillisecondAfterWriting = msAfterWriting;
        }

        /// <summary>
        /// Writes the formatted text to the console.
        /// </summary>
        public void Write()
        {
            Console.BackgroundColor = BackgroundColor;
            Console.ForegroundColor = TextColor;
            if (MillisecondBeforeWriting > 0)
            {//Pause the thread for x milliseconds before continuing
                Thread.Sleep(MillisecondBeforeWriting);
            }

            if (MillisecondsBetweenEachCharacter > 0) 
            { //If there is a timer between each character, we'll simply have to write each character individually.
                for (int i = 0; i < Text.Length; i++)
                {
                    //Write the character
                    Console.Write(Text[i]);

                    //Sleep for x milliseconds.
                    Thread.Sleep(MillisecondsBetweenEachCharacter);
                }
            }
            else
            {
                //Otherwise we can simply write the text in bulk, instantly.
                Console.Write(Text);
            }

            Console.BackgroundColor = Helper.defaultBackgroundColor;
            Console.ForegroundColor = Helper.defaultColor;

            if (MillisecondAfterWriting > 0)
            {//Pause the thread for x milliseconds before continuing
                Thread.Sleep(MillisecondAfterWriting);
            }
        }
    }
}

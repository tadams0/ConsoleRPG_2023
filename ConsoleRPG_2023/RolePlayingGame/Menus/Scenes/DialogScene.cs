using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace ConsoleRPG_2023.RolePlayingGame.Menus.Scenes
{
    public class DialogScene : Menu
    {
        private static readonly string characterNameSeparator = ":";
        private static readonly string characterNameSpacing = " ";

        public DialogScene()
        {
            writeEmptyMessage = false;
            writeOptions = false;
            skipMenuInput = true;
        }

        public override InputResult GetMenuInput()
        {
            //We specifically override here so we can only get a single key press for dialog scenes.
            return Helper.GetInputWholeNumberOnly(100, true);
        }

        protected override string CreateMessage()
        {
            return "";
        }

        /// <summary>
        /// Writes the given text to the screen with the specified format, then asks the user for a single line input.
        /// </summary>
        protected virtual void WriteNoReplyDialog(string text)
        {
            Helper.WriteFormattedString(text);
            Helper.GetInput(true);
        }

        /// <summary>
        /// Writes the given text to the screen with the specified format, then asks the user for a single line input.
        /// </summary>
        protected virtual void WriteNoReplyDialog(string text, ConsoleColor textColor, ConsoleColor backgroundColor, int msDelay, int msPerChar, int msAfter)
        {
            string str = $"{Helper.FormatString(text, textColor, backgroundColor, msDelay, msPerChar, msAfter)}";
            Helper.WriteFormattedString(str);
            Helper.GetInput(true);
        }

        /// <summary>
        /// Writes the given text to the screen with the specified format, then asks the user for a single line input.
        /// </summary>
        protected virtual void WriteNoReplyDialog(string characterName, string text, ConsoleColor textColor, ConsoleColor backgroundColor, int msDelay, int msPerChar, int msAfter)
        {
            string str = $"{Helper.FormatString(characterName + characterNameSeparator + characterNameSpacing, textColor, backgroundColor, msDelay,0,0)}{Helper.FormatString(text, textColor, backgroundColor, 0, msPerChar, msAfter)}";
            Helper.WriteFormattedString(str);
            Helper.GetInput(true);
        }

    }
}

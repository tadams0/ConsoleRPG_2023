using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleRPG_2023.RolePlayingGame.Text
{
    /// <summary>
    /// Defines a class that writes text in specially formatted ways.
    /// </summary>
    public class TextWriter
    {

        private readonly char tagOpeningChar = '{';
        private readonly char tagClosingChar = '}';

        private readonly string tagSeparator = ":";
        private readonly string multiTagSeparator = "|";

        private readonly string foregroundColorTagStart = "color";


        public void WriteFormattedText(string text, bool endLine = true)
        {
            List<FormattedString> formattedStrings = GetFormattedStrings(text);

            for (int i = 0; i < formattedStrings.Count; i++)
            {
                formattedStrings[i].Write();
            }
            if (endLine)
            {
                Console.WriteLine();
            }
        }

        public int CountDisplaySize(string text)
        {
            //So the lazy way here is to just use GetFormattedStrings and count up all the text of each formatted string object.
            //This works because those objects don't include the tags and parts that don't get shown on screen.

            int count = 0;
            List<FormattedString> strings = GetFormattedStrings(text);
            foreach (FormattedString formattedString in strings)
            {
                count += formattedString.Text.Length;
            }
            return count;
        }

        public List<FormattedString> GetFormattedStrings(string text)
        {
            List<FormattedString> formattedStrings = new List<FormattedString>();

            string tagText = "";
            string remainingText = text;

            int startIndex;
            int endIndex;

            string tagValue;
            string tagType;

            ConsoleColor foregroundColor;
            ConsoleColor backgroundColor;
            FormattedString str;
            while (!string.IsNullOrEmpty(remainingText))
            {
                foregroundColor = Helper.defaultColor;
                backgroundColor = Helper.defaultBackgroundColor;

                tagText = TryGetFirstTag(remainingText, out startIndex, out endIndex);

                str = new FormattedString(remainingText, Helper.defaultColor);

                if (tagText == null)
                {//No tag text, no formatting. Just use the defaults.
                    formattedStrings.Add(str);
                    break;
                }
                else if (startIndex > 0)
                {//Capture the first part into a default string, basically everything before the first tag.
                    string textBeforeFirstTag = remainingText.Substring(0, startIndex);
                    str.Text = textBeforeFirstTag;
                    formattedStrings.Add(str);

                    //Create a new formatted string now for the formatted portion AFTER the first tag.
                    str = new FormattedString(remainingText, Helper.defaultColor);
                }

                string[] multiTagSplit = tagText.Split(multiTagSeparator);
                if (multiTagSplit.Length > 0)
                {//There were multiple tags in the value
                    for (int i = 0; i < multiTagSplit.Length; i++)
                    {
                        string fullTagText = multiTagSplit[i];
                        tagType = GetTagType(fullTagText, out tagValue);
                        if (!string.IsNullOrEmpty(tagType))
                        {
                            ApplyTag(str, tagType, tagValue);
                        }
                    }
                }

                if (endIndex + 1 < remainingText.Length)
                { //This checks if the tag was at the end or if there was more text.

                    //Removing the tag from the text and all text before the start of the tag.

                    remainingText = remainingText.Substring(endIndex + 1);

                    //Getting the next tag's start point so we can take the text inbetween into the formatted string.

                    tagText = TryGetFirstTag(remainingText, out startIndex, out endIndex);

                    string tagAffectedText = remainingText;

                    if (startIndex >= 0)
                    {
                        tagAffectedText = tagAffectedText.Substring(0, startIndex);

                        //Removing the affected text from the remaining string
                        remainingText = remainingText.Substring(startIndex);
                    }
                    else
                    {//the affected text is the entire remainder, so we can just set the remaining text as empty.
                        remainingText = string.Empty;
                    }

                    str.Text = tagAffectedText;

                    formattedStrings.Add(str);
                }
                else
                {
                    remainingText = string.Empty;
                }

            }

            return formattedStrings;
        }

        public void ApplyTag(FormattedString str, string tagType, string tagValue)
        {
            //trimming
            tagType = tagType.Trim();
            tagValue = tagValue.Trim();
            if (Helper.StringEquals(tagType, "None"))
            {//Any "None" tag types will result in stopping any values from the tags.
                return;
            }
            else if (Helper.StringEquals(tagType, "Color")) //foreground / text color
            {//It's a color string.
                ConsoleColor foregroundColor;
                if (Helper.StringEquals("None", tagValue))
                {//special case for "none" color.
                    foregroundColor = Helper.defaultColor;
                    str.TextColor = foregroundColor;
                }
                else
                {
                    bool result = TryParseColorTag(tagValue, out foregroundColor);
                    if (result)
                    {
                        str.TextColor = foregroundColor;
                    }
                    else
                    {
                        throw new Exception($"No valid color for tag {tagType} with value {tagValue}!");
                    }
                }
            }
            else if (Helper.StringEquals(tagType, "background")) //background color
            {
                ConsoleColor backgroundColor;
                if (Helper.StringEquals("None", tagValue))
                {//special case for "none" color.
                    backgroundColor = Helper.defaultColor;
                    str.BackgroundColor = backgroundColor;
                }
                else
                {
                    bool result = TryParseColorTag(tagValue, out backgroundColor);
                    if (result)
                    {
                        str.BackgroundColor = backgroundColor;
                    }
                    else
                    {
                        throw new Exception($"No valid color for tag {tagType} with value {tagValue}!");
                    }
                }
            }
            else if (Helper.StringEquals(tagType, "mschar")) //milliseconds per character
            {
                int msPerChar = 0;
                if (!Helper.StringEquals("None", tagValue))
                {
                    bool result = TryParseIntTag(tagValue, out msPerChar);
                    if (!result)
                    {
                        throw new Exception($"No valid integer for tag {tagType} with value {tagValue}!");
                    }
                }
                str.MillisecondsBetweenEachCharacter = msPerChar;
            }
            else if (Helper.StringEquals(tagType, "msdelay")) //milliseconds before writing the text
            {
                int msDelay = 0;
                if (!Helper.StringEquals("None", tagValue))
                {
                    bool result = TryParseIntTag(tagValue, out msDelay);
                    if (!result)
                    {
                        throw new Exception($"No valid integer for tag {tagType} with value {tagValue}!");
                    }
                }
                str.MillisecondBeforeWriting = msDelay;
            }
            else if (Helper.StringEquals(tagType, "msafter")) //milliseconds before writing the text
            {
                int msDelay = 0;
                if (!Helper.StringEquals("None", tagValue))
                {
                    bool result = TryParseIntTag(tagValue, out msDelay);
                    if (!result)
                    {
                        throw new Exception($"No valid integer for tag {tagType} with value {tagValue}!");
                    }
                }
                str.MillisecondAfterWriting = msDelay;
            }
        }

        public bool TryParseIntTag(string tagValue, out int value)
        {
            bool result = int.TryParse(tagValue, out value);

            return result;
        }
        public bool TryParseColorTag(string tagValue, out ConsoleColor color)
        {
            bool result = Enum.TryParse<ConsoleColor>(tagValue, true, out color);

            return result;
        }

        public string GetTagType(string tagText, out string tagValue)
        {//Tags are in the format {color:DARK_YELLOW} or {color:DARK_YELLOW|background:DARK_YELLOW}
            //So if we split on the : then the first/left side is the tag type.

            string[] splitResult = tagText.Split(tagSeparator);
            if (splitResult.Length > 0)
            {
                if (splitResult.Length == 1)
                {//No value
                    tagValue = string.Empty;
                }
                else
                {
                    tagValue = splitResult[1];
                }

                return splitResult[0];
            }

            tagValue = string.Empty;
            return null;
        }

        public string TryGetFirstTag(string text, out int startIndex, out int endIndex)
        {
            //Scan through each character, ignore escape characters and the character aftwards (/n for example)
            //Find brackets "{" for example.

            startIndex = text.IndexOf(tagOpeningChar);
            endIndex = text.IndexOf(tagClosingChar);

            if (startIndex == -1 || endIndex == -1 || startIndex + 1 > text.Length - 1) //If start or end index was not found (so the tag chars were missing)
            {
                return null;
            }

            int tagSubStringLength = endIndex - startIndex - 1;
            if (tagSubStringLength < 0)
            {
                return null;
            }
            string tagSubString = text.Substring(startIndex + 1, tagSubStringLength);

            return tagSubString;
        }



    }
}

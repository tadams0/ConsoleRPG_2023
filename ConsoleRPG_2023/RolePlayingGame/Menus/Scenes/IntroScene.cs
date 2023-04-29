using ConsoleRPG_2023.RolePlayingGame.Text;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace ConsoleRPG_2023.RolePlayingGame.Menus.Scenes
{
    public class IntroScene : DialogScene
    {
        public IntroScene()
        {
            writeEmptyMessage = false;
        }

        public override MenuResult Update(InputResult input, OptionDisplay usedDisplay)
        {
            MenuResult result = base.Update(input, usedDisplay);

            string str = $"{Helper.FormatString("Well, not even last night's storm could wake you. I heard them say we've reached", Helper.defaultSpeechColor, Helper.defaultSpeechBackgroundColor, 0,40,0)}{Helper.FormatString("....", Helper.defaultSpeechColor, Helper.defaultSpeechBackgroundColor, 300, 200, 300)}";
            Helper.WriteFormattedString(str);

            str = (Helper.FormatString("Huh.. I heard them say we've reached...", Helper.defaultSpeechColor, Helper.defaultSpeechBackgroundColor, 0, 40, 500));
            Helper.WriteFormattedString(str);

            str = Helper.FormatString("That's strange. I can't seem to say it, but I'm sure they'll let us go.", Helper.defaultSpeechColor, Helper.defaultSpeechBackgroundColor, 0, 30, 1000);
            Helper.WriteFormattedString(str);

            WriteNoReplyDialog("Guard", "This is where you get off.", ConsoleColor.DarkBlue, Helper.defaultSpeechBackgroundColor, 0, 30, 1000);

            WriteNoReplyDialog(Helper.FormatString("...", 0, 50, 0));

            WriteNoReplyDialog(Helper.FormatString("...", 0, 100, 0));

            WriteNoReplyDialog(Helper.FormatString("...", 0, 150, 0));

            Console.Clear();

            result.Action = "MapExploreMenu";

            return result;
        }


    }
}

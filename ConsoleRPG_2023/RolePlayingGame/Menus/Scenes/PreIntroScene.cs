using ConsoleRPG_2023.RolePlayingGame.Text;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace ConsoleRPG_2023.RolePlayingGame.Menus.Scenes
{
    public class PreIntroScene : DialogScene
    {
        public PreIntroScene()
        {
            writeEmptyMessage = false;
        }

        public override MenuResult Update(InputResult input, OptionDisplay usedDisplay)
        {
            MenuResult result = base.Update(input, usedDisplay);

            string str = $"You will be required to press any key to progress promptless screens. Pressing any key more than once per section or while text is being written may result in skipped text.{NewLineString}{NewLineString}Please press {Helper.FormatString("any key", ConsoleColor.Green)} to continue.";

            Helper.WriteFormattedString(str);

            Helper.GetInput(true);

            WriteNoReplyDialog(Helper.FormatString("...", 0, 50, 0));

            WriteNoReplyDialog(Helper.FormatString("...", 0, 100, 0));

            WriteNoReplyDialog(Helper.FormatString("...", 0, 200, 0));

            Console.Clear();

            WriteNoReplyDialog("Wake up, we're here. Why are you shaking? Are you ok? Wake up.", Helper.defaultSpeechColor, Helper.defaultSpeechBackgroundColor, 0, 50, 0);

            Console.Clear();

            WriteNoReplyDialog("Stand up... there you go. You were dreaming. What's your name?", Helper.defaultSpeechColor, Helper.defaultSpeechBackgroundColor, 0, 60, 0);

            result.Action = "CharacterCreator";
            return result;
        }


    }
}

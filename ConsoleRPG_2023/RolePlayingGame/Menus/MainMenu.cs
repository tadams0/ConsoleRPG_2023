using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleRPG_2023.RolePlayingGame.Menus
{
    internal class MainMenu : Menu
    {

        protected override OptionDisplay CreateOptions()
        {
            OptionDisplay options = base.CreateOptions();


            options.AddOption("New Game", 1, x => NewGame(x));
            options.AddOption("Load Game", 2, x => LoadGame(x));
            options.AddOption("Settings", 3, x => Settings(x));
            options.AddOption("Exit", 4, x => Exit(x));
            options.AddOption("TEST Inventory Menu", 5, x => InventoryMenuTest(x));

            return options;
        }

        protected override string CreateMessage()
        {
            return $"Welcome to {Helper.FormatString(Helper.GameTitle, ConsoleColor.Magenta)}.{NewLineString}{NewLineString}Please select one of the options below by typing the number to the left and pressing enter.";
        }

        private MenuResult NewGame(InputResult input)
        {
            MenuResult result = new MenuResult();
            if (RPGGame.Settings.SkipIntro)
            {
                result.Action = "CharacterCreator";
            }
            else
            {
                result.Action = "PreIntroScene";
            }
            return result;
        }
        private MenuResult LoadGame(InputResult input)
        {
            MenuResult result = new MenuResult();
            //TODO
            return result;
        }

        private MenuResult Settings(InputResult input)
        {
            MenuResult result = new MenuResult("GameSettingsMenu");
            return result;
        }

        private MenuResult Exit(InputResult input)
        {
            MenuResult result = new MenuResult();
            result.Action = Helper.ActionExit;
            return result;
        }
        private MenuResult InventoryMenuTest(InputResult input)
        {
            MenuResult result = new MenuResult();
            result.Action = "InventoryMenu";
            return result;
        }

        public override MenuResult Update(InputResult input, OptionDisplay usedDisplay)
        {
            MenuResult result = base.Update(input, usedDisplay);

            if (!usedDisplay.HasOption(input.NumericAsInt))
            {//Custom message for when the player selects something that wasn't within the main menu.
                result.CustomMessage = "Please select a valid number from 1-4";
                result.Action = Helper.ActionNone;
            }

            return result;
        }
    }

}

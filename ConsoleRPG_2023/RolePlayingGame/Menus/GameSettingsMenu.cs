using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleRPG_2023.RolePlayingGame.Menus
{
    public class GameSettingsMenu : Menu
    {
        private ConsoleColor goodColor = ConsoleColor.Green;
        private ConsoleColor badColor = ConsoleColor.Red;

        private GameSettings settings;

        public GameSettingsMenu() 
        {
            //Setting up the menu behavior
            defaultAction = Helper.ActionNone;

            //We link the game settings by grabbing the reference to the settings object.
            settings = RPGGame.Settings;
        }

        protected override string CreateMessage()
        {
            return "You can modify the below settings by typing the number to the left of them.";
        }

        protected override OptionDisplay CreateOptions()
        {
            OptionDisplay options = base.CreateOptions();

            options.AddOption("Return", 1, x => Return(x));

            ConsoleColor valueColor;
            if (settings.SkipIntro)
            {
                valueColor = goodColor;
            }
            else
            {
                valueColor = badColor;
            }

            options.AddOption("Skip Intro | " + Helper.FormatString(settings.SkipIntro.ToString(), valueColor), 2, x => SkipIntro(x));

            if (settings.AutoPopulateNameOnNewGame)
            {
                valueColor = goodColor;
            }
            else
            {
                valueColor = badColor;
            }

            options.AddOption("Auto Populate Name on New Game | " + Helper.FormatString(settings.AutoPopulateNameOnNewGame.ToString(), valueColor), 3, x => AutoGenerateNameOnNewGame(x));

            if (settings.MapRenderOptimizations)
            {
                valueColor = goodColor;
            }
            else
            {
                valueColor = badColor;
            }

            options.AddOption("Map Render Optimizations On | " + Helper.FormatString(settings.MapRenderOptimizations.ToString(), valueColor), 4, x => MapRenderOptimizations(x));

            return options;
        }

        private MenuResult SkipIntro(InputResult x)
        {
            //Flips the skip intro setting.
            settings.SkipIntro = !settings.SkipIntro;

            return new MenuResult(Helper.ActionNone);
        }
        private MenuResult AutoGenerateNameOnNewGame(InputResult x)
        {
            //Flips the setting.
            settings.AutoPopulateNameOnNewGame = !settings.AutoPopulateNameOnNewGame;

            return new MenuResult(Helper.ActionNone);
        }

        private MenuResult MapRenderOptimizations(InputResult x)
        {
            //Flips the setting.
            settings.MapRenderOptimizations = !settings.MapRenderOptimizations;

            return new MenuResult(Helper.ActionNone);
        }


        private MenuResult Return(InputResult x)
        {

            return new MenuResult(Helper.ActionBackOrReturn);
        }
    }
}

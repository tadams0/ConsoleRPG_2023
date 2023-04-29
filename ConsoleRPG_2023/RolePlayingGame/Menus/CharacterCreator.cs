using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Linq;

namespace ConsoleRPG_2023.RolePlayingGame.Menus
{
    public class CharacterCreator : Menu
    {

        private Character player;

        private int minOptionValue = 1;
        private int maxOptionValue = 3;

        public CharacterCreator()
        {
        }

        protected override OptionDisplay CreateOptions()
        {
            OptionDisplay options = base.CreateOptions();

            //Storing the player variable for later use.
            player = GameState.PlayerCharacter;

            if (RPGGame.Settings.AutoPopulateNameOnNewGame)
            {
                player.FirstName = RandomGenerator.GenerateRandomFirstName(player.Race, true);
                player.LastName = RandomGenerator.GenerateRandomFirstName(player.Race, true);
            }

            //Name select option
            string selectNameOption = "Select a name.";

            string name = player.FirstName + " " + player.LastName;
            if (!string.IsNullOrWhiteSpace(name))
            {
                selectNameOption += $" | Selected: {Helper.FormatString(name, ConsoleColor.White)}";
            }

            options.AddOption(selectNameOption, 1, x => SelectName(x));

            //Race select option

            string selectRaceOption = "Select a race.";

            Race race = player.Race;
            if (race != Race.None)
            {
                string raceName = Helper.GetRaceName(race);
                ConsoleColor raceColor = Helper.GetRaceColor(race);

                selectRaceOption += $" | Selected: {Helper.FormatString(raceName, raceColor)}";
            }

            options.AddOption(selectRaceOption, 2, x => SelectRace(x));


            options.AddOption("Complete", 3, x => Complete(x));

            return options;
        }

        protected override string CreateMessage()
        {
            return "Please fill out the information below before continuing.";
        }

        public override MenuResult Update(InputResult input, OptionDisplay usedDisplay)
        {
            //Cache the player into a variable before we do any updates in the options methods.
            player = GameState.PlayerCharacter;

            MenuResult result = base.Update(input, usedDisplay);

            if (input.IsNumeric && (input.Numeric < minOptionValue || input.Numeric > maxOptionValue))
            {
                result.CustomMessage = $"Please type a {Helper.FormatString("whole", ConsoleColor.Green)} number within the range of {Helper.FormatString(minOptionValue.ToString() + "-" + maxOptionValue.ToString(), ConsoleColor.Green)}.";
                result.Action = Helper.ActionNone;
            }

            return result;
        }

        private MenuResult Complete(InputResult input)
        {
            //To complete the name, they must not have a null or whitespace name.
            bool completedName = !string.IsNullOrWhiteSpace(player.FirstName + " " + player.LastName);

            //To complete the race, they must have selected something other than none.
            bool completedRace = player.Race != Race.None;

            //Generate a custom message to help the player realize what they messed up.
            string customMsg = string.Empty;
            if (!completedName)
            {
                customMsg += "You have not selected a name. ";
            }
            if (!completedRace)
            {
                customMsg += "You have not selected a race. ";
            }

            MenuResult result = new MenuResult(Helper.ActionNone);
            result.CustomMessage = customMsg;

            if (string.IsNullOrWhiteSpace(customMsg))
            {//No message to send, then the user completed things.
                if (!RPGGame.Settings.SkipIntro)
                {
                    result.Action = "IntroScene";
                }
                else
                {
                    result.Action = "MapExploreMenu";
                }
            }

            return result;
        }

        private MenuResult SelectName(InputResult input)
        {
            bool loopName = true;

            string firstName = string.Empty;
            string lastName = string.Empty;

            while (loopName)
            {
                Helper.WriteFormattedString("Please type your first name: ", false);

                InputResult result = Helper.GetInput(false, 50, false);

                firstName = result.Text;

                Helper.WriteFormattedString("Please type your last name: ", false);

                result = Helper.GetInput(false, 50,false);

                lastName = result.Text;

                Helper.WriteFormattedString($"Are you sure you want your name to be {Helper.FormatString(firstName + " " + lastName, ConsoleColor.DarkRed)} ? ({Helper.FormatString("Yes/No", ConsoleColor.Green)}) ");

                result = Helper.GetInputOnlyValid(true, false, "Yes", "No");

                //We can exit the loop is they say yes, so if they said no, we continue the loop.
                loopName = Helper.StringEquals(result.Text, "No");
            }

            player.FirstName = firstName;
            player.LastName = lastName;

            return new MenuResult(Helper.ActionNone);
        }

        private MenuResult SelectRace(InputResult input)
        {
            Helper.WriteFormattedString("Please select one of the below races:");

            OptionDisplay raceOptionDisplay = new OptionDisplay();
            raceOptionDisplay.NumberEmptyLinesAfterOption = 2;

            //Gets all the playable races, then sorts them by their value (x => x) then converts the sorted result to an array.
            Race[] races = Helper.GetPlayableRaces().OrderBy(x => x).ToArray();

            //Building out the race option display.
            for (int i = 0; i < races.Length; i++) 
            {
                Race race = races[i];

                string raceName = Helper.GetRaceName(race);

                string description = GetRaceDescription(race);

                int raceNumber = (int)race;

                string formattedRaceName = Helper.FormatString(raceName, Helper.GetRaceColor(race));

                //No need to add a callback to the options here, we're handling them all within this method.
                raceOptionDisplay.AddOption(formattedRaceName, raceNumber, description, null);
            }

            raceOptionDisplay.WriteToConsole();

            InputResult raceSelection = Helper.GetInputWholeNumberWithRange(1, races.Length);

            Race selectedRace = (Race)raceSelection.Numeric;
            player.Race = selectedRace;

            return new MenuResult(Helper.ActionNone);

        }

        private string GetRaceDescription(Race race)
        {
            switch (race)
            {
                default: return "Invalid race description.";
                case Race.None: return $"Nondescript";
                case Race.Orc: return $"Strong warrior barbarians. They excel in all areas of melee combat. {NewLineString} Orcs have a culture of war and domination. They hold the idea that the mighty have the right to rule over all.";
                case Race.Dwarf: return $"Short but stout race of architects. They are particularly good in heavy melee combat and drinking. {NewLineString} Dwarves have a culture of building complex buildings and art, as well as drinking.";
                case Race.Human: return $"Standard race who are jack of all trades and master of none. {NewLineString} Humans live in large cities and have a culture of sharing knowledge and learning.";
                case Race.High_Elf: return $"Tall magical elven race. They are masters of the magical arts. {NewLineString} High Elves tend to be solitary in nature and mostly focusing on exploring the realm of magic.";
            }
        }


    }
}

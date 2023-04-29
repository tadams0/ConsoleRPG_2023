using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleRPG_2023.RolePlayingGame
{
    public class HUD
    {

        public string HudDisplay
        {
            get { return GetHudDisplay(); }
        }

        private Character character;

        //private string hudDisplay = string.Empty;

        private char barCharacter = '█';

        private ConsoleColor healthBarFillColor = ConsoleColor.DarkGreen;
        private ConsoleColor healthBarEmptyColor = ConsoleColor.DarkGray;
        private ConsoleColor healthBarBackgroundColor = ConsoleColor.DarkGray;

        private ConsoleColor staminaBarFillColor = ConsoleColor.DarkBlue;
        private ConsoleColor staminaBarEmptyColor = ConsoleColor.DarkGray;
        private ConsoleColor staminaBarBackgroundColor = ConsoleColor.DarkGray;

        private int healthBarWidth = 20;
        private int staminaBarWidth = 20;

        private int barLabelWidth = 9;

        public HUD() 
        { 
        }

        public void SetCharacter(Character character)
        {
            this.character = character;
        }

        public string GetHudDisplay()
        {
            string hudDisplay = string.Empty;

            string healthBarDisplay = GetBar(character.Health, character.MaxHealth, healthBarWidth, healthBarFillColor, healthBarEmptyColor, healthBarBackgroundColor);
            string staminaBarDisplay = GetBar(character.Stamina, character.MaxStamina, staminaBarWidth, staminaBarFillColor, staminaBarEmptyColor, staminaBarBackgroundColor);

            string healthBarLabel = "Health: ";
            string staminaBarLabel = "Stamina: ";

            //Now we pad the labels so they are the same number of characters.
            healthBarLabel = healthBarLabel.PadRight(barLabelWidth);
            staminaBarLabel = staminaBarLabel.PadRight(barLabelWidth);


            hudDisplay += healthBarLabel + healthBarDisplay + Helper.NewLineString;
            hudDisplay += staminaBarLabel + staminaBarDisplay + Helper.NewLineString;

            return hudDisplay;
        }

        private string GetBar(int currentValue, int maxValue, int widthInCharacters, ConsoleColor barFillColor, ConsoleColor barEmptyColor, ConsoleColor barBackground)
        {
            if (currentValue < 0)
            {
                currentValue = 0;
            }
            if (maxValue < 0)
            {
                maxValue = 0;
            }
            double percentageBarFilled = (double)currentValue / maxValue;
            int fillWidth = (int)(widthInCharacters * percentageBarFilled + 0.5f); // we can add 0.5f to round to nearest int.
            int unfilledWidth = widthInCharacters - fillWidth;

            string fillBarStr = new string(barCharacter, fillWidth);
            string emptyBarStr = new string(barCharacter, unfilledWidth);

            string formattedFillBar = Helper.FormatString(fillBarStr, barFillColor, barBackground);
            string formattedEmptyBar = Helper.FormatString(emptyBarStr, barEmptyColor, barBackground);

            return $"{formattedFillBar}{formattedEmptyBar}";
        }


    }
}

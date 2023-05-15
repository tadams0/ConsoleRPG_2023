using ConsoleRPG_2023.RolePlayingGame.Items;
using ConsoleRPG_2023.RolePlayingGame.Maps;
using CustomConsoleColors;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace ConsoleRPG_2023.RolePlayingGame
{
    public class Character : MapObject
    {
        /// <summary>
        /// The first name of the character.
        /// </summary>
        public string FirstName { get; set; } = string.Empty;

        /// <summary>
        /// The last name of the character.
        /// </summary>
        public string LastName { get; set; } = string.Empty;


        /// <summary>
        /// The character's race.
        /// </summary>
        public Race Race { get; set; } = Race.None;

        public int MaxHealth { get; set; } = 100;

        public int Health { get; set; } = 100;

        public int MinHealth { get; set; } = 0;

        public int MaxStamina { get; set; } = 100;
        public int Stamina { get; set; } = 100;

        public int MinStamina { get; set; } = 0;

        public bool IsPlayer { get; }

        public Container Inventory
        {
            get { return inventory; }
        }

        private Container inventory;

        public Character(bool isPlayer = false) 
        { 
            IsPlayer = isPlayer;
            inventory = new Container();
        }

        public override string ToString()
        {
            ConsoleColor consoleColor = Helper.GetRaceColor(this.Race);
            Color c = ConsoleColorScheme.GetConsoleColor(consoleColor, false);
            return Helper.FormatString($"{FirstName} {LastName}", c);
        }


    }
}

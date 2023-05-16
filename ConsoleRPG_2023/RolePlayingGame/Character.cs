using ConsoleRPG_2023.RolePlayingGame.Combat;
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

        public int Health { get; protected set; } = 100;

        public int MinHealth { get; set; } = 0;

        public int MaxStamina { get; set; } = 100;
        public int Stamina { get; protected set; } = 100;

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

        /// <summary>
        /// Sets the health of the character and ensures it is within bounds of the minimum and maximum for the character.
        /// </summary>
        /// <param name="health">The health to set to the character.</param>
        public void SetHealth(int health)
        {
            this.Health = Math.Clamp(health, MinHealth, MaxHealth);
        }

        /// <summary>
        /// Sets the stamina of the character and ensures it is within bounds of the minimum and maximum for the character.
        /// </summary>
        /// <param name="stamina">The stamina to set to the character.</param>
        public void SetStamina(int stamina)
        {
            this.Stamina = Math.Clamp(stamina, MinStamina, MaxStamina);
        }

        /// <summary>
        /// Applies the given damage of the given types to the character.
        /// </summary>
        /// <param name="magnitude">The amount (positive to remove health, negative to add health)</param>
        /// <param name="damageTypes">The types of damage being applied.</param>
        public void ApplyDamage(double magnitude, List<DamageType> damageTypes)
        {
            int damageAmount = (int)magnitude;
            SetHealth(Health - damageAmount);
        }

        public override string ToString()
        {
            ConsoleColor consoleColor = Helper.GetRaceColor(this.Race);
            Color c = ConsoleColorScheme.GetConsoleColor(consoleColor, false);
            return Helper.FormatString($"{FirstName} {LastName}", c);
        }


    }
}

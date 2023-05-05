using ConsoleRPG_2023.RolePlayingGame.Items;
using ConsoleRPG_2023.RolePlayingGame.Maps.MapObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleRPG_2023.RolePlayingGame.MenuPayloads
{
    /// <summary>
    /// Defines a class that holds information from the result of a user interacting with the item pickup menu.
    /// </summary>
    public class ItemPickupResult
    {

        public Item Item { get; set; }

        public bool Stolen { get; set; } = false;


    }
}

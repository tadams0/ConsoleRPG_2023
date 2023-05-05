using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleRPG_2023.RolePlayingGame
{
    /// <summary>
    /// Generic object type which is the parent of all game object types. Like characters, enemies, items, etc..
    /// </summary>
    public class GameObject
    {
        private static long idCounter = 1;

        public virtual long Id { get; } = idCounter++; //Set to idCounter and increment it.
    }
}

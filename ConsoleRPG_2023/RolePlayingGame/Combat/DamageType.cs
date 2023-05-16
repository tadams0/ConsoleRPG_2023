using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleRPG_2023.RolePlayingGame.Combat
{
    /// <summary>
    /// Defines an enumeration containing various types of damage that can be dealt.
    /// </summary>
    public enum DamageType
    {
        None = 0,

        DirectHealth = 10,
        Poison = 11,
        Fire = 12,
        Corruption = 13,
        Frost = 14,
    }
}

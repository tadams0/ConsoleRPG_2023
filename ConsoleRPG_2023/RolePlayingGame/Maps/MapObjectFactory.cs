using ConsoleRPG_2023.RolePlayingGame.Effects;
using ConsoleRPG_2023.RolePlayingGame.Maps.MapObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleRPG_2023.RolePlayingGame.Maps
{
    /// <summary>
    /// Defines a class for generating map objects of various types.
    /// </summary>
    public static class MapObjectFactory
    {

        /// <summary>
        /// Creates a <see cref="HotSpring"/> instance with a permanent steam cloud effect active on it.
        /// </summary>
        /// <returns></returns>
        public static HotSpring CreateHotSpring()
        {
            HotSpring hotSpring = new HotSpring();

            Effect steamEffect = EffectCreator.CreateHotSpringStaminaCloudEffect(3, -1, 4, 1, 15, 1);

            ActiveEffect e = new ActiveEffect(steamEffect, hotSpring);
            hotSpring.ActiveEffects.Add(e);

            return hotSpring;
        }

    }
}

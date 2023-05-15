using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleRPG_2023.RolePlayingGame.Effects
{
    /// <summary>
    /// Defines a class for generating <see cref="Effect"/> instances. 
    /// <br/>Basically an effect factory.
    /// </summary>
    public static class EffectCreator
    {

        public static Effect CreatePlayerHealStaminaEffect(int range, int duration, int magnitude)
        {
            Effect e = new Effect("HEAL_STAMINA", "TARGET_PLAYER");
            e.Range = range;
            e.Duration = duration;
            e.Magnitude = magnitude;

            return e;
        }

        public static Effect CreateDebugTreeDeleteEffect(int range, int duration, int magnitude)
        {
            Effect e = new Effect("MAP_REMOVAL", "TARGET_NEARBY_TREES");
            e.Range = range;
            e.Duration = duration;
            e.Magnitude = magnitude;

            return e;
        }

    }
}

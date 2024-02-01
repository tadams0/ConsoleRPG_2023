using ConsoleRPG_2023.RolePlayingGame.Combat;
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

        public static Effect CreateAnyCharacterOnTileHealStaminaEffect(int range, int duration, int magnitude)
        {
            Effect e = new Effect("HEAL_STAMINA", "TARGET_CHARACTERS_ON_TILE", "");
            e.RangeX = range;
            e.RangeY = range;
            e.Duration = duration;
            e.Magnitude = magnitude;

            return e;
        }

        public static Effect CreatePlayerHealStaminaEffect(int range, int duration, int magnitude)
        {
            Effect e = new Effect("HEAL_STAMINA", "TARGET_PLAYER", "");
            e.RangeX = range;
            e.RangeY = range;
            e.Duration = duration;
            e.Magnitude = magnitude;

            return e;
        }

        public static Effect CreatePlayerHealHealthEffect(int range, int duration, int magnitude)
        {
            Effect e = new Effect("HEAL_HEALTH", "TARGET_PLAYER", "");
            e.RangeX = range;
            e.RangeY = range;
            e.Duration = duration;
            e.Magnitude = magnitude;

            return e;
        }

        public static Effect CreateDebugTreeDeleteEffect(int range, int duration, int magnitude)
        {
            Effect e = new Effect("MAP_REMOVAL", "TARGET_NEARBY_TREES", "");
            e.RangeX = range;
            e.RangeY = range;
            e.Duration = duration;
            e.Magnitude = magnitude;

            return e;
        }

        public static Effect CreateDamageCloudEffect(int range, int cloudSpawningDuration, int cloudLifetime, int damageDuration, double cloudSpawnChance, double damageAmountPerTurn, DamageType type)
        {
            DamageEffect damageEffect = new DamageEffect("APPLY_DAMAGE", "TARGET_CHARACTERS_ON_TILE", "");
            damageEffect.Magnitude = damageAmountPerTurn;
            damageEffect.Duration = damageDuration;
            damageEffect.AddDamageType(type);

            MultiMagnitudeEffect e = new MultiMagnitudeEffect("EFFECT_CLOUD", "", "");
            e.AlwaysRetarget = false;
            e.Stackable = true;
            e.RangeX = range;
            e.RangeY = range;
            e.Duration = cloudSpawningDuration;
            e.Magnitude = cloudSpawnChance;
            e.AddSubEffect(damageEffect);
            e.AddMagnitude(cloudLifetime); //The timer for how long the clouds stick around.

            return e;
        }

        public static Effect CreateHotSpringStaminaCloudEffect(int steamRange, int steamSpawningDuration, int steamLifetime, int staminaHealDuration, double steamSpawnChance, int staminaHealPerTurn)
        {
            Effect staminaHealEffect = CreateAnyCharacterOnTileHealStaminaEffect(1, staminaHealDuration, staminaHealPerTurn);

            MultiMagnitudeEffect e = new MultiMagnitudeEffect("EFFECT_CLOUD", "", "");
            e.AlwaysRetarget = false;
            e.Stackable = true;
            e.RangeX = steamRange;
            e.RangeY = steamRange;
            e.Duration = steamSpawningDuration;
            e.Magnitude = steamSpawnChance;
            e.AddSubEffect(staminaHealEffect);
            e.AddMagnitude(steamLifetime); //The timer for how long the clouds stick around.

            return e;
        }

        public static Effect CreateConjureHotSpringEffect()
        {
            Effect e = new Effect("CREATE_HOTSPRING", "TARGET_TRIGGER", "");

            return e;
        }

    }
}

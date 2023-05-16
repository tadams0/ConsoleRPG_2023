using ConsoleRPG_2023.RolePlayingGame.Combat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleRPG_2023.RolePlayingGame.Effects
{
    /// <summary>
    /// Defines an effect that is specialized for damage.
    /// </summary>
    public class DamageEffect : Effect
    {
        /// <summary>
        /// The types of damage associated with the effect.
        /// </summary>
        public List<DamageType> DamageTypes
        {
            get { return new List<DamageType>(damageTypes); }
        }

        private List<DamageType> damageTypes = new List<DamageType>();


        public DamageEffect(string mainEffectFunctionName, string targetingFunctionName, string cleanupFunctionName)
            : base(mainEffectFunctionName, targetingFunctionName, cleanupFunctionName)
        {
        }

        /// <summary>
        /// Adds a damage type to the effect.
        /// </summary>
        /// <param name="damageType">The type of damage.</param>
        public void AddDamageType(DamageType damageType)
        {
            damageTypes.Add(damageType);
        }

    }
}

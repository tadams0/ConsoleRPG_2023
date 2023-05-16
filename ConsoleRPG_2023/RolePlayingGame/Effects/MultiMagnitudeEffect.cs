using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleRPG_2023.RolePlayingGame.Effects
{
    /// <summary>
    /// Defines an effect which can have more than a single magnitude.
    /// </summary>
    public class MultiMagnitudeEffect : Effect
    {
        /// <summary>
        /// Gets a newly created list containing the additional magnitudes of the effect.
        /// <br/>Note that this does not include the original effect magnitude.
        /// </summary>
        public List<double> AdditionalMagnitudes
        {
            get { return new List<double>(additionalMagnitudes); }
        }

        private List<double> additionalMagnitudes = new List<double>();

        public MultiMagnitudeEffect(string mainEffectFunctionName, string targetingFunctionName, string cleanupFunctionName)
            : base(mainEffectFunctionName, targetingFunctionName, cleanupFunctionName)
        {
        }

        /// <summary>
        /// Adds an additional magnitude to the list.
        /// </summary>
        /// <param name="magnitude"></param>
        public void AddMagnitude(double magnitude)
        {
            additionalMagnitudes.Add(magnitude);
        }
    }
}

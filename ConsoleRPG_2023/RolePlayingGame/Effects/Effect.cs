using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleRPG_2023.RolePlayingGame.Effects
{
    /// <summary>
    /// Class which represents a magnitude based change.
    /// </summary>
    public class Effect
    {

        /// <summary>
        /// The object which triggered this event. Null if none.
        /// </summary>
        public GameObject Trigger { get; set; }

        /// <summary>
        /// The targets for the effect.
        /// </summary>
        public List<GameObject> targets { get; set; } = new List<GameObject>();

        /// <summary>
        /// The power of the effect.
        /// </summary>
        public double Magnitude { get; set; }

        /// <summary>
        /// The time in turns that the effect lasts for.
        /// </summary>
        public int Duration { get; set; }

        public Effect()
        {

        }

    }
}

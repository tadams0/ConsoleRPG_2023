using ConsoleRPG_2023.RolePlayingGame.Maps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleRPG_2023.RolePlayingGame.Effects
{
    /// <summary>
    /// Defines a class that holds state information for an <see cref="Effect"/> instance that is actively ongoing.
    /// </summary>
    public class ActiveEffect
    {
        /// <summary>
        /// The object which triggered this event. Null if none.
        /// </summary>
        public GameObject Trigger { get; set; }

        /// <summary>
        /// The targets for the effect.
        /// </summary>
        public List<GameObject> Targets { get; set; } = new List<GameObject>();

        /// <summary>
        /// The effect instance.
        /// </summary>
        public Effect Effect { get; set; }

        /// <summary>
        /// The current duration counter. Starts at 1.
        /// </summary>
        public int DurationCounter { get; set; } = 1;

        /// <summary>
        /// The location that the active effect was activated.
        /// </summary>
        public PointL ActivateLocation { get; set; }

        /// <summary>
        /// Gets whether the active effect has been initialized already.
        /// </summary>
        public bool Initialized { get; protected set; }

        /// <summary>
        /// Creates a new active effect instance for handling effect logic.
        /// </summary>
        /// <param name="effect">The effect to be applied.</param>
        /// <param name="triggerObject">The object triggering the effect. Can be null if not triggering object.</param>
        public ActiveEffect(Effect effect, MapObject triggerObject) 
        {
            Trigger = triggerObject;
            Effect = effect;
        }

        /// <summary>
        /// Determines if the active effect has run its course.
        /// </summary>
        /// <returns></returns>
        public bool IsDone()
        {
            //Negative duration effects are never done.
            return DurationCounter > Effect.Duration && Effect.Duration >= 0;
        }

        /// <summary>
        /// Initializes the <see cref="ActiveEffect"/> for further application.
        /// </summary>
        public void InitializeEffect(GameState state, Map map, PointL location)
        {
            ActivateLocation = location;
            var initAction = EffectFunctionManager.GetAction(Effect.InitializeFunctionId);
            initAction(this, state, map);
        }

        /// <summary>
        /// Applies the effect and increments the duration counter.
        /// </summary>
        public void ApplyEffect(GameState state, Map map)
        {
            var effectAction = EffectFunctionManager.GetAction(Effect.MainEffectFunctionId);
            effectAction(this, state, map);
            DurationCounter++;
        }

    }
}

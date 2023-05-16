
using System.Collections.Generic;

namespace ConsoleRPG_2023.RolePlayingGame.Effects
{
    /// <summary>
    /// Class which represents a magnitude based change.
    /// </summary>
    public class Effect
    {
        /// <summary>
        /// The power of the effect.
        /// </summary>
        public double Magnitude { get; set; }

        /// <summary>
        /// The time in turns that the effect lasts for.
        /// <br/>Use 0 for one-time use or instant.
        /// <br/>Use negative numbers for infinite.
        /// </summary>
        public int Duration { get; set; } = 0;

        /// <summary>
        /// The range in tiles on the x-axis of the effect. Negative values and 0 represent no range.
        /// </summary>
        public int RangeX { get; set; } = 0;

        /// <summary>
        /// The range in tiles on the y-axis of the effect. Negative values and 0 represent no range.
        /// </summary>
        public int RangeY { get; set; } = 0;

        /// <summary>
        /// If true, then the initialization function of the effect will always trigger each time the effect is applied.
        /// </summary>
        public bool AlwaysRetarget { get; set; } = true;

        /// <summary>
        /// Gets or sets whether the effect is stackable. If true, then a map object can have more than one active.
        /// </summary>
        public bool Stackable { get; set; } = false;

        /// <summary>
        /// Text shown to users when examined either on themselves or on others.
        /// </summary>
        public string ExamineText { get; set; } = string.Empty;

        /// <summary>
        /// The list of sub effects tied to this effect.
        /// </summary>
        public List<Effect> SubEffects
        {
            get { return new List<Effect>(subEffects); }
        }

        /// <summary>
        /// The initialize function is used to generate the targets of the effect. It may target the player character for example.
        /// </summary>
        public int TargetingFunctionId
        {
            get { return targetingFunctionId; }
        }

        /// <summary>
        /// The main effect function is the actual logic the effect executes. It may be healing of character health for example.
        /// </summary>
        public int MainEffectFunctionId
        {
            get { return mainEffectFunctionId; }
        }

        public int CleanupFunctionId
        {
            get { return cleanupFunctionId; }
        }

        /// <summary>
        /// The targeting function is used to generate the targets of the effect. It may target the player character for example.
        /// </summary>
        protected int targetingFunctionId = -1;

        /// <summary>
        /// The main effect function is the actual logic the effect executes. It may be healing of character health for example.
        /// </summary>
        protected int mainEffectFunctionId = -1;

        /// <summary>
        /// The function run during the effect removal and cleanup. Negative numbers represent no function.
        /// </summary>
        protected int cleanupFunctionId = -1;

        /// <summary>
        /// Sub effects tied to the main effect function. Leave empty if no sub effects exist.
        /// </summary>
        protected List<Effect> subEffects = new List<Effect>();

        public Effect(int mainEffectFunctionId, int targetingFunctionId, int cleanupFunctionId)
        {
            this.mainEffectFunctionId = mainEffectFunctionId;
            this.targetingFunctionId = targetingFunctionId;
            this.cleanupFunctionId = cleanupFunctionId;
        }

        public Effect(string mainEffectFunctionName, string targetingFunctionName, string cleanupFunctionName)
        {
            this.mainEffectFunctionId = EffectFunctionManager.GetActionId(mainEffectFunctionName);

            if (string.IsNullOrWhiteSpace(targetingFunctionName))
            {
                targetingFunctionId = -1;
            }
            else
            {
                this.targetingFunctionId = EffectFunctionManager.GetActionId(targetingFunctionName);
            }

            if (string.IsNullOrWhiteSpace(cleanupFunctionName))
            {
                cleanupFunctionId = -1;
            }
            else
            {
                this.cleanupFunctionId = EffectFunctionManager.GetActionId(cleanupFunctionName);
            }
        }

        /// <summary>
        /// Adds a sub effect to this effect instance.
        /// </summary>
        /// <param name="effect">The sub effect to add.</param>
        public void AddSubEffect(Effect effect)
        {
            subEffects.Add(effect);
        }

    }
}

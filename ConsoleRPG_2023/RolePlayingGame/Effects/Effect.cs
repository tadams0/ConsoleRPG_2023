using ConsoleRPG_2023.RolePlayingGame.Maps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
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
        /// The range in tiles of the effect. Negative values and 0 represent no range.
        /// </summary>
        public int Range { get; set; } = 0;

        /// <summary>
        /// If true, then the initialization function of the effect will always trigger each time the effect is applied.
        /// </summary>
        public bool AlwaysRetarget { get; set; } = true;

        /// <summary>
        /// The initialize function is used to generate the targets of the effect. It may target the player character for example.
        /// </summary>
        public int InitializeFunctionId
        {
            get { return initializeFunctionId; }
        }

        /// <summary>
        /// The main effect function is the actual logic the effect executes. It may be healing of character health for example.
        /// </summary>
        public int MainEffectFunctionId
        {
            get { return mainEffectFunctionId; }
        }

        /// <summary>
        /// The initialize function is used to generate the targets of the effect. It may target the player character for example.
        /// </summary>
        protected int initializeFunctionId = -1;

        /// <summary>
        /// The main effect function is the actual logic the effect executes. It may be healing of character health for example.
        /// </summary>
        protected int mainEffectFunctionId = -1;

        public Effect(int mainEffectFunctionId, int initializeFunctionId)
        {
            this.mainEffectFunctionId = mainEffectFunctionId;
            this.initializeFunctionId = initializeFunctionId;
        }

        public Effect(string mainEffectFunctionName, string initializeFunctionName)
        {
            this.mainEffectFunctionId = EffectFunctionManager.GetActionId(mainEffectFunctionName);
            this.initializeFunctionId = EffectFunctionManager.GetActionId(initializeFunctionName);
        }

        public void SetInitializeFunction(string functionName)
        {
            this.initializeFunctionId = EffectFunctionManager.GetActionId(functionName);
        }

    }
}

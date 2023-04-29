using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleRPG_2023.RolePlayingGame
{
    /// <summary>
    /// Defines a simple class that holds the values of a menu result.
    /// </summary>
    public class MenuResult
    {

        public string Action { get; set; } = string.Empty;

        public string CustomMessage { get; set; } = string.Empty;

        /// <summary>
        /// The payload to pass into the next menu. This can be left as null if no payload was given.
        /// </summary>
        public object Payload { get; set; } = null;

        public MenuResult()
        {

        }

        public MenuResult(string action)
        {
            Action = action;
        }

    }
}

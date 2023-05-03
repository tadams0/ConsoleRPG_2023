using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleRPG_2023.RolePlayingGame.Menus
{
    public class Menu
    {
        /// <summary>
        /// Determines whether to run the custom render before or after the menu message.
        /// </summary>
        public bool CustomRenderBeforeMessage
        {
            get { return customRenderBeforeMessage; }
        }

        /// <summary>
        /// Determines if the custom message appears below or above menu options.
        /// </summary>
        public bool CustomMessageAppearsBelow
        {
            get { return customMessageAppearsBelow; }
            set { customMessageAppearsBelow = value; }
        }

        /// <summary>
        /// If true then the main game loop will skip calling this menu's <see cref="GetMenuInput"/> method.
        /// <br/>If false, the method will be called normally.
        /// </summary>
        public bool SkipMenuInput
        { 
            get { return skipMenuInput; } 
        }

        public string LineSeperator
        {
            get { return lineSeparator; }
            set { lineSeparator = value; }
        }

        public ConsoleColor LineSeperatorColor
        {
            get { return lineSeperatorColor; }
        }

        /// <summary>
        /// Gets the string that represents a new line.
        /// </summary>
        public string NewLineString
        {
            get { return newLineString; }
        }

        public GameState GameState
        {
            get { return state; }
            private set { state = value; }
        }

        public GameSettings Settings
        {
            get { return settings; }
        }


        private string newLineString = "\n";
        private string lineSeparator = "_________________________";

        private ConsoleColor lineSeperatorColor = ConsoleColor.White;

        private GameState state;

        private GameSettings settings;

        /// <summary>
        /// If true, then even if the menu message is empty, an empty line will still be written to the screen.
        /// </summary>
        protected bool writeEmptyMessage = false;

        /// <summary>
        /// If true, then the options display will be written to screen. If false, it will not.
        /// </summary>
        protected bool writeOptions = true;

        /// <summary>
        /// If true then the main game loop will skip calling this menu's <see cref="GetMenuInput"/> method.
        /// <br/>If false, the method will be called normally.
        /// </summary>
        protected bool skipMenuInput = false;

        /// <summary>
        /// The default action to use if no update override is applied or no option corresponds.
        /// </summary>
        protected string defaultAction = string.Empty;

        /// <summary>
        /// Determines if the custom message appears below or above menu options.
        /// </summary>
        protected bool customMessageAppearsBelow = true;

        /// <summary>
        /// Determines whether to run the custom render before or after the menu message.
        /// </summary>
        protected bool customRenderBeforeMessage = true;

        /// <summary>
        /// The last payload passed into the menu from a previous menu.
        /// </summary>
        protected object lastPayload = null;

        /// <summary>
        /// Adds the gamestate to this menu.
        /// </summary>
        /// <param name="state">The current active game's gamestate.</param>
        public void SetGameState(GameState state, GameSettings settings)
        {
            this.state = state;
            this.settings = settings;
            OnSetGameState();
        }

        /// <summary>
        /// Sets data that was given from a previous menu.
        /// </summary>
        /// <param name="obj">The data given.</param>
        public void SetPayload(object obj)
        {
            lastPayload = obj;
            OnSetPayload();
        }

        /// <summary>
        /// A method that child classes can override as needed once they recieve a payload from a menu.
        /// </summary>
        protected virtual void OnSetPayload()
        {
        }

        /// <summary>
        /// A method that child classes can override as needed once they recieve the game state.
        /// </summary>
        protected virtual void OnSetGameState()
        {
        }

        /// <summary>
        /// Retrieves the input that the menu desires.
        /// </summary>
        /// <returns>Returns a new <see cref="InputResult"/> object containing the resulting input.</returns>
        public virtual InputResult GetMenuInput()
        {
            return Helper.GetInputWholeNumberOnly();
        }

        /// <summary>
        /// Creates the message display for this menu.
        /// </summary>
        /// <returns></returns>
        protected virtual string CreateMessage()
        {
            return "ERROR: No message found.";
        }

        /// <summary>
        /// A method child classes can use to manually render items to the screen.
        /// </summary>
        protected virtual void CustomRender()
        {

        }

        /// <summary>
        /// Creates the options display for this menu.
        /// </summary>
        /// <returns></returns>
        protected virtual OptionDisplay CreateOptions()
        {
            return new OptionDisplay();
        }

        protected void WriteMessage()
        {
            string message = CreateMessage();

            if (writeEmptyMessage || !string.IsNullOrEmpty(message))
            {//Only write the message if we don't care if it's an empty message OR if it's populated.
                Helper.WriteFormattedString(message);
            }
        }

        /// <summary>
        /// Writes a newly generated options display to the screen.
        /// </summary>
        /// <returns>Returns the options display that was used.</returns>
        protected OptionDisplay WriteOptions()
        {
            if (writeOptions)
            {
                OptionDisplay options = CreateOptions();

                Console.WriteLine();
                Helper.WriteFormattedString(Helper.FormatString(lineSeparator, lineSeperatorColor));
                options.WriteToConsole();
                Helper.WriteFormattedString(Helper.FormatString(lineSeparator, lineSeperatorColor));
                Console.WriteLine();

                return options;
            }

            return null;
        }

        /// <summary>
        /// Writes the menu message and its options to the console.
        /// </summary>
        /// <returns>Returns the options display that was used.</returns>
        public OptionDisplay WriteToConsole()
        {
            if (CustomRenderBeforeMessage)
            {
                CustomRender();
            }

            WriteMessage();

            if (!CustomRenderBeforeMessage)
            {
                CustomRender();
            }

            return WriteOptions();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <param name="message"></param>
        /// <returns>Returns the name of the menu or otherwise the action.</returns>
        public virtual MenuResult Update(InputResult input, OptionDisplay displayUsed)
        {
            if (input.IsNumeric && displayUsed != null)
            {
                //Convert the decimal to an integer.
                //To do this safely, we need to make sure the deci
                int inputNumeric = input.NumericAsInt;

                //Try to get the callback from the display used.
                var callback = displayUsed.GetCallback(inputNumeric);

                //Check if it exists.
                if (callback != null)
                {
                    return callback(input);
                }
            }

            //If no callback was found or it was not a numeric input, then we return the default empty result.
            return new MenuResult(defaultAction);
        }

    }
}

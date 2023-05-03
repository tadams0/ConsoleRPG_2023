using ConsoleRPG_2023.RolePlayingGame.Maps;
using ConsoleRPG_2023.RolePlayingGame.Menus;
using ConsoleRPG_2023.RolePlayingGame.Renderers;
using ConsoleRPG_2023.RolePlayingGame.Text;
using CustomConsoleColors;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Numerics;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace ConsoleRPG_2023.RolePlayingGame
{
    /// <summary>
    /// Defines an RPG Game entry point.
    /// </summary>
    public static class RPGGame
    {
        /// <summary>
        /// Gets the active game settings.
        /// </summary>
        public static GameSettings Settings
        {
            get { return settings; }
        }

        //The primise of this 'game' is to go between menus using a whole number only.
        //Each 'turn' the player must select a new number for a new menu.
        //Menus can capture players by utilizing loops and input methods. e.g. combat.

        private static Menu currentMenu;

        /// <summary>
        /// The state of the game.
        /// </summary>
        private static GameState state;

        private static List<InputResult> inputStack = new List<InputResult>();

        private static List<string> menuActionStack = new List<string>();

        private static Dictionary<string, Menu> MenuTypeNameToMenu = new Dictionary<string, Menu>();

        private static bool clearScreenEveryMenu = true;

        private static GameSettings settings = new GameSettings();

        public static void Start()
        {
            state = new GameState();
            InitializeGameState(state);

            /*
            Console.Write("This");
            string s = Helper.FormatString("This", ConsoleColor.Red, ConsoleColor.White, 1000,50,1000);
            Helper.WriteFormattedString(s, false);
            Console.Write("This");
            Console.Write("This");
            return;
            */
            /*
            for (int i = 0; i < 200; i++)
            {
                Console.WriteLine(RandomGenerator.GenerateRandomFirstName(Race.High_Elf, false));
            }
            return;
            */

            /*
            TextWriter writer = new TextWriter();
            writer.WriteFormattedText($"This is what you call {Helper.FormatString("Shannegians", ConsoleColor.White, ConsoleColor.DarkGray, 1000, 75, 1000)}. Pretty cool.");

            //writer.WriteFormattedText("1) Select a name.\n2) Select a race. | Selected: {msafter:3000|msdelay:2000|mschar:100| color: DarkYellow}High Elf{none}\n3) Complete");
            return;
            */

            PopulateMenuMapping();

            bool running = true;

            menuActionStack.Add("MainMenu");

            currentMenu = GetMenuByName("MainMenu");

            InputResult lastInput = null;
            MenuResult lastMenuResult = new MenuResult();

            //The menu that the last result came from.
            Menu lastMenuResultMenu = currentMenu;

            while (running)
            {
                //Clearing the console so previous menus are not shown.
                if (clearScreenEveryMenu)
                {
                    Console.BackgroundColor = Helper.defaultBackgroundColor;
                    Console.ForegroundColor = Helper.defaultColor;

                    //Due to a bug in Windows 11 terminal, the below three lines clears text off-screen as well.
                    //See this post for more info: https://stackoverflow.com/questions/75471607/console-clear-doesnt-clean-up-the-whole-console
                    Console.Clear();
                    Console.WriteLine("\x1b[3J");
                    Console.Clear();
                }

                //Writing any custom message the menu has to say. If the menu that it came from has it configured to appear here.
                if (lastMenuResult.CustomMessage != "" && !lastMenuResultMenu.CustomMessageAppearsBelow)
                {
                    Helper.WriteFormattedString(lastMenuResult.CustomMessage);
                }

                //Write the menu message and options to screen.
                OptionDisplay displayUsed = currentMenu.WriteToConsole();

                //Writing any custom message the menu has to say. If the menu that it came from has it configured to appear here.
                if (lastMenuResult.CustomMessage != "" && lastMenuResultMenu.CustomMessageAppearsBelow)
                {
                    Helper.WriteFormattedString(lastMenuResult.CustomMessage);
                }

                //Only skip the menu input if the menu calls for it.
                if (!currentMenu.SkipMenuInput)
                {
                    //Get the menu input
                    lastInput = currentMenu.GetMenuInput();
                }

                //Now update the menu based on the input.
                MenuResult result = currentMenu.Update(lastInput, displayUsed);

                //Saving the menu reference that the result came from.
                lastMenuResultMenu = currentMenu;

                //Handling the resulting actions from the menu.

                //If the none action was specified, then we will not do anything.
                if (!Helper.StringEquals(result.Action, Helper.ActionNone))
                {
                    if (Helper.StringEquals(result.Action, "Exit"))
                    {//Special case for exiting the application.
                        running = false;
                    }
                    else if (Helper.StringEquals(result.Action, "return") || Helper.StringEquals(result.Action, "back"))
                    {//Return or back actions result in going to the previous action.
                        if (menuActionStack.Count <= 1) //If there is only 1 menu, that's the main menu, we can't navigate past that. Also how are you returning on the main menu?
                        {
                            string currentMenuName = currentMenu.GetType().Name;
                            Helper.WriteFormattedString($"Oh buddy, there was no previous menu to return to from {Helper.FormatString(currentMenuName, ConsoleColor.Red)}.");
                            Helper.WriteFormattedString($"Type any key to continue.");
                            Console.ReadKey(); //We want to pause the user so they can read the error.
                        }
                        else
                        {//If there is a previous menu, then let's go to that.
                            string lastMenuToVisit = menuActionStack[menuActionStack.Count - 2]; //Second to last should skip the current menu, which should be the last.

                            //Navigate to the last menu.
                            bool success = TryGoToMenu(lastMenuToVisit, lastInput);
                            
                            //Pop the stack since we went back.
                            menuActionStack.RemoveAt(menuActionStack.Count - 1);

                            if (success)
                            {//If the menu was successfully swapped or moved, then we set the payload from the result to it.
                                currentMenu.SetPayload(result.Payload);
                            }
                        }
                    }
                    else if (string.IsNullOrWhiteSpace(result.Action))
                    {//Case for when there was no action returned.
                        string currentMenuName = currentMenu.GetType().Name;
                        Helper.WriteFormattedString($"Ut oh, there's no action in the menu result for type {Helper.FormatString(currentMenuName, ConsoleColor.Red)}.");
                        Helper.WriteFormattedString($"Did you forget to return the {Helper.FormatString("None", ConsoleColor.Red)} type from {Helper.FormatString("Helper.ActionNone", ConsoleColor.Red)}?");
                        Helper.WriteFormattedString($"Type any key to continue.");
                        Console.ReadKey(); //We want to pause the user so they can read the error.
                    }
                    else
                    {//Checking if the action was a valid menu, and if so swapping to it.
                        bool success = TryGoToMenu(result.Action, lastInput);
                        menuActionStack.Add(result.Action);

                        if (success)
                        {//If the menu was successfully swapped or moved, then we set the payload from the result to it.
                            currentMenu.SetPayload(result.Payload);
                        }
                    }


                }

                lastMenuResult = result;
                inputStack.Add(lastInput);
            }

        }

        private static void InitializeGameState(GameState state)
        {
            state.PlayerCharacter = new Character(true);

            state.PlayerHud = new HUD();
            state.PlayerHud.SetCharacter(state.PlayerCharacter);

            ProceduralWorldMap map = new ProceduralWorldMap();

            state.WorldMap = map;
            state.MapRenderer = new MapRendererExtraColorRange(map, state, settings, true);

        }

        private static bool TryGoToMenu(string menuName, InputResult lastInput)
        {//Technically lastInput isn't really required, but I want to log it so here I am taking it in.

            Menu newMenu = GetMenuByName(menuName);
            if (newMenu != null)
            {
                currentMenu = newMenu;
                return true;
            }
            else
            {
                string currentMenuName = currentMenu.GetType().Name;
                Helper.WriteFormattedString($"Oh no, the action {Helper.FormatString(menuName, ConsoleColor.Red)} does not exist. Action came from type {Helper.FormatString(currentMenuName, ConsoleColor.Red)} with value {Helper.FormatString(lastInput.Numeric.ToString(), ConsoleColor.Red)}.");
                Helper.WriteFormattedString($"Did you forget to return the {Helper.FormatString("None", ConsoleColor.Red)} type from {Helper.FormatString("Helper.ActionNone", ConsoleColor.Red)}?");
                Helper.WriteFormattedString($"Type any key to continue.");
                Console.ReadKey(); //We want to pause the user so they can read the error.
            }
            return false;
        }

        private static Menu GetMenuByName(string name)
        {
            name= name.ToUpperInvariant();
            if (MenuTypeNameToMenu.ContainsKey(name))
            {
                return MenuTypeNameToMenu[name];
            }
            return null;
        }

        private static void PopulateMenuMapping()
        {
            var allAssemblyTypes = Assembly.GetExecutingAssembly().GetTypes();
            //checks the executing assembly (think .exe kinda) for all of its types. Then looks through (where) all those types and adds only the ones that are a class and have the parent of Menu.
            Type[] menuTypes = allAssemblyTypes.Where(x => x.IsClass && x.IsSubclassOf(typeof(Menu))).ToArray();

            foreach (Type type in menuTypes)
            {
                string capitalizedTypeName = type.Name.ToUpperInvariant();

                //This generates the new menu instance from the type above.
                Menu newMenu = (Menu)Activator.CreateInstance(type);

                //Adds the game state to the menu so it can access it as needed.
                newMenu.SetGameState(state, settings);

                //Complete the mapping.
                MenuTypeNameToMenu[capitalizedTypeName] = newMenu;

            }

        }


    }
}

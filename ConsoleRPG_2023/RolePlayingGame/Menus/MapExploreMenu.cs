using ConsoleRPG_2023.RolePlayingGame.Dungeons;
using ConsoleRPG_2023.RolePlayingGame.Maps;
using ConsoleRPG_2023.RolePlayingGame.Maps.MapObjects;
using ConsoleRPG_2023.RolePlayingGame.Renderers;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Timers;

namespace ConsoleRPG_2023.RolePlayingGame.Menus
{
    public class MapExploreMenu : Menu
    {

        private HUD playerHud;

        private Map map;
        private MapRenderer mapRenderer;

        private Character player;

        private Timer detailedAreaViewTimer;
        private double msBeforeDetailedView = 300;

        private bool detailedMode = false;

        private bool debugMode = false;
        private bool showPlayerCoordinates = false;

        /// <summary>
        /// Number of objects the player is currently ontop.
        /// </summary>
        private int objectCount = 0;

        public MapExploreMenu() 
        {
            customRenderBeforeMessage = true;

            int lineSeperatorLength = LineSeperator.Length;
            LineSeperator = new string('~', lineSeperatorLength);

            detailedAreaViewTimer = new Timer(msBeforeDetailedView);
            detailedAreaViewTimer.AutoReset = false;
            detailedAreaViewTimer.Elapsed += DetailedAreaViewTimer_Elapsed;
        }

        private void DetailedAreaViewTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (detailedMode)
                return;
            
            ConsoleExtensions.CancelInput();
        }

        protected override void OnSetGameState()
        {
            //Saving state variable references to this local instance for ease of use.
            playerHud = GameState.PlayerHud;
            map = GameState.WorldMap;
            mapRenderer = GameState.MapRenderer;
            player = GameState.PlayerCharacter;

            //TEST:
            //TODO: Look into the map cap, it should be much higher (Probably due to an inproper key in a dictionary or float precision or broken point normalization)
            //There seems to be a hard limit at 34359738368
            //player.X = map.MaximumNumberOfChunks / 2;
            //player.X = 491;
            //player.Y = -190;

            //Add the player to the map (Afterall we want to track the player).
            MapChunk chunk = map.GetChunkAtWorldSpace(player.X, player.Y);
            chunk.AddMapObject(player);

            //TEST ONLY:
            DungeonMap testDungeon = new DungeonMap(11, 11);
            long id = map.AddDungeon(testDungeon, 0, 0);

            MapDungeonObj dungeonMarker = new MapDungeonObj(id);
            dungeonMarker.Name = testDungeon.Name;

            chunk.AddMapObject(0, 0, dungeonMarker);
            //END TEST
        }

        protected override void CustomRender()
        {
            if (detailedMode)
            {
                detailedAreaViewTimer.Stop();
            }

            //Render the map here.
            mapRenderer.Map = GameState.WorldMap; //Set the renderer to use the world map (just in case it's not for some reason).
            mapRenderer.DrawMapCenterOnCoordinates(player.X, player.Y);
            var result = mapRenderer.GetConsoleSpaceFromWorldSpace(player.X, player.Y);
            if (debugMode)
            {
                Console.WriteLine($"Player console x: " + result.X + " y: " + result.Y);
            }

            if (showPlayerCoordinates)
            {
                Console.WriteLine($"Player x: " + player.X + " y: " + player.Y);
            }

            /*
            PointL worldSpaceFromConsole = mapRenderer.GetWorldSpaceFromConsoleSpace(Console.WindowWidth / 2, 0);
            Tile t = map.GetTileAtWorldSpace(worldSpaceFromConsole.X, worldSpaceFromConsole.Y);
            
            */

            if (debugMode)
            {
                Tile t = map.GetTileAtWorldSpace(player.X, player.Y);

                string tileDisplay = "null";
                if (t != null)
                {
                    tileDisplay = t.TileType.ToString();
                }

                Console.WriteLine($"Tile under player is: " + tileDisplay);

                if (map is ProceduralWorldMap)
                {
                    ProceduralWorldMap procMap = (ProceduralWorldMap)map;

                    BiomeData data = procMap.GetBiomeDataForTile(player.X, player.Y);
                    BiomeRegionData regionData = procMap.GetBiomeRegionData(player.X, player.Y);
                    BiomeData generalBiomeData = procMap.GetGeneralBiomeData(player.X, player.Y);
                    Console.WriteLine($"Biome Type {generalBiomeData.GetBiomeType()} Moisture: {Math.Round(data.Moisture, 2)} Temperature: {Math.Round(data.Temperature, 2)} Fertility: {Math.Round(data.Fertility, 2)} Height: {Math.Round(data.Height, 2)}");
                    Console.WriteLine($"General Biome Stats Moisture: {Math.Round(generalBiomeData.Moisture, 2)} Temperature: {Math.Round(generalBiomeData.Temperature, 2)} Fertility: {Math.Round(generalBiomeData.Fertility, 2)} Height: {Math.Round(generalBiomeData.Height, 2)}");
                }
            }

            if (detailedMode)
            {
                Console.WriteLine($"You look around and see: ");

                MapChunk chunk = map.GetChunkAtWorldSpace(player.X, player.Y);
                List<MapObject> objects = chunk.GetAllObjectsAtWorldCoordinates(player.X, player.Y);
                
                //Remove the player from the list.
                objects.Remove(player);

                MapObject currentObj;
                for (int i = 0; i < objects.Count; i++)
                {
                    currentObj = objects[i];
                    Console.WriteLine($"{i + 1}) {currentObj.ToString()}");
                }

                objectCount = objects.Count;
            }

        }

        protected override string CreateMessage()
        {
            string hudDisplay = string.Empty;
            
            if (!detailedMode)
            {//Hide the HUD when in detailed mode.
                hudDisplay = playerHud.GetHudDisplay();
            }

            return hudDisplay;
        }

        protected override OptionDisplay CreateOptions()
        {
            OptionDisplay options = base.CreateOptions();



            return options;
        }
        public override InputResult GetMenuInput()
        {
            //We surround the input with a try catch here because we can cancel the input which throws an exception to end the input stream.
            try
            {
                //We specifically override here so we can only get a single key press
                if (detailedMode && objectCount > 9)
                {//If there are more than 9 objects, then we will not use a single key press for input.
                    return Helper.GetInput(false);
                }
                else
                {//Otherwise simply do a single key press for easy selection and movement.
                    return Helper.GetInput(true);
                }
            }
            catch (Exception e)
            {

            }

            //If the try catch fails, it should be due to the input being cancelled. In this case, we return the cancelled default input.
            return InputResult.GetCancelledInput();
        }


        public override MenuResult Update(InputResult input, OptionDisplay displayUsed)
        {
            MenuResult result = base.Update(input, displayUsed);

            result.Action = Helper.ActionNone;

            result.CustomMessage = $"INPUT: Cancelled? {input.Canceled} Value: {input.Text}";

            if (input.Canceled)
            {//If the input was cancelled, then we can swap to detailed mode.
                detailedMode = true;

                //End the method early.
                return result;
            }

            //The player can always freely move around regardless of the mode state.
            if (Helper.StringEquals(input.Text, "w")
                    || input.Key.Key == ConsoleKey.UpArrow)
            {//Move north
                map.MoveObject(player, 0, -1);
                OnPlayerAction();
                OnPlayerMove();
            }
            else if (Helper.StringEquals(input.Text, "s")
                || input.Key.Key == ConsoleKey.DownArrow)
            {//Move south
                map.MoveObject(player, 0, 1);
                OnPlayerAction();
                OnPlayerMove();
            }
            else if (Helper.StringEquals(input.Text, "a")
                || input.Key.Key == ConsoleKey.LeftArrow)
            {//Move west
                map.MoveObject(player, -1, 0);
                OnPlayerAction();
                OnPlayerMove();
            }
            else if (Helper.StringEquals(input.Text, "d")
                || input.Key.Key == ConsoleKey.RightArrow)
            {//Move east
                map.MoveObject(player, 1, 0);
                OnPlayerAction();
                OnPlayerMove();
            }

            else if (detailedMode && objectCount > 0)
            {//If the player is over more than 0 objects...

                if (input.IsNumeric)
                {//Numeric input here is considered the player trying to interact with a specified object.
                    int objectIndex = input.NumericAsInt - 1;

                    //Out of bounds check
                    if (objectIndex < 0 || objectIndex > objectCount)
                        return result;

                    var interactionResult = InteractWithObjectAtIndex(objectIndex, result);
                    if (interactionResult != null)
                    {//Might receive a null interaction result if the player is trying to iteract with themself for example.
                        result.CustomMessage = interactionResult.InteractionMessage;
                    }
                }
                else if (Helper.StringEquals(input.Text, " ")
                    || input.Key.Key == ConsoleKey.Spacebar)
                {//Spacebar or empty space as text is considered a shortcut to interacting with the first object.
                    var interactionResult = InteractWithObjectAtIndex(0, result);
                    result.CustomMessage = interactionResult.InteractionMessage;
                }

            }

            return result;
        }

        private MapObjectInteractionResult InteractWithObjectAtIndex(int index, MenuResult result)
        {
            MapChunk chunk = map.GetChunkAtWorldSpace(player.X, player.Y);
            List<MapObject> objects = chunk.GetAllObjectsAtWorldCoordinates(player.X, player.Y);
            MapObject interactingObject = objects[index];
            if (interactingObject != null && interactingObject != player)
            {
                Maps.MapObjects.MapObjectInteractionResult interactionResult = interactingObject.Interact(map);
                result.Action = interactionResult.Action;
                result.Payload = interactionResult;

                return interactionResult;
            }

            return null;
        }

        /// <summary>
        /// Method that runs whenever a player does an action on the map.
        /// </summary>
        private void OnPlayerAction()
        {
            player.Stamina -= 1;
        }

        /// <summary>
        /// Method that runs whenever a player moves location.
        /// </summary>
        private void OnPlayerMove()
        {
            detailedMode = false;

            //Restart the detailed view timer:
            detailedAreaViewTimer.Stop();
            detailedAreaViewTimer.Start();
        }

    }
}

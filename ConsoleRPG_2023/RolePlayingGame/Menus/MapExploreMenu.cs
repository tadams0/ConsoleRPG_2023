using ConsoleRPG_2023.RolePlayingGame.Dungeons;
using ConsoleRPG_2023.RolePlayingGame.Effects;
using ConsoleRPG_2023.RolePlayingGame.Items;
using ConsoleRPG_2023.RolePlayingGame.Maps;
using ConsoleRPG_2023.RolePlayingGame.Maps.MapObjects;
using ConsoleRPG_2023.RolePlayingGame.MenuPayloads;
using ConsoleRPG_2023.RolePlayingGame.Renderers;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Timers;

namespace ConsoleRPG_2023.RolePlayingGame.Menus
{
    public class MapExploreMenu : Menu
    {
        /// <summary>
        /// A stack of maps in order of visitation.
        /// </summary>
        private List<Pair<Map, PointL>> mapStack = new List<Pair<Map, PointL>>();

        private HUD playerHud;

        private Map map;
        private MapRenderer mapRenderer;

        private Character player;

        private Timer detailedAreaViewTimer;
        private double msBeforeDetailedView = 300;

        private bool detailedMode = false;

        private bool debugMode = true;
        private bool showPlayerCoordinates = true;

        /// <summary>
        /// Number of objects the player is currently ontop.
        /// </summary>
        private int objectCount = 0;

        public MapExploreMenu() 
        {
            customRenderBeforeMessage = true;

            int lineSeperatorLength = LineSeperator.Length;
            LineSeperator = new string('~', lineSeperatorLength);

            this.writeOptions = false;
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

            detailedAreaViewTimer = new Timer(msBeforeDetailedView);
            detailedAreaViewTimer.AutoReset = false;
            detailedAreaViewTimer.Elapsed += DetailedAreaViewTimer_Elapsed;

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
            PointL startPosition = new PointL(0, 0);
            DungeonMap testDungeon = new DungeonMap(20, 20, startPosition.X, startPosition.Y);
            testDungeon.CreateEntranceExitReturns(map.Name);
            long id = map.AddSubMap(testDungeon, 0, 0);

            MapEntranceObj dungeonMarker = new MapEntranceObj(id, startPosition.X, startPosition.Y);
            dungeonMarker.Name = testDungeon.Name;

            chunk.AddMapObject(0, 0, dungeonMarker);


            Item coifOfTesting = new Item();
            coifOfTesting.Name = "Coif of Testing";
            coifOfTesting.ItemType = ItemUseType.Equippable;
            coifOfTesting.Description = "A coif used during testing to see how object and item interaction works within a map.";
            coifOfTesting.Category = ItemCategoryType.HeadArmor;
            coifOfTesting.Noun = "coif";
            MapItem itemMarker1 = new MapItem(coifOfTesting);
            itemMarker1.LocationDescription = "within the mud on the ground";
            chunk.AddMapObject(itemMarker1);

            List<Consumable> testConsumableList = new List<Consumable>();

            Effect healthHealEffect = EffectCreator.CreatePlayerHealHealthEffect(0, 0, 50);

            Consumable healthPotion = new Consumable();
            healthPotion.Name = "Standard Healing Potion";
            healthPotion.ItemType = ItemUseType.Consumable;
            healthPotion.ActionVerb = "drink";
            healthPotion.Description = "A simple healing potion. Heals 50 health.";
            healthPotion.Category = ItemCategoryType.Potion;
            healthPotion.Noun = "potion";
            healthPotion.Effect = healthHealEffect;
            MapItem itemMarker2 = new MapItem(healthPotion);
            itemMarker2.LocationDescription = "cleanly sitting on the ground";
            chunk.AddMapObject(itemMarker2);
            testConsumableList.Add(healthPotion);

            Effect staminaHealEffect = EffectCreator.CreatePlayerHealStaminaEffect(0, 0, 100);

            Consumable staminaPotion = new Consumable();
            staminaPotion.Name = "Stamina Surge Potion";
            staminaPotion.ItemType = ItemUseType.Consumable;
            staminaPotion.ActionVerb = "drink";
            staminaPotion.Description = "A simple potion used to alleviate fatigue. Recovers 100 stamina.";
            staminaPotion.Category = ItemCategoryType.Potion;
            staminaPotion.Noun = "potion";
            staminaPotion.Effect = staminaHealEffect;
            testConsumableList.Add(staminaPotion);

            Effect removeTrees = EffectCreator.CreateDebugTreeDeleteEffect(10, 10, 100);

            Consumable scrollOfTreeRemoval = new Consumable();
            scrollOfTreeRemoval.Name = "Scroll of Tree Deletion";
            scrollOfTreeRemoval.ItemType = ItemUseType.Consumable;
            scrollOfTreeRemoval.ActionVerb = "cast";
            scrollOfTreeRemoval.Description = "A powerful scroll passed down by the gods to cease the existing of nearby trees for 10 turns.";
            scrollOfTreeRemoval.Category = ItemCategoryType.Scroll;
            scrollOfTreeRemoval.Noun = "scroll";
            scrollOfTreeRemoval.Effect = removeTrees;
            testConsumableList.Add(scrollOfTreeRemoval);

            Effect poisonCloudEffect = EffectCreator.CreateDamageCloudEffect(3, 50,4, 1, 20, 10, Combat.DamageType.Poison);

            Consumable scrollOfPoison = new Consumable();
            scrollOfPoison.Name = "Plague Scroll";
            scrollOfPoison.ItemType = ItemUseType.Consumable;
            scrollOfPoison.ActionVerb = "cast";
            scrollOfPoison.Description = "A scroll which summons numerous poison clouds to strike damage to nearby characters.";
            scrollOfPoison.Category = ItemCategoryType.Scroll;
            scrollOfPoison.Noun = "scroll";
            scrollOfPoison.Effect = poisonCloudEffect;
            testConsumableList.Add(scrollOfPoison);

            Effect fireCloudEffect = EffectCreator.CreateDamageCloudEffect(3, 50, 4, 1, 20, 10, Combat.DamageType.Fire);

            Consumable scrollOfFire = new Consumable();
            scrollOfFire.Name = "Lingering Fire Winds Scroll";
            scrollOfFire.ItemType = ItemUseType.Consumable;
            scrollOfFire.ActionVerb = "cast";
            scrollOfFire.Description = "Scroll that spawns gusts of dangerous fire winds.";
            scrollOfFire.Category = ItemCategoryType.Scroll;
            scrollOfFire.Noun = "scroll";
            scrollOfFire.Effect = fireCloudEffect;
            testConsumableList.Add(scrollOfFire);

            Effect frostCloudEffect = EffectCreator.CreateDamageCloudEffect(3, 50, 4, 1, 20, 10, Combat.DamageType.Frost);

            Consumable scrollOfBlizzard = new Consumable();
            scrollOfBlizzard.Name = "Blizard Scroll";
            scrollOfBlizzard.ItemType = ItemUseType.Consumable;
            scrollOfBlizzard.ActionVerb = "cast";
            scrollOfBlizzard.Description = "Scroll that generates a blizzard in a nearby area of the caster hurting anyone who enters.";
            scrollOfBlizzard.Category = ItemCategoryType.Scroll;
            scrollOfBlizzard.Noun = "scroll";
            scrollOfBlizzard.Effect = frostCloudEffect;
            testConsumableList.Add(scrollOfBlizzard);

            Effect corruptionCloudEffect = EffectCreator.CreateDamageCloudEffect(3, 50, 4, 1, 20, 10, Combat.DamageType.Corruption);

            Consumable scrollOfCorruption = new Consumable();
            scrollOfCorruption.Name = "Ancient Scroll of Demise";
            scrollOfCorruption.ItemType = ItemUseType.Consumable;
            scrollOfCorruption.ActionVerb = "cast";
            scrollOfCorruption.Description = "A scroll capable of conjuring putrid clouds.";
            scrollOfCorruption.Category = ItemCategoryType.Scroll;
            scrollOfCorruption.Noun = "scroll";
            scrollOfCorruption.Effect = corruptionCloudEffect;
            testConsumableList.Add(scrollOfCorruption);

            Effect conjureHotSpringEffect = EffectCreator.CreateConjureHotSpringEffect();

            Consumable scrollOfSteam = new Consumable();
            scrollOfSteam.Name = "Conjure Hot Spring";
            scrollOfSteam.ItemType = ItemUseType.Consumable;
            scrollOfSteam.ActionVerb = "cast";
            scrollOfSteam.Description = "Conjures a permanent hotspring on location.";
            scrollOfSteam.Category = ItemCategoryType.Scroll;
            scrollOfSteam.Noun = "scroll";
            scrollOfSteam.Effect = conjureHotSpringEffect;
            testConsumableList.Add(scrollOfSteam);

            Random r = new Random();
            MapContainer chest = new MapContainer();
            for (int i = 0; i < 26; i++)
            {
                //Add a random item to the container.
                Item newItem = testConsumableList[r.Next(0, testConsumableList.Count)].Clone();

                chest.Container.AddItem(newItem);
            }
            chest.X = 1;
            chest.ContainerDescription = "Old Chest";
            chunk.AddMapObject(chest);
            //END TEST
        }

        protected override void OnSetPayload()
        {
            base.OnSetPayload();

            if (lastPayload == null)
            {
                return;
            }

            //Handle payload data from callbacks from other menus.
            ItemPickupResult itemPickupResult = lastPayload as ItemPickupResult;
            if (itemPickupResult != null)
            {
                //TODO: Handle stealing or whatever else needs doing. Container management is done within its own menu.
            }

            MapObjectInteractionResult mapInteractionResult = lastPayload as MapObjectInteractionResult;
            if (mapInteractionResult != null)
            {
                MapReturnObj returnObj = mapInteractionResult.InteractedMapObject as MapReturnObj;
                if (returnObj != null && mapStack.Count > 0)
                {
                    //Begin player exit logic as needed.
                    OnPlayerExitMap();

                    //Grab the last map location pair on the stack.
                    var mapLocationPair = PopStack();

                    //Get the new map
                    map = mapLocationPair.Value1;

                    //Set the renderer to use the new map.
                    mapRenderer.Map = map;

                    OnPlayerEnterMap(mapLocationPair.Value2.X, mapLocationPair.Value2.Y);

                    //We're done so end.
                    return;
                }

                //Now check if it was an entrance marker.
                MapEntranceObj mapEntranceMarker = mapInteractionResult.InteractedMapObject as MapEntranceObj;
                
                if (mapEntranceMarker != null)
                {
                    //Add the current map to the stack
                    AddToStack(map, player.X, player.Y);

                    //Handle the actions needed for exiting the map.
                    OnPlayerExitMap();

                    //Get the new map
                    map = mapInteractionResult.Map.GetSubMap(mapEntranceMarker.DungeonId);

                    //Set the renderer to use the new map.
                    mapRenderer.Map = map;

                    //Now handle actions required for entering a new map.
                    OnPlayerEnterMap(mapEntranceMarker.StartX, mapEntranceMarker.StartY);

                    //We're done, so end.
                    return;
                }
            }

        }

        protected override void CustomRender()
        {
            if (detailedMode)
            {
                detailedAreaViewTimer.Stop();
            }

            //Ensure the map being rendered is the current one 
            mapRenderer.Map = map;

            //Begin the actual rendering.
            mapRenderer.DrawMapCenterOnCoordinates(player.X, player.Y);

            if (debugMode)
            {
                var result = mapRenderer.GetConsoleSpaceFromWorldSpace(player.X, player.Y);
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

                Console.WriteLine($"Chunk seed is: " + map.GetChunkAtWorldSpace(player.X, player.Y).Seed);
            }

            if (detailedMode)
            {
                Console.WriteLine($"You look around and see: ");

                MapChunk chunk = map.GetChunkAtWorldSpace(player.X, player.Y);
                List<MapObject> objects = chunk.GetAllObjectsAtWorldCoordinates(player.X, player.Y);
                
                if (objects != null)
                {
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

            //result.CustomMessage = $"INPUT: Cancelled? {input.Canceled} Value: {input.Text}";

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
                UpdateMap();
            }
            else if (Helper.StringEquals(input.Text, "s")
                || input.Key.Key == ConsoleKey.DownArrow)
            {//Move south
                map.MoveObject(player, 0, 1);
                OnPlayerAction();
                OnPlayerMove();
                UpdateMap();
            }
            else if (Helper.StringEquals(input.Text, "a")
                || input.Key.Key == ConsoleKey.LeftArrow)
            {//Move west
                map.MoveObject(player, -1, 0);
                OnPlayerAction();
                OnPlayerMove();
                UpdateMap();
            }
            else if (Helper.StringEquals(input.Text, "d")
                || input.Key.Key == ConsoleKey.RightArrow)
            {//Move east
                map.MoveObject(player, 1, 0);
                OnPlayerAction();
                OnPlayerMove();
                UpdateMap();
            }
            else if (input.Key.Key == ConsoleKey.Spacebar)
            {//Spacebar acts as a map update key / empty action.
                UpdateMap();
            }
            else if (input.Key.Key == ConsoleKey.E 
                || input.Key.Key == ConsoleKey.I)
            {
                detailedMode = true;
                result.Payload = new ContainerInteractionResult(map, null, player)
                {
                    ViewingContainer = player.Inventory
                };
                result.Action = "InventoryMenu";
            }
            else if (input.Key.Key == ConsoleKey.Escape)
            {
                detailedMode = true;
                result.Action = "PauseMenu";
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

            if (objects == null)
                return null;

            //Ensure the player is not in the list of interactable objects.
            objects.Remove(player); 

            if (index >= objects.Count)
            {//Selected index is now out of bounds of the available objects to select.
                return null;
            }

            MapObject interactingObject = objects[index];
            if (interactingObject != null && interactingObject != player)
            {
                Maps.MapObjects.MapObjectInteractionResult interactionResult = interactingObject.Interact(map, player);
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
            player.SetStamina(player.Stamina - 1);
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

        /// <summary>
        /// Forces a single update for the active map.
        /// </summary>
        private void UpdateMap()
        {
            //Update the map and any calculations it needs to do.
            map.Update(GameState, player.X, player.Y, 10, 20);
        }


        private void OnPlayerExitMap()
        {
            map.RemoveObject(player);

        }

        private void OnPlayerEnterMap(long startX, long startY)
        {
            player.X = startX;
            player.Y = startY;

            map.AddObject(player);
        }

        /// <summary>
        /// Adds the given map and its world x and y to the map stack.
        /// </summary>
        /// <param name="map"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        private void AddToStack(Map map, long x, long y)
        {
            Pair<Map, PointL> newPair = new Pair<Map, PointL>(map, new PointL(x, y));

            mapStack.Add(newPair);
        }

        /// <summary>
        /// Returns the last instance on the map stack and removes it from the stack.
        /// </summary>
        private Pair<Map, PointL> PopStack()
        {
            if (mapStack.Count > 0)
            {
                var stackPair = mapStack.Last();
                mapStack.RemoveAt(mapStack.Count - 1);
                return stackPair;
            }
            return null;
        }

    }
}

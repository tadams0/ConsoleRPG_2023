using ConsoleRPG_2023.RolePlayingGame.Dungeons;
using ConsoleRPG_2023.RolePlayingGame.Maps;
using ConsoleRPG_2023.RolePlayingGame.Maps.MapObjects;
using ConsoleRPG_2023.RolePlayingGame.Renderers;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace ConsoleRPG_2023.RolePlayingGame.Menus
{
    internal class DungeonExploreMenu : Menu
    {
        private DungeonMap dungeon;

        private MapRenderer mapRenderer;

        private long previousPlayerX;
        private long previousPlayerY;

        private Map previousMap;

        private Character player;

        protected override void OnSetGameState()
        {
            base.OnSetGameState();

            player = GameState.PlayerCharacter;

        }

        protected override void OnSetPayload()
        {
            MapObjectInteractionResult result = (MapObjectInteractionResult)lastPayload;
            MapDungeonObj dungeonMarker = (MapDungeonObj)result.MapObject;
            dungeon = result.Map.GetDungeon(dungeonMarker.DungeonId);

            previousMap = result.Map;

            mapRenderer = new MapRendererExtraColorRange(dungeon, GameState, Settings, true);

            mapRenderer.Map = dungeon;
            //TODO: Log if the dungeon was still null for some reason. That would be an error.

            //If we recieved a payload, then it is assumed the player entered the dungeon and we're in the menu to view it.
            OnPlayerEnterDungeon();

        }

        protected override void CustomRender()
        {
            base.CustomRender();

            mapRenderer.DrawMapCenterOnCoordinates(player.X, player.Y);
        }

        protected override OptionDisplay CreateOptions()
        {
            OptionDisplay options = base.CreateOptions();

            options.AddOption("Exit", 4, x => Exit(x));

            return options;
        }

        protected override string CreateMessage()
        {
            return $"Insert dungeon description here. Dungeon {dungeon.Name}";
        }

        public override InputResult GetMenuInput()
        {
            return Helper.GetInput(true);
        }

        private MenuResult Exit(InputResult input)
        {
            MenuResult result = new MenuResult();
            result.Action = Helper.ActionBackOrReturn;

            OnPlayerExitDungeon();

            return result;
        }

        public override MenuResult Update(InputResult input, OptionDisplay usedDisplay)
        {
            MenuResult result = base.Update(input, usedDisplay);

            if (result.Action != Helper.ActionBackOrReturn)
            {//Unless the player is returning, then no action should be taken.
                result.Action = Helper.ActionNone;
            }

            //The player can always freely move around regardless of the mode state.
            if (Helper.StringEquals(input.Text, "w")
                    || input.Key.Key == ConsoleKey.UpArrow)
            {//Move north
                dungeon.MoveObject(player, 0, -1);
            }
            else if (Helper.StringEquals(input.Text, "s")
                || input.Key.Key == ConsoleKey.DownArrow)
            {//Move south
                dungeon.MoveObject(player, 0, 1);
            }
            else if (Helper.StringEquals(input.Text, "a")
                || input.Key.Key == ConsoleKey.LeftArrow)
            {//Move west
                dungeon.MoveObject(player, -1, 0);
            }
            else if (Helper.StringEquals(input.Text, "d")
                || input.Key.Key == ConsoleKey.RightArrow)
            {//Move east
                dungeon.MoveObject(player, 1, 0);
            }

            /*
            if (!usedDisplay.HasOption(input.NumericAsInt))
            {//Custom message for when the player selects something that wasn't within the main menu.
                result.CustomMessage = "Please select a valid number from 1-4";
                result.Action = Helper.ActionNone;
            }
            */

            return result;
        }

        private void OnPlayerExitDungeon()
        {
            dungeon.RemoveObject(player);

            player.X = previousPlayerX;
            player.Y = previousPlayerY;

        }

        private void OnPlayerEnterDungeon()
        {
            previousPlayerX = player.X;
            previousPlayerY = player.Y;
            player.X = dungeon.Entrance.X;
            player.Y = dungeon.Entrance.Y;

            dungeon.AddObject(player);
        }
    }

}

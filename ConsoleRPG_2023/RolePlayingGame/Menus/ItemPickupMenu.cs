using ConsoleRPG_2023.RolePlayingGame.Items;
using ConsoleRPG_2023.RolePlayingGame.MenuPayloads;
using ConsoleRPG_2023.RolePlayingGame.Renderers;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleRPG_2023.RolePlayingGame.Menus
{
    internal class ItemPickupMenu : Menu
    {

        private ItemInteractionResult payload;

        private Item item;

        private ItemRenderer itemRenderer = new ItemRenderer();

        protected override void OnSetGameState()
        {
            base.OnSetGameState();
        }

        protected override OptionDisplay CreateOptions()
        {
            OptionDisplay options = base.CreateOptions();

            options.AddOption("Return", 1, x => Return(x));

            if (item.Owner != null
                && item.Owner != GameState.PlayerCharacter)
            {
                options.AddOption("Steal", 2, x => StealItem(x));
            }
            else
            {
                options.AddOption("Take", 2, x => TakeItem(x));
            }

            return options;
        }

        protected override void OnSetPayload()
        {
            base.OnSetPayload();

            if (lastPayload != null)
            {
                payload = (ItemInteractionResult)lastPayload;
                if (payload == null)
                {
                    throw new NullReferenceException("No payload given for the item pickup menu.");
                }

                item = payload.Item;
            }

        }

        protected override string CreateMessage()
        {
            return $"You see a {item.Noun} {payload.LocationDescription}.\n{itemRenderer.GetContainerDisplay(item, Console.WindowWidth)}\n{item.Description}";
        }

        public override InputResult GetMenuInput()
        {
            return Helper.GetInput(true);
        }

        private MenuResult StealItem(InputResult input)
        {
            MenuResult result = new MenuResult();

            ItemPickupResult pickupResult = new ItemPickupResult();
            pickupResult.Item = item;
            pickupResult.Stolen = false;
            result.Payload = pickupResult;

            //Add item to player's inventory and remove it from the map.
            payload.Character.Inventory.AddItem(item);
            payload.Map.RemoveObject(payload.MapObject);

            result.Action = Helper.ActionBackOrReturn;
            return result;
        }

        private MenuResult TakeItem(InputResult input)
        {
            MenuResult result = new MenuResult();

            ItemPickupResult pickupResult = new ItemPickupResult();
            pickupResult.Item = item;
            pickupResult.Stolen = false;
            result.Payload = pickupResult;

            //Add item to player's inventory and remove it from the map.
            payload.Character.Inventory.AddItem(item);
            payload.Map.RemoveObject(payload.MapObject);

            result.Action = Helper.ActionBackOrReturn;
            return result;
        }

        private MenuResult Return(InputResult input)
        {
            MenuResult result = new MenuResult();
            result.Action = Helper.ActionBackOrReturn;
            return result;
        }

        public override MenuResult Update(InputResult input, OptionDisplay usedDisplay)
        {
            MenuResult result = base.Update(input, usedDisplay);

            if (!usedDisplay.HasOption(input.NumericAsInt))
            {//Custom message for when the player selects something that wasn't within the main menu.
                result.CustomMessage = "Please select a valid number from 1-2";
                result.Action = Helper.ActionNone;
            }

            return result;
        }
    }

}

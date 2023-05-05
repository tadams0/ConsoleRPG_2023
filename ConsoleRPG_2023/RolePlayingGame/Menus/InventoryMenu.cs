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

namespace ConsoleRPG_2023.RolePlayingGame.Menus
{
    internal class InventoryMenu : Menu
    {

        private ContainerInteractionResult payload;

        private Container previousContainer;

        /// <summary>
        /// The container being viewed by the inventory menu.
        /// </summary>
        private Container viewingContainer;

        /// <summary>
        /// The container which items will flow into.
        /// </summary>
        private Container intoContainer;

        private string viewingContainerDescription;

        private GridRenderer renderer;

        private int selectedIndex = 0;

        private ItemRenderer itemRenderer = new ItemRenderer();

        private Item selectedItem;

        private bool storageMode = false;

        protected override void OnSetGameState()
        {
            base.OnSetGameState();
            /*
            viewingContainer = new Container();

            for (int i = 0; i < 250; i++)
            {
                if (i % 3 == 0)
                {
                    Consumable c = new Consumable();
                    c.Name = "Scroll of Testing " + i;
                    c.ActionVerb = "Cast";
                    c.Description = "A scroll that can be casted to test consumable functionality on items within a menu.";
                    viewingContainer.AddItem(c);
                }
                else if (i % 5 == 0)
                {
                    Consumable c = new Consumable();
                    c.Name = "Bread " + i;
                    c.ActionVerb = "Eat";
                    c.Description = "Basic food consumed by a wide range of cultures.";
                    viewingContainer.AddItem(c);
                }
                else if (i % 2 == 0)
                {
                    Consumable c = new Consumable();
                    c.Name = "Potion of Healing " + i;
                    c.ActionVerb = "Drink";
                    c.Description = "Medicine that can be used to cure minor or basic wounds.";
                    viewingContainer.AddItem(c);
                }
                else
                {
                    Item item = new Item();
                    item.Name = "Test item #" + i;
                    item.Description = "A useless bauble from ancient times. It has no known use.";
                    viewingContainer.AddItem(item);
                }
            }
            */

            renderer = new GridRenderer(Console.WindowWidth);
            //renderer.SetFilter(x=>x.OrderByDescending(y=>((Item)y).Name).ToList());
            //PopulateRendererWithContainer(viewingContainer);
        }

        protected override void OnSetPayload()
        {
            base.OnSetPayload();

            previousContainer = viewingContainer;
            intoContainer = null;
            storageMode = false;
            viewingContainerDescription = "[No description found (ut oh!)]";
            payload = (ContainerInteractionResult)lastPayload;
            if (payload != null)
            {
                Container container = payload.ViewingContainer;
                if (container != null)
                {
                    viewingContainer = container;
                    PopulateRendererWithContainer(container);
                }

                viewingContainerDescription = payload.ViewingContainerDescription;

                intoContainer = payload.IntoContainer;
            }

            if (previousContainer != viewingContainer)
            {//If the containers are not the same, then we need to reset the selected index.
                selectedIndex = 0;
                selectedItem=null;
            }
        }

        private void PopulateRendererWithContainer(Container c)
        {
            renderer.Clear();

            int rendererMaxSize = renderer.GetMaximumItemDisplayWidth();

            var items = c.GetItems();
            for (int i = 0; i <  items.Count; i++)
            {
                renderer.AddItem(items[i], x => itemRenderer.GetContainerDisplay((Item)x, rendererMaxSize));
            }
        }

        protected override OptionDisplay CreateOptions()
        {
            OptionDisplay options = base.CreateOptions();


            options.AddOption("Return", 1, x => Return(x));
            int ConsumeNumber = 2;
            if (payload.IntoContainer != null && viewingContainer.Count > 0)
            {
                ConsumeNumber = 4;
                if (storageMode)
                {//Change the verbage to match the expected "mode".
                    options.AddOption("Store", 2, x => TakeItem(x));
                    options.AddOption("Store All", 3, x => TakeAllItems(x));
                }
                else
                {
                    options.AddOption("Take", 2, x => TakeItem(x));
                    options.AddOption("Take All", 3, x => TakeAllItems(x));
                }
            }

            //+1 to account for storage mode options.
            ConsumeNumber += 1;

            if (storageMode)
            {
                options.AddOption("Take Items", ConsumeNumber - 1, SwapModes);
            }
            else
            {
                options.AddOption("Store Items", ConsumeNumber - 1, SwapModes);
            }

            if (selectedItem != null)
            {
                if (selectedItem.ItemType == ItemUseType.Consumable)
                {
                    Consumable c = (Consumable)selectedItem;
                    string action = Helper.FirstCharToUpper(c.ActionVerb);
                    options.AddOption(action, ConsumeNumber, x => ConsumeItem(x));
                }
            }

            return options;
        }

        protected override string CreateMessage()
        {
            return string.Empty;
        }

        protected override void CustomRender()
        {
            if (payload.MapObject == null)
            {//The mapobject is null when the player is viewing their own inventory.
                Helper.WriteFormattedString($"When looking inside your inventory, you find:");
            }
            else
            {
                Helper.WriteFormattedString($"Inside the {viewingContainerDescription}, you find:");
            }

            renderer.Render(selectedIndex);

            object selectedObject = renderer.GetObjectAtIndex(selectedIndex);
            if (selectedObject != null)
            {
                this.selectedItem = (Item)selectedObject;
                if (this.selectedItem != null)
                {
                    Helper.WriteFormattedString(selectedItem.Description);
                }
            }

            if (renderer.LastRenderedItemCount == 0)
            {
                Helper.WriteFormattedString($"There's nothing here.");
            }
            else
            {
                Helper.WriteFormattedString($"Use {Helper.FormatString("WASD", Color.Green)} or {Helper.FormatString("↑←↓→", Color.Green)} and {Helper.FormatString("ENTER", Color.Green)} to select an item.");
            }

        }

        public override InputResult GetMenuInput()
        {
            return Helper.GetInput(true);
        }

        public override MenuResult Update(InputResult input, OptionDisplay usedDisplay)
        {
            MenuResult result = base.Update(input, usedDisplay);

            //The player can always freely move around regardless of the mode state.
            if (Helper.StringEquals(input.Text, "w")
                    || input.Key.Key == ConsoleKey.UpArrow)
            {//Move up
                int expectedIndex = selectedIndex - renderer.Columns;
                if (expectedIndex >= renderer.MinIndex)
                {
                    selectedIndex = expectedIndex;
                }
            }
            else if (Helper.StringEquals(input.Text, "s")
                || input.Key.Key == ConsoleKey.DownArrow)
            {//Move down
                int expectedIndex = selectedIndex + renderer.Columns;
                if (expectedIndex < renderer.LastRenderedItemCount)
                {
                    selectedIndex = expectedIndex;
                }
            }
            else if (Helper.StringEquals(input.Text, "a")
                || input.Key.Key == ConsoleKey.LeftArrow)
            {//Move left
                int expectedIndex = selectedIndex - 1;
                if (selectedIndex % renderer.Columns != 0 //Will not wrap around
                    && expectedIndex >= renderer.MinIndex)
                {
                    selectedIndex = expectedIndex;
                }
            }
            else if (Helper.StringEquals(input.Text, "d")
                || input.Key.Key == ConsoleKey.RightArrow)
            {//Move right
                int expectedIndex = selectedIndex + 1;
                if (expectedIndex < renderer.LastRenderedItemCount
                    && expectedIndex % renderer.Columns != 0) //Did not wrap around.
                {
                    selectedIndex = expectedIndex;
                }
            }
            else if (input.Key.Key == ConsoleKey.Escape)
            {
                result.Action = Helper.ActionBackOrReturn;
            }

            if (input.Key.Key == ConsoleKey.Enter)
            {

            }

            if (string.IsNullOrEmpty(result.Action))
            {
                result.Action = Helper.ActionNone;
            }

            return result;
        }

        private MenuResult ConsumeItem(InputResult input)
        {
            MenuResult result = new MenuResult();
            result.Action = Helper.ActionNone;

            Consumable c = (Consumable)selectedItem;
            
            RemoveSelectedItem();

            //TODO: actually do something with the consumable.

            return result;
        }

        private MenuResult TakeItem(InputResult input)
        {
            MenuResult result = new MenuResult();

            if (selectedItem != null && intoContainer != null)
            {
                intoContainer.AddItem(selectedItem);
                RemoveSelectedItem();
            }

            result.Action = Helper.ActionNone;
            return result;
        }

        private MenuResult TakeAllItems(InputResult input)
        {
            MenuResult result = new MenuResult();

            if (selectedItem != null && intoContainer != null)
            {
                List<Item> itemsToTake = viewingContainer.GetItems();
                intoContainer.AddRange(itemsToTake);
                RemoveAllItems();
            }

            result.Action = Helper.ActionNone;
            return result;
        }

        private MenuResult SwapModes(InputResult input)
        {
            MenuResult result = new MenuResult();
            result.Action = Helper.ActionNone;

            //Swap the containers around.
            Container temp = viewingContainer;
            viewingContainer = intoContainer;
            intoContainer = temp;

            //Swap the mode.
            storageMode = !storageMode;

            PopulateRendererWithContainer(viewingContainer);

            selectedIndex = 0;
            selectedItem = null;

            if (storageMode)
            {
                viewingContainerDescription = payload.IntoContainerDescription;
            }
            else
            {
                viewingContainerDescription = payload.ViewingContainerDescription;
            }

            return result;
        }

        private MenuResult Return(InputResult input)
        {
            MenuResult result = new MenuResult();
            result.Action = Helper.ActionBackOrReturn;
            return result;
        }

        /// <summary>
        /// Removes the selected item from the viewing container.
        /// </summary>
        private void RemoveSelectedItem()
        {
            viewingContainer.RemoveItem(selectedItem);

            if (selectedIndex >= renderer.LastRenderedItemCount - 1)
            {
                selectedIndex -= 1;
            }

            object nextSelectedObj = renderer.GetObjectAtIndex(selectedIndex);

            PopulateRendererWithContainer(viewingContainer);

            if (nextSelectedObj != null)
            {
                selectedItem = (Item)nextSelectedObj;
            }
            else
            {
                selectedItem = null;
            }
        }

        /// <summary>
        /// Removes all items from the viewing container.
        /// </summary>
        private void RemoveAllItems()
        {
            viewingContainer.RemoveItem(selectedItem);
            viewingContainer.Clear();
            selectedIndex = 0;
            renderer.Clear();
            selectedItem = null;
        }

    }

}

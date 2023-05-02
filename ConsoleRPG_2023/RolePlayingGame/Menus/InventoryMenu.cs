using ConsoleRPG_2023.RolePlayingGame.Items;
using ConsoleRPG_2023.RolePlayingGame.Maps;
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
        /// <summary>
        /// The container being viewed by the inventory menu.
        /// </summary>
        private Container viewingContainer;

        private GridRenderer renderer;

        private int selectedIndex = 0;

        private ItemRenderer<Item> itemRenderer = new ItemRenderer<Item>();

        private Item selectedItem;

        protected override void OnSetGameState()
        {
            base.OnSetGameState();


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

            renderer = new GridRenderer(Console.WindowWidth);
            renderer.SetFilter(x=>x.OrderByDescending(y=>((Item)y).Name).ToList());
            PopulateRendererWithContainer(viewingContainer);
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

            if (selectedItem != null)
            {
                if (selectedItem.ItemType == ItemUseType.Consumable)
                {
                    Consumable c = (Consumable)selectedItem;
                    options.AddOption(c.ActionVerb, 2, x => ConsumeItem(x));
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
            renderer.Render(selectedIndex);

            Helper.WriteFormattedString($"Use {Helper.FormatString("WASD", Color.Green)} or {Helper.FormatString("↑←↓→", Color.Green)} and {Helper.FormatString("ENTER", Color.Green)} to select an item.");
            
            object selectedObject = renderer.GetObjectAtIndex(selectedIndex);
            if (selectedObject != null)
            {
                this.selectedItem = (Item)selectedObject;
                if (this.selectedItem != null)
                {
                    Helper.WriteFormattedString(selectedItem.Description);
                }
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

            return result;
        }

        private MenuResult Return(InputResult input)
        {
            MenuResult result = new MenuResult();
            result.Action = Helper.ActionBackOrReturn;
            return result;
        }

    }

}

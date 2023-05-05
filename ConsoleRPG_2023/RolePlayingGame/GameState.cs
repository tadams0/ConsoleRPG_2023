using ConsoleRPG_2023.RolePlayingGame.Items;
using ConsoleRPG_2023.RolePlayingGame.Maps;
using ConsoleRPG_2023.RolePlayingGame.Renderers;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleRPG_2023.RolePlayingGame
{
    /// <summary>
    /// A class which contains all information regarding the current game state.
    /// </summary>
    public class GameState
    {
        public Character PlayerCharacter { get; set; }

        public HUD PlayerHud { get; set; }

        public ProceduralWorldMap WorldMap { get; set; }

        public MapRenderer MapRenderer { get; set; }

        private Dictionary<string, ItemTemplate> itemTemplatesByName = new Dictionary<string, ItemTemplate>();
        private Dictionary<int, ItemTemplate> itemTemplatesById = new Dictionary<int, ItemTemplate>();

        /// <summary>
        /// Adds an item template to the game.
        /// </summary>
        /// <param name="template">The template being added.</param>
        public void AddItemTemplate(ItemTemplate template)
        {
            itemTemplatesById[template.TemplateId] = template;
            itemTemplatesByName[template.Name] = template;
        }

        /// <summary>
        /// Gets the item template which has the given template id.
        /// </summary>
        /// <param name="templateId">The id of the template to fetch.</param>
        public ItemTemplate GetItemTemplate(int templateId)
        {
            return itemTemplatesById[templateId];
        }

        /// <summary>
        /// Gets the item template which has the given template name.
        /// </summary>
        /// <param name="name">The name of the template to fetch.</param>
        public ItemTemplate GetItemTemplate(string name)
        {
            return itemTemplatesByName[name];
        }

        /// <summary>
        /// Gets a clone of the item based on the template which has the given template id.
        /// </summary>
        /// <param name="templateId">The id of the template to fetch.</param>
        public Item GetItemByTemplate(int templateId)
        {
            return itemTemplatesById[templateId].Clone();
        }

        /// <summary>
        /// Gets a clone of the item based on the template which has the given template name.
        /// </summary>
        /// <param name="name">The name of the template to fetch.</param>
        public Item GetItemByTemplate(string name)
        {
            return itemTemplatesByName[name].Clone();
        }

    }
}

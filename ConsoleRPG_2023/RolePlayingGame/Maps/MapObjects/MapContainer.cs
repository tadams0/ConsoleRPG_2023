using ConsoleRPG_2023.RolePlayingGame.Items;
using ConsoleRPG_2023.RolePlayingGame.MenuPayloads;
using ConsoleRPG_2023.RolePlayingGame.Renderers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleRPG_2023.RolePlayingGame.Maps.MapObjects
{
    public class MapContainer : MapObject
    {
        /// <summary>
        /// Gets the related container.
        /// </summary>
        public Container Container
        {
            get { return container; }
        }

        /// <summary>
        /// The noun-type of the chest. e.g. "bag", "chest", "open top barrel".
        /// <br/>Can be multiple words.
        /// </summary>
        public string ContainerDescription { get; set; } = "Chest";

        private Container container = new Container();

        public MapContainer() 
        { 
        }


        public override MapObjectInteractionResult Interact(Map map, Character character)
        {
            ContainerInteractionResult result = new ContainerInteractionResult(map, this, character);
            result.InteractionMessage = string.Empty;
            result.ViewingContainer = container;
            result.IntoContainer = character.Inventory;
            result.ViewingContainerDescription = this.ContainerDescription;
            result.Action = "InventoryMenu";

            if (character.IsPlayer)
            {
                result.IntoContainerDescription = "your inventory";
            }
            else
            {
                result.IntoContainerDescription = $"{character.FirstName} {character.LastName}'s inventory";
            }

            return result;
        }

        public override string ToString()
        {
            if (container.Count == 0)
            {
                return $"{ContainerDescription} (Empty)";
            }
            else
            {
                return ContainerDescription;
            }
        }

    }
}

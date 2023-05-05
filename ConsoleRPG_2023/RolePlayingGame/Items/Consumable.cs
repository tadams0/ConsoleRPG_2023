using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleRPG_2023.RolePlayingGame.Items
{
    /// <summary>
    /// Defines a consumable item.
    /// </summary>
    public class Consumable : Item
    {

        /// <summary>
        /// The action verb used by the consumable.
        /// </summary>
        public string ActionVerb
        {
            get { return actionVerb; }
            set { actionVerb = value; }
        }

        private string actionVerb = "eat";

        public Consumable() 
        {
            this.ItemType = ItemUseType.Consumable;
        
        }

        public override Item Clone()
        {
            Consumable clone = new Consumable()
            {
                Name = Name,
                Description = Description,
                Owner = Owner,
                Category = Category,
                Noun = Noun,
                ItemType = ItemType,
                ActionVerb = ActionVerb
            };

            return clone;
        }

    }
}

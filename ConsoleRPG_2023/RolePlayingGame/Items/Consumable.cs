using ConsoleRPG_2023.RolePlayingGame.Effects;
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

        /// <summary>
        /// Gets or sets the effect of the consumable. Null implies there is no effect.
        /// </summary>
        public Effect Effect { get; set; } = null;

        private string actionVerb = "eat";

        public Consumable() 
        {
            this.ItemType = ItemUseType.Consumable;
        
        }

        /// <summary>
        /// Creates a clone of the consumable instance. The clone will share a reference to the effect property.
        /// </summary>
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
                ActionVerb = ActionVerb,
                Effect = Effect
            };

            return clone;
        }

    }
}

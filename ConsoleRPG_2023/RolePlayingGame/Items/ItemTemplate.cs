using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleRPG_2023.RolePlayingGame.Items
{
    public class ItemTemplate : Item
    {
        private static int idCounter = 0;

        /// <summary>
        /// The id of the template.
        /// </summary>
        public int TemplateId { get; set; } = idCounter++;

        /// <summary>
        /// The name for the template. Mostly used for lookup purposes.
        /// </summary>
        public string TemplateName { get; set; }

    }
}

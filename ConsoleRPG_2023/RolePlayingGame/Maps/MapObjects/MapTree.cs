using ConsoleRPG_2023.RolePlayingGame.Maps.MapObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleRPG_2023.RolePlayingGame.Maps
{
    public class MapTree : MapObject
    {
        public TreeType TreeType { get; set; }

        public MapTree(TreeType type) 
        { 
            TreeType = type;
        }

        public override string ToString()
        {
            string enumValue = Enum.GetName(typeof(TreeType), TreeType);
            enumValue = enumValue.Replace("_", "");
            enumValue = enumValue.Substring(0,1).ToUpper() + enumValue.Substring(1);
            return enumValue + " Tree";
        }

        public override MapObjectInteractionResult Interact(Map map, Character character)
        {
            var result = base.Interact(map, character);

            result.InteractionMessage = $"Yup that's definitely a {this.ToString()}";

            return result;
        }

    }
}

using ConsoleRPG_2023.RolePlayingGame.Maps.MapObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleRPG_2023.RolePlayingGame.Maps
{
    /// <summary>
    /// Defines a map object that triggers a return to the directly previous map on the <see cref="ConsoleRPG_2023.RolePlayingGame.Menus.MapExploreMenu"/> stack.
    /// </summary>
    public class MapReturnObj : MapObject
    {
        public string ReturningMapName
        {
            get { return name; }
            set { name = value; }
        }

        private string name = "[Unnamed Map Return Obj]";
        private long dungeonId = 0;

        public MapReturnObj()
        {
        }

        public override MapObjectInteractionResult Interact(Map map, Character character)
        {
            var result = base.Interact(map, character);
            
            //Navigate over to the dungeon explore menu.
            result.Action = "MapExploreMenu";
            result.InteractionMessage = string.Empty;

            return result;
        }

        public override string ToString()
        {
            return name;
        }
    }
}

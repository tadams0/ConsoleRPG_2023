using ConsoleRPG_2023.RolePlayingGame.Maps.MapObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleRPG_2023.RolePlayingGame.Maps
{
    public class MapDungeonObj : MapObject
    {
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        /// <summary>
        /// The id of the dungeon this map dungeon is tied to.
        /// </summary>
        public long DungeonId
        {
            get { return dungeonId; }
        }

        private string name = "Test Dungeon Entrance";
        private long dungeonId = 0;

        public MapDungeonObj(long dungeonId)
        {
            this.dungeonId = dungeonId;
        }

        public override MapObjectInteractionResult Interact(Map map, Character character)
        {
            var result = base.Interact(map, character);
            
            //Navigate over to the dungeon explore menu.
            result.Action = "DungeonExploreMenu";
            result.InteractionMessage = string.Empty;

            return result;
        }

        public override string ToString()
        {
            return name;
        }
    }
}

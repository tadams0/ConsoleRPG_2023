using ConsoleRPG_2023.RolePlayingGame.Maps.MapObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleRPG_2023.RolePlayingGame.Maps
{
    public class MapEntranceObj : MapObject
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

        /// <summary>
        /// The starting x position in the new map if this entrance is used.
        /// </summary>
        public long StartX { get; set; }

        /// <summary>
        /// The starting y position in the new map if this entrance is used.
        /// </summary>
        public long StartY { get; set; }

        private string name = "[Unnamed Map Entrance Obj]";
        private long dungeonId = 0;

        public MapEntranceObj(long dungeonId, long startX, long startY)
        {
            this.dungeonId = dungeonId;
            this.StartX = startX;
            this.StartY = startY;
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

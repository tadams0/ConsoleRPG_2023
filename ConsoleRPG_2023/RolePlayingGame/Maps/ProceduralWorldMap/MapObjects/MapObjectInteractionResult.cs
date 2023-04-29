using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleRPG_2023.RolePlayingGame.Maps.MapObjects
{
    /// <summary>
    /// Defines a class that holds data from a map object interaction.
    /// </summary>
    public class MapObjectInteractionResult
    {
        public MapObject MapObject
        {
            get { return mapObject; }
            set { mapObject = value; }
        }

        public Map Map
        {
            get { return map; }
            set { map = value; }
        }

        public string Action
        {
            get { return action; }
            set { action = value; }
        }

        public string InteractionMessage
        {
            get { return interactionMessage; }
            set { interactionMessage = value; }
        }


        private MapObject mapObject;
        private Map map;

        private string action = Helper.ActionNone;

        private string interactionMessage = string.Empty;

        public MapObjectInteractionResult(Map map, MapObject mapObject)
        {
            this.map = map;
            this.mapObject = mapObject;
        }

    }
}

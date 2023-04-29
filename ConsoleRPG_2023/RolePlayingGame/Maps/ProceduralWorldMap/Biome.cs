using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleRPG_2023.RolePlayingGame.Maps
{
    /// <summary>
    /// Defines an object which holds settings for map generation in regards to
    /// tile type selection.
    /// </summary>
    public class Biome
    {
        private static int idCounter = 1;

        public int Id { get; set; } = idCounter++;

        public BiomeType Type { get; }

        public bool AllowAnyTileTypeBlending { get; set; } = true;

        private HashSet<TileType> allowedTilesForBlending = new HashSet<TileType>();

        private SortedDictionary<int, Func<BiomeData, TileType>> tileMappings = new SortedDictionary<int, Func<BiomeData, TileType>>();

        private SortedDictionary<int, Func<BiomeData, Tile, MapObject>> objectInitMutliTileFunctions = new SortedDictionary<int, Func<BiomeData, Tile, MapObject>>();

        private SortedDictionary<int, Func<BiomeData, Tile, MapObject>> objectInitSingleTileFunctions = new SortedDictionary<int, Func<BiomeData, Tile, MapObject>>();


        public Biome(BiomeType biomeType)
        {
            Type = biomeType;
        }

        public void AddObjectInitSingleTileFunction(int priority, Func<BiomeData, Tile, MapObject> function)
        {
            objectInitSingleTileFunctions[priority] = function;
        }

        public List<MapObject> GetSingleTileObjects(BiomeData data, Tile tile)
        {
            List<MapObject> result = new List<MapObject>();
            MapObject currentObj = null;
            foreach (var pair in objectInitSingleTileFunctions)
            {
                currentObj = pair.Value(data, tile);
                if (currentObj != null)
                {
                    result.Add(currentObj);
                }
            }
            return result;
        }


        public void AddTileType(int priority, Func<BiomeData, TileType> function)
        {
            tileMappings[priority] = function;
        }

        /// <summary>
        /// Adds a tile type to the whitelist for allowed tiles that can blend into the biome.
        /// </summary>
        /// <param name="tileType"></param>
        public void AddTileTypeForBlending(TileType tileType)
        {
            allowedTilesForBlending.Add(tileType);
        }

        /// <summary>
        /// Determines if the given tile type is a valid type for blending into the biome.
        /// </summary>
        /// <param name="tileType"></param>
        /// <returns></returns>
        public bool CanBlendTileType(TileType tileType)
        {
            return allowedTilesForBlending.Contains(tileType);
        }

        public TileType GetTileType(BiomeData data)
        {
            TileType result = TileType.None;
            foreach (var pair in tileMappings)
            {
                result = pair.Value(data);
                if (result != TileType.None)
                {
                    return result;
                }
            }
            return result;
        }

    }
}

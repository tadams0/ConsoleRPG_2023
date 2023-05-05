﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace ConsoleRPG_2023.RolePlayingGame.Maps
{
    /// <summary>
    /// Defines a section of map.
    /// </summary>
    public class MapChunk
    {
        public long Id {  get; }

        /// <summary>
        /// The left position of the chunk.
        /// </summary>
        public int X { get; }

        /// <summary>
        /// The top y position of the chunk.
        /// </summary>
        public int Y { get; }

        /// <summary>
        /// The number of tiles along the x axis of this chunk.
        /// </summary>
        public int Width
        {
            get { return width; }
        }

        /// <summary>
        /// The number of tiles along the y axis of this chunk.
        /// </summary>
        public int Height
        {
            get { return height; }
        }

        private int width;
        private int height;

        private Tile[] tiles;

        private Dictionary<byte, List<MapObject>> mapObjects = new Dictionary<byte, List<MapObject>>();
        
        /// <summary>
        /// A cache mapping for quickly discovering the location of an object within the chunk without knowing it's x and y.
        /// </summary>
        private Dictionary<MapObject, byte> mapObjectToKey = new Dictionary<MapObject, byte>();

        public MapChunk(long id, int x, int y, int width, int height) 
        {
            Id = id;
            X = x;
            Y = y;
            this.width = width;
            this.height = height;
            tiles = new Tile[width * height];
            GenerateDefaultChunk();
        }

        /// <summary>
        /// Fills the chunk with generated tiles that are set to default values.
        /// </summary>
        private void GenerateDefaultChunk()
        {
            byte tileId;
            Tile newTile;
            for (byte i = 0; i <  width; i++)
            {
                for (byte j = 0; j < height; j++)
                {
                    tileId = (byte)(j * width + i);
                    newTile = new Tile(tileId);
                    tiles[tileId] = newTile;
                }
            }
        }

        /*
        public Tile GetTileAtWorldCoordinates(int x, int y)
        {
            //Must convert from world coordinates to local coordinates (0-width on x and 0-height on y).
            //To do this, we need to subtract the x and y of this chunk which is in world coordinates.

            int localX = x - X;
            int localY = y - Y;
            return GetTileRelativeCoordinates(localX, localY);
        }
        */

        /// <summary>
        /// Adds a map object to the chunk.
        /// </summary>
        /// <param name="x">The world x coordinate to place the object.</param>
        /// <param name="y">The world y coordinate to place the object.</param>
        /// <param name="obj">The object to add to the chunk.</param>
        public void AddMapObject(long x, long y, MapObject obj)
        {
            Point p = ConvertWorldCoordinatesToChunkCoordinates(x, y);
            byte objectId = GetTileId(p.X, p.Y);

            //Use id to store the object in this chunk.
            if (!mapObjects.ContainsKey(objectId))
            {
                mapObjects[objectId] = new List<MapObject>();
            }

            mapObjects[objectId].Add(obj);
            mapObjectToKey[obj] = objectId;
        }

        /// <summary>
        /// Adds a map object to the chunk.
        /// </summary>
        /// <param name="x">The world x coordinate to place the object.</param>
        /// <param name="y">The world y coordinate to place the object.</param>
        /// <param name="obj">The object to add to the chunk.</param>
        public void AddRangeMapObject(long x, long y, IEnumerable<MapObject> objs)
        {
            Point p = ConvertWorldCoordinatesToChunkCoordinates(x, y);
            byte objectId = GetTileId(p.X, p.Y);

            //Use id to store the object in this chunk.
            if (!mapObjects.ContainsKey(objectId))
            {
                mapObjects[objectId] = new List<MapObject>();
            }

            mapObjects[objectId].AddRange(objs);
            foreach (MapObject obj in objs)
            {
                mapObjectToKey[obj] = objectId;
            }
        }

        /// <summary>
        /// Adds a map object to the chunk.
        /// </summary>
        /// <param name="obj">The object to add to the chunk.</param>
        public void AddMapObject(MapObject obj)
        {
            Point p = ConvertWorldCoordinatesToChunkCoordinates(obj.X, obj.Y);
            byte objectId = GetTileId(p.X, p.Y);

            //Use id to store the object in this chunk.
            if (!mapObjects.ContainsKey(objectId))
            {
                mapObjects[objectId] = new List<MapObject>();
            }

            mapObjects[objectId].Add(obj);
            mapObjectToKey[obj] = objectId;
        }

        /// <summary>
        /// Removes the given object from the chunk.
        /// </summary>
        /// <param name="obj">The given object to remove.</param>
        public void RemoveMapObject(MapObject obj)
        {
            //Retrieve the key / 1d coordinate of the map. This is also a handy way of determining
            //if the object is in this chunk to begin with.
            if (mapObjectToKey.TryGetValue(obj, out byte value))
            {
                mapObjectToKey.Remove(obj);
                mapObjects[value].Remove(obj);
            }
        }

        /// <summary>
        /// Removes all map objects located at the specified world position from the chunk.
        /// </summary>
        public void RemoveMapObjects(long x, long y)
        {
            Point p = ConvertWorldCoordinatesToChunkCoordinates(x, y);
            byte squashedCoordinate = GetTileId(p.X, p.Y);

            //Retrieve the key / 1d coordinate of the map. This is also a handy way of determining
            //if the object is in this chunk to begin with.
            if (mapObjects.ContainsKey(squashedCoordinate))
            {
                List<MapObject> objectsToRemove = mapObjects[squashedCoordinate];

                foreach (MapObject obj in objectsToRemove)
                {
                    mapObjectToKey.Remove(obj);
                }

                mapObjects[squashedCoordinate].Clear();
            }
        }

        /// <summary>
        /// Sets the tile at the given x and y world position.
        /// </summary>
        /// <param name="x">The world space x coordinate.</param>
        /// <param name="y">The world space y coordinate.</param>
        /// <param name="tile">The tile to set to the given position.</param>
        public void SetTileByWorldCoordinates(long x, long y, Tile tile)
        {
            byte id = GetTileIdFromWorldCoordinates(x, y);
            tiles[id] = tile;
        }

        /// <summary>
        /// Sets the tile at the given x and y relative position.
        /// </summary>
        /// <param name="x">Relative x where 0 represents the left edge and the width of the chunk represents the right edge.</param>
        /// <param name="y">Relative y where 0 represents the top edge and the height of the chunk represents the bottom edge.</param>
        /// <param name="tile">The tile to set to the given position.</param>
        public void SetTileRelative(int x, int y, Tile tile)
        {
            byte id = GetTileId(x, y);
            tiles[id] = tile;
        }

        /// <summary>
        /// Gets a tile id from the local chunk coordinates.
        /// </summary>
        public byte GetTileId(int x, int y)
        {
            return (byte)(y * width + x);
        }

        /// <summary>
        /// Gets a tile id of the world coordinates.
        /// </summary>
        public byte GetTileIdFromWorldCoordinates(long x, long y)
        {
            return (byte)(Math.Abs((int)(y % height)) * width + Math.Abs((int)(x % width)));
        }

        private Point ConvertWorldCoordinatesToChunkCoordinates(long x, long y)
        {
            //Convert from world space to chunk space...
            //By using modulo we essentially cap the maximum value to the chunk width and height (by default 0-15, which is local only to the specified chunk).
            //Also it's only positive. There are no negative tile ids (it's currently a byte, so minimum value is 0 anyways).
            int localTileX = Math.Abs((int)(x % width));
            int localTileY = Math.Abs((int)(y % height));
            return new Point(localTileX, localTileY);
        }

        /// <summary>
        /// Gets the tile found at the given x and y world coordinates.
        /// </summary>
        public Tile GetTileAtWorldCoordinates(long x, long y)
        {
            //This is repeated to prevent additional overhead of making a point.
            int localTileX = Math.Abs((int)(x % width));
            if (x < 0)
            {//Flip x
                //When negative, -1 is the minimum value, -16 is the highest value possible (in world coordinates).
                localTileX = width - localTileX;
            }
            int localTileY = Math.Abs((int)(y % height));
            if (y < 0)
            {//Flip y
                localTileY = height - localTileY;
            }
            return GetTileRelativeCoordinates(localTileX, localTileY);
        }

        /// <summary>
        /// Gets the first map object found at the given x and y world coordinates.
        public List<MapObject> GetAllObjectsAtWorldCoordinates(long x, long y)
        {
            byte tileId = GetTileIdFromWorldCoordinates(x, y);
            if (mapObjects.TryGetValue(tileId, out List<MapObject> objects))
            {
                return new List<MapObject>(objects);
            }
            return null;
        }

        /// <summary>
        /// Gets the first instance of a map dungeon located at the given x and y position.
        /// </summary>
        public MapDungeonObj GetMapDungeonAtWorldCoordinates(long x, long y)
        {
            byte tileId = GetTileIdFromWorldCoordinates(x, y);
            if (mapObjects.TryGetValue(tileId, out List<MapObject> objects))
            {
                foreach (MapObject obj in objects)
                {
                    if (obj is MapDungeonObj)
                    {
                        return (MapDungeonObj)obj;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Gets the first map object found at the given x and y world coordinates.
        public MapObject GetObjectAtWorldCoordinates(long x, long y)
        {
            byte tileId = GetTileIdFromWorldCoordinates(x, y);
            if (mapObjects.TryGetValue(tileId, out List<MapObject> objects))
            {
                if (objects.Count > 0)
                {
                    return objects[0];
                }
            }
            return null;
        }

        public Tile GetTileRelativeCoordinates(int x, int y)
        {
            byte tileId = (byte)(y * width + x);
            return tiles[tileId];
        }

        /// <summary>
        /// Checks if the given coordinates within world space are 
        /// within the bounds of this <see cref="MapChunk"/>.
        /// </summary>
        public bool AreWorldCoordinatesOutOfBounds(int x, int y)
        {
            return x >= X && y >= Y && x < X + width && y < Y + height; //Checking if the cooridnates within range.
        }


        /// <summary>
        /// Determines if the given x and y LOCAL/Relative coordinates are within bounds. This means 0,0 is the top left tile of this chunk, and the width,height would be the bottom right.
        /// </summary>
        /// <param name="x">The horizontal coordinate. Left is negative, right is positive.</param>
        /// <param name="y">The vertical coordinate. Up is negative, down is positive.</param>
        /// <returns>True if the x and y are within bounds of this <see cref="MapChunk"/> instance. False otherwise.</returns>
        public bool IsOutOfBounds(int x, int y)
        {
            if (x < 0 || y < 0)
                return true;
            if (x >= width || y >= height)
                return true;

            return false;
        }


    }
}

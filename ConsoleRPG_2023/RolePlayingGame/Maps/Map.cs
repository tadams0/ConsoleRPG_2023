﻿using ConsoleRPG_2023.Dependencies;
using ConsoleRPG_2023.RolePlayingGame.Collections;
using ConsoleRPG_2023.RolePlayingGame.Dungeons;
using ConsoleRPG_2023.RolePlayingGame.Noise;
using ConsoleRPG_2023.RolePlayingGame.Structs;
using csDelaunay;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ConsoleRPG_2023.RolePlayingGame.Maps
{
    public class Map
    {
        //Maps have 3 spaces:
        //World space
        //Chunk Space
        //Tile Space

        //World space is relative to all tiles in the whole map. So 0,0 is the first tile. 0,1 is the second tile. 0,2 is the third tile. And so on.
        //Chunk space is relative to the chunks position in the world. So 0,0 is the first chunk. 0,1 is the second chunk. 0,2 is the third chunk. And so on.
        //Tile space is relative to a single chunk. So 0,0 is the first tile within a single chunk. 0,1 is the second tile in that same chunk. And so on.

        public int ChunkWidth
        {
            get { return chunkWidth; }
        }

        public int ChunkHeight
        {
            get { return chunkHeight; }
        }

        public long MaximumNumberOfChunks
        {
            get { return maximumNumberOfChunks; }
        }

        protected int chunkWidth = 16;
        protected int chunkHeight = 16;

        /// <summary>
        /// The total maximum number of chunks, in one direction from 0,0.
        /// <br/> This is by default int.MaxValue * 32 or 1,099,511,627,264 chunks.
        /// </summary>
        protected long maximumNumberOfChunks = int.MaxValue * 32L;

        protected Dictionary<long, MapChunk> mapChunkMapping = new Dictionary<long, MapChunk>();

        /// <summary>
        /// A mapping of all submaps within the map.
        /// </summary>
        protected Dictionary<long, Map> subMaps = new Dictionary<long, Map>();


        public Map()
        {
        }

        /// <summary>
        /// Generates a chunk id that would coorespond to the given x and y position within the chunk-relative space. 
        /// <br/>
        /// <br/>E.g. 
        /// <br/>0,0 = chunk 1. 
        /// <br/>0,1 = chunk 2. 
        /// <br/>etc..
        /// </summary>
        /// <param name="x">The x coordinate within chunk space.</param>
        /// <param name="y">The y coordinate within chunk space.</param>
        /// <returns></returns>
        public long GetChunkIdInChunkSpace(int x, int y)
        {
            return (long)x * maximumNumberOfChunks + y;
        }

        /// <summary>
        /// Gets the local position of the chunk that is at the given world x and y coordinates.
        /// <br/>Local position of the chunks starts at 0,0. So the first chunk will be 0,0. The next at 0,1. 0,2 and so on.
        /// </summary>
        public Point GetLocalChunkSpaceFromWorldSpace(long x, long y)
        {
            if (x < 0)
            {
                x -= chunkWidth;
            }
            if (y < 0)
            {
                y -= chunkHeight;
            }

            int localChunkX = (int)(x / chunkWidth);
            int localChunkY = (int)(y / chunkHeight);

            return new Point(localChunkX, localChunkY);
        }

        /// <summary>
        /// Gets the id of the chunk at the given x and y world space.
        /// </summary>
        public long GetChunkIdFromWorldSpace(long x, long y)
        {
            if (x < 0)
            {
                x -= chunkWidth;
            }
            if (y < 0)
            {
                y -= chunkHeight;
            }

            int localChunkX = (int)(x / chunkWidth);
            int localChunkY = (int)(y / chunkHeight);

            return GetChunkIdInChunkSpace(localChunkX, localChunkY);
        }

        /// <summary>
        /// Adds the given chunk to the map based on the relative x and y coordinates where 0,0 is the first chunk. 0,1 is the second. 0,2 is the third and so long.
        /// <br/>The x and y position of the chunk must be in chunk space.
        /// </summary>
        protected virtual void AddChunkInChunkSpace(MapChunk chunk)
        {
            long chunkId = GetChunkIdInChunkSpace(chunk.X, chunk.Y);

            mapChunkMapping[chunkId] = chunk;
        }

        /// <summary>
        /// Gets the chunk at the given <b>world</b> space. Which is dependent on the chunk width/height.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns>The chunk if it exists within the map. Otherwise null.</returns>
        public virtual MapChunk GetChunkAtWorldSpace(long x, long y)
        {
            long chunkId = GetChunkIdFromWorldSpace(x, y);

            bool chunkExists = mapChunkMapping.TryGetValue(chunkId,out var chunk);

            if (!chunkExists)
            {
                return null;
            }

            return chunk;
        }

        /// <summary>
        /// Gets the given tile at the given x and y world space coordinates.
        /// </summary>
        /// <param name="x">The position along the x axis (horizontal).</param>
        /// <param name="y">The position along the y axis (vertical).</param>
        /// <returns>The tile if it exists within the chunk at the given location. Otherwise null.</returns>
        public virtual Tile GetTileAtWorldSpace(long x, long y)
        {
            MapChunk chunk = GetChunkAtWorldSpace(x, y);
            if (chunk != null)
            {
                return chunk.GetTileAtWorldCoordinates(x, y);
            }
            return null;
        }

        /// <summary>
        /// Moves the given object by x on the x-axis, and y on the y-axis.
        /// </summary>
        /// <param name="obj">The object to move.</param>
        /// <param name="x">The number of tiles to move horizontally.</param>
        /// <param name="y">The number of tiles to move vertically.</param>
        /// <returns>Returns true if the object was moved. Returns false otherwise.</returns>
        public bool MoveObject(MapObject obj, long x, long y)
        {
            return SetObjectPosition(obj, obj.X + x, obj.Y + y);
        }

        /// <summary>
        /// Sets the given map object to the given x and y world coordinates.
        /// </summary>
        /// <param name="obj">The map object to move.</param>
        /// <param name="x">World x-axis coordinate.</param>
        /// <param name="y">World y-axis coordinate.</param>
        /// <returns>Returns true if the object was moved. Returns false otherwise.</returns>
        /// <exception cref="Exception">Exception can occur is the map object's x and y does not exist within a chunk.</exception>
        public bool SetObjectPosition(MapObject obj, long x, long y)
        {
            MapChunk previousChunk = GetChunkAtWorldSpace(obj.X, obj.Y);
            if (previousChunk == null)
            {
                throw new Exception($"Cannot move object not currently in a chunk. {obj.ToString()} x: {obj.X} y: {obj.Y}");
            }

            MapChunk newChunk = GetChunkAtWorldSpace(x, y);
            if (newChunk == null)
            {//Cannot move into a null chunk.
                return false;
            }

            if (previousChunk != newChunk)
            {//Remove from previous chunk, add to new chunk.
                previousChunk.RemoveMapObject(obj);
                obj.X = x;
                obj.Y = y;
                newChunk.AddMapObject(obj);
            }
            else
            {
                //Add / remove the map object from the chunk so it can regenerate the corresponding keys.
                previousChunk.RemoveMapObject(obj);
                obj.X = x;
                obj.Y = y;
                previousChunk.AddMapObject(obj);
            }

            return true;
        }

        /// <summary>
        /// Adds the given dungeon to the given x and y world coordinates in the map. 
        /// <br/> This does not create <see cref="MapDungeonObj"/> instances or entrances to the given dungeon.
        /// </summary>
        /// <param name="newDungeon">The dungeon to add to the map.</param>
        /// <param name="x">The world x coordinate.</param>
        /// <param name="y">The world y coordinate.</param>
        /// <returns>The id of the dungeon.</returns>
        public long AddDungeon(DungeonMap newDungeon, long x, long y)
        {
            long key = y * maximumNumberOfChunks + x;
            subMaps[key] = newDungeon;
            newDungeon.Id = key;
            return key;
        }


        /// <summary>
        /// Retrieves a submap with the given id.
        /// </summary>
        public Map GetSubMap(long id)
        {
            return subMaps[id];
        }

        /// <summary>
        /// Retrieves the dungeon with the given id.
        /// </summary>
        public DungeonMap GetDungeon(long id)
        {
            return (DungeonMap)subMaps[id];
        }

        /// <summary>
        /// Adds a <see cref="MapObject"/> to the map based on its x and y position.
        /// </summary>
        public void AddObject(MapObject obj)
        {
            MapChunk chunk = GetChunkAtWorldSpace(obj.X, obj.Y);
            if (chunk != null)
            {
                chunk.AddMapObject(obj.X, obj.Y, obj);
            }
        }

        /// <summary>
        /// Adds a <see cref="MapObject"/> to the given world x and y position.
        /// </summary>
        public void AddObject(MapObject obj, long x, long y)
        {
            MapChunk chunk = GetChunkAtWorldSpace(x, y);
            if (chunk != null)
            {
                chunk.AddMapObject(x,y, obj);
            }
        }

        public void RemoveObject(MapObject obj)
        {
            MapChunk chunk = GetChunkAtWorldSpace(obj.X, obj.Y);
            if (chunk != null)
            {
                chunk.RemoveMapObject(obj);
            }
        }

    }
}
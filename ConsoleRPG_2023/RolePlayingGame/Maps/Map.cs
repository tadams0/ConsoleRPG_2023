using ConsoleRPG_2023.Dependencies;
using ConsoleRPG_2023.RolePlayingGame.Collections;
using ConsoleRPG_2023.RolePlayingGame.Dungeons;
using ConsoleRPG_2023.RolePlayingGame.Effects;
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


        /// <summary>
        /// Gets or sets the id of the <see cref="Map"/> instance.
        /// </summary>
        public long Id { get; set; }
        
        /// <summary>
        /// The name of the map.
        /// </summary>
        public string Name { get; set; } = "[Unnamed Map]";

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

        protected long seed = 1;

        /// <summary>
        /// The total maximum number of chunks, in one direction from 0,0.
        /// <br/> This is by default int.MaxValue * 32 or 1,099,511,627,264 chunks.
        /// </summary>
        //protected long maximumNumberOfChunks = int.MaxValue * 32L;
        protected long maximumNumberOfChunks = 200;

        protected Dictionary<long, MapChunk> mapChunkMapping = new Dictionary<long, MapChunk>();

        /// <summary>
        /// A mapping of all submaps within the map.
        /// </summary>
        protected Dictionary<long, Map> subMaps = new Dictionary<long, Map>();

        private HashSet<MapObject> trackedObjects = new HashSet<MapObject>();

        public Map(long id)
        {
            this.seed = new Random().Next(0, int.MaxValue);
            Id = id;
        }

        public Map(long id, long seed)
        {
            this.seed = seed;
            Id = id;
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
            //return (long)x * maximumNumberOfChunks + y;
            return (long)y * maximumNumberOfChunks + x;
        }

        /// <summary>
        /// Converts the x and y world space coordinates to relative 0-chunkWidth, 0-chunkHeight chunk space coordinates.
        /// </summary>
        public Point GetLocalChunkSpaceFromWorldSpace(long x, long y)
        {
            int localChunkX = (int)Math.Floor((decimal)x / chunkWidth);
            int localChunkY = (int)Math.Floor((decimal)y / chunkHeight);

            return new Point(localChunkX + (int)(x % chunkWidth), localChunkY + (int)(y % chunkHeight));
        }

        /// <summary>
        /// Gets the local position of the chunk that is at the given world x and y coordinates.
        /// <br/>Local position of the chunks starts at 0,0. So the first chunk will be 0,0. The next at 0,1. 0,2 and so on.
        /// </summary>
        public Point GetLocalChunkXYFromWorldSpace(long x, long y)
        {
            int localChunkX = (int)Math.Floor((decimal)x / chunkWidth);
            int localChunkY = (int)Math.Floor((decimal)y / chunkHeight);

            return new Point(localChunkX, localChunkY);
        }

        /// <summary>
        /// Gets the id of the chunk at the given x and y world space.
        /// </summary>
        public long GetChunkIdFromWorldSpace(long x, long y)
        {
            int localChunkX = (int)Math.Floor((decimal)x / chunkWidth);
            int localChunkY = (int)Math.Floor((decimal)y / chunkHeight);

            return GetChunkIdInChunkSpace(localChunkX, localChunkY);
        }

        /// <summary>
        /// Gets the world position from the given local chunk XY and local position with that chunk.
        /// </summary>
        /// <param name="chunkX">The horizontal chunk on the x-axis. 0 is the first chunk, 1 is the second chunk, 2 is the third and so on.</param>
        /// <param name="chunkY">The vertical chunk on the y-axis. 0 is the first chunk, 1 is the second chunk, 2 is the third and so on.</param>
        /// <param name="localX">The local position within the specified chunk on the x axis.</param>
        /// <param name="localY">The local position within the specified chunk on the y axis.</param>
        /// <returns>The world position within the map.</returns>
        public PointL GetWorldPositionFromChunkPosition(long chunkX, long chunkY, int localX, int localY)
        {
            long worldX = chunkX * chunkWidth + localX;
            long worldY = chunkY * chunkHeight + localY;

            return new PointL(worldX, worldY);
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
        /// Gets the chunk at the given <b>chunk/local</b> space. Which is independent of the chunk width/height.
        /// </summary>
        /// <param name="x">The chunk position on the x-axis. Where 0,0 denotes the first chunk. 1,0 the second chunk and so on.</param>
        /// <param name="y">The chunk position on the y-axis. Where 0,0 denotes the first chunk. 0,1 the second chunk and so on.</param>
        /// <returns>The chunk if it exists within the map. Otherwise null.</returns>
        public virtual MapChunk GetChunkInChunkSpace(int x, int y)
        {
            long chunkId = GetChunkIdInChunkSpace(x, y);

            bool chunkExists = mapChunkMapping.TryGetValue(chunkId, out var chunk);

            if (!chunkExists)
            {
                return null;
            }

            return chunk;
        }

        /// <summary>
        /// Gets the chunk at the given <b>world</b> space. Which is dependent on the chunk width/height.
        /// </summary>
        /// <param name="x">The world position on the x-axis.</param>
        /// <param name="y">The world position on the y-axis.</param>
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
        /// Gets map objects at the given x and y world position.
        /// </summary>
        /// <param name="x">The horizontal x-axis position.</param>
        /// <param name="y">The vertical y-axis position.</param>
        /// <returns>A newly generated list containing the references of any <see cref="MapObject"/> instance found at the given location.</returns>
        public List<MapObject> GetObjects(long x, long y)
        {
            MapChunk chunk = GetChunkAtWorldSpace(x, y);
            return chunk.GetAllObjectsAtWorldCoordinates(x, y);
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
        /// <br/> This does not create <see cref="MapEntranceObj"/> instances or entrances to the given dungeon.
        /// </summary>
        /// <param name="subMap">The map to add to the map.</param>
        /// <param name="x">The world x coordinate.</param>
        /// <param name="y">The world y coordinate.</param>
        /// <returns>The id of the dungeon.</returns>
        public long AddSubMap(Map subMap, long x, long y)
        {
            long key = y * maximumNumberOfChunks + x;
            subMaps[key] = subMap;
            subMap.Id = key;
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
        /// Adds a <see cref="MapObject"/> to the map based on its x and y position.
        /// </summary>
        public void AddObject(MapObject obj)
        {
            MapChunk chunk = GetChunkAtWorldSpace(obj.X, obj.Y);
            if (chunk != null)
            {
                chunk.AddMapObject(obj.X, obj.Y, obj);
            }
            if (obj.ActiveEffects.Count > 0)
            {
                AddTrackedObject(obj);
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
            if (obj.ActiveEffects.Count > 0)
            {
                AddTrackedObject(obj);
            }
        }

        public void RemoveObject(MapObject obj)
        {
            MapChunk chunk = GetChunkAtWorldSpace(obj.X, obj.Y);
            if (chunk != null)
            {
                chunk.RemoveMapObject(obj);
            }
            if (trackedObjects.Contains(obj))
            {
                trackedObjects.Remove(obj);
            }
        }

        /// <summary>
        /// Adds a tracked object which will be updated.
        /// </summary>
        public void AddTrackedObject(MapObject obj)
        {
            trackedObjects.Add(obj);
        }

        /// <summary>
        /// Removes a tracked object.
        /// </summary>
        public void RemoveTrackedObject(MapObject obj)
        { 
            trackedObjects.Remove(obj); 
        }

        /// <summary>
        /// Adds an active effect to the object using it as the trigger.
        /// </summary>
        public void AddActiveEffectToObject(GameState state, MapObject obj, Effect effect)
        {
            ActiveEffect e = new ActiveEffect(effect, obj);
            bool hasEffectAlready = obj.ActiveEffects.Any(x => x.Effect == effect);
            if (!hasEffectAlready || effect.Stackable)
            {
                obj.ActiveEffects.Add(e);
                trackedObjects.Add(obj);

                ApplyEffect(state, obj, e);
            }
        }

        /// <summary>
        /// Applies an <see cref="ActiveEffect"/> on the given <see cref="MapObject"/> instance.
        /// </summary>
        /// <param name="state">The current game state.</param>
        /// <param name="trackedObj">The object to recieve the effect.</param>
        /// <param name="effect">The effect being applied.</param>
        protected void ApplyEffect(GameState state, MapObject trackedObj, ActiveEffect effect)
        {
            if (!effect.TargetsRetrieved || effect.Effect.AlwaysRetarget)
            {
                effect.GetTargets(state, this, new PointL(trackedObj.X, trackedObj.Y));
            }

            //Apply the active effect.
            effect.ApplyEffect(state, this);

            //If the effect is finished, then let's remove it and clean it up.
            if (effect.IsDone())
            {
                effect.OnRemoval(state, this);
                trackedObj.ActiveEffects.Remove(effect);
            }
        }

        /// <summary>
        /// Updates all tracked objects and their effects.
        /// </summary>
        /// <param name="state">The current game state.</param>
        private void UpdateTrackedObjects(GameState state)
        {
            //Manage any tracked object active effects.
            foreach (var trackedObj in trackedObjects)
            {
                if (trackedObj.ActiveEffects.Count > 0)
                {
                    for (int j = trackedObj.ActiveEffects.Count - 1; j >= 0; j--)
                    {
                        ApplyEffect(state, trackedObj, trackedObj.ActiveEffects[j]);
                    }
                }

                if (trackedObj.ActiveEffects.Count <= 0)
                {//clean up tracked objects without active effects.
                    trackedObjects.Remove(trackedObj);
                }
            }
        }

        /// <summary>
        /// Updates the map centered on the given world coordinates within the number of chunks specified.
        /// </summary>
        /// <param name="state">The current game state.</param>
        /// <param name="worldX">The x-axis position to center the update on.</param>
        /// <param name="worldY">The y-axis position to center the update on.</param>
        /// <param name="verticalChunks">The number of chunks above and below the centered position to update.</param>
        /// <param name="horizontalChunks">The number of chunks left and right of the centered position to update.</param>
        public void Update(GameState state, long worldX, long worldY, int verticalChunks, int horizontalChunks)
        {
            Point topLeftChunk = GetLocalChunkXYFromWorldSpace(worldX - horizontalChunks * chunkWidth, worldY - verticalChunks * chunkHeight);
            Point bottomRightChunk = GetLocalChunkXYFromWorldSpace(worldX + horizontalChunks * chunkWidth, worldY + verticalChunks * chunkHeight);

            MapChunk currentChunk;
            if (topLeftChunk == bottomRightChunk)
            {//If there was only a single chunk selected.
                currentChunk = GetChunkInChunkSpace(topLeftChunk.X, topLeftChunk.Y);
                currentChunk.Update(state, this);
            }
            else
            {//If there was a range of chunks selected, then we must loop through each and update them all.
                for (int x = topLeftChunk.X; x <= bottomRightChunk.X; x++)
                {
                    for (int y = topLeftChunk.Y; y <= bottomRightChunk.Y; y++)
                    {
                        currentChunk = GetChunkInChunkSpace(x, y);
                        if (currentChunk != null)
                        {
                            currentChunk.Update(state, this);
                        }
                    }
                }

            }

            UpdateTrackedObjects(state);
        }

    }
}

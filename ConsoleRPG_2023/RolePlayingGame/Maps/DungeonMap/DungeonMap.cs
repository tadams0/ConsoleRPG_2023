using ConsoleRPG_2023.RolePlayingGame.Maps;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleRPG_2023.RolePlayingGame.Dungeons
{
    /// <summary>
    /// Defines a map that acts as an explorable dungeon.
    /// </summary>
    public class DungeonMap : Map
    {

        /// <summary>
        /// Gets or sets the id of the dungeon.
        /// </summary>
        public long Id
        {
            get { return dungeonId; }
            set { dungeonId = value; }
        }

        public string Name
        {
            get { return dungeonName; }
            set { dungeonName = value; }
        }

        public Point Entrance
        {
            get { return dungeonEntrance; }
        }

        private int brushThickness = 5;

        private int maxChunksHorizontal;
        private int maxChunksVertical;

        private long dungeonId;

        private string dungeonName;

        private Point dungeonEntrance;

        private static Random rand = new Random(2);

        public DungeonMap(int maxChunksHorizontal, int maxChunksVertical)
        {
            //chunkWidth = 16;
            //chunkHeight = 16;
            this.maxChunksHorizontal = maxChunksHorizontal;
            this.maxChunksVertical = maxChunksVertical;

            Name = "Test Dungeon";
            Point entrance = new Point(0, 0);
            this.SetEntrance(entrance);

            GenerateMap(maxChunksHorizontal, maxChunksVertical);
        }

        /// <summary>
        /// Sets the location where the player will be set when they enter the dungeon.
        /// </summary>
        /// <param name="entranceLocation">The x and y position relative to within the dungeon.</param>
        public void SetEntrance(Point entranceLocation)
        {
            dungeonEntrance = entranceLocation;
        }

        private void GenerateMap(int maxChunksHorizontal, int maxChunksVertical)
        {
            bool willExceedBounds = false;

            int currentChunkX = 0;
            int currentChunkY = 0;

            int startX = 0;
            int startY = 0;

            bool chunkExists = true;

            while (!willExceedBounds)
            {
                long chunkId = GetChunkIdInChunkSpace(currentChunkX, currentChunkY);
                Point end;
                MapChunk chunk;

                chunkExists = mapChunkMapping.ContainsKey(chunkId);
                if (chunkExists)
                {
                    chunk = mapChunkMapping[chunkId];
                }
                else
                {
                    chunk = new MapChunk(chunkId, currentChunkX, currentChunkY, chunkWidth, chunkHeight);
                    this.AddChunkInChunkSpace(chunk);
                }

                FillChunk(chunk, startX, startY, out end);


                if (end.X < 0)
                { //End on the left
                    currentChunkX--;
                    startX = ChunkWidth - 1;
                    startY = end.Y;

                }
                else if (end.X > chunkWidth - 1)
                {//End on the right
                    currentChunkX++;
                    startX = 0;
                    startY = end.Y;
                }
                else if (end.Y < 0)
                {//End on the top
                    currentChunkY--;
                    startY = ChunkHeight - 1;
                    startX = end.X;
                }
                else if (end.Y > chunkHeight - 1)
                {//End on the bottom
                    currentChunkY++;
                    startY = 0;
                    startX = end.X;
                }

                willExceedBounds = Math.Abs(currentChunkX) + 1 > maxChunksHorizontal
                    || Math.Abs(currentChunkY) + 1 > maxChunksVertical;
            }
        }

        private void FillChunk(MapChunk chunk, int startingX, int startingY, out Point end)
        {
            MapChunk newChunk = chunk;
            int downBias = 27;
            int rightBias = 25;
            int leftBias = 25;
            int upBias = 25;

            bool reachedEnd = false;
            int currentX = startingX;
            int currentY = startingY;

            long worldX;
            long worldY;

            end = Point.Empty;

            while (!reachedEnd)
            {
                byte newTileId;
                Tile room;
                for (int i = 0; i < brushThickness; i++)
                {
                    for (int j = 0; j < brushThickness; j++)
                    {
                        worldX = newChunk.X * chunkWidth + currentX + i;
                        worldY = newChunk.Y * ChunkHeight + currentY + j;

                        MapChunk existingChunk = GetChunkAtWorldSpace(worldX, worldY);

                        if (existingChunk == null)
                        {//If the brush size goes beyond the newChunk,
                            //Then we must generate a new one to place the overflow in.
                            long chunkId = GetChunkIdFromWorldSpace(worldX, worldY);
                            Point chunkXY = GetLocalChunkSpaceFromWorldSpace(worldX, worldY);
                            chunk = new MapChunk(chunkId, chunkXY.X, chunkXY.Y, chunkWidth, chunkHeight);
                            this.AddChunkInChunkSpace(chunk);

                            newTileId = newChunk.GetTileIdFromWorldCoordinates(worldX, worldY);
                            room = GenerateTile(newTileId);

                            chunk.SetTileByWorldCoordinates(worldX, worldY, room);
                        }
                        else if (existingChunk != newChunk)
                        {
                            newTileId = newChunk.GetTileIdFromWorldCoordinates(worldX, worldY);
                            room = GenerateTile(newTileId);

                            existingChunk.SetTileByWorldCoordinates(worldX, worldY, room);
                        }
                        else
                        {
                            newTileId = newChunk.GetTileId(currentX + i, currentY+j);
                            room = GenerateTile(newTileId);

                            newChunk.SetTileRelative(currentX + i, currentY + j, room);
                        }

                    }
                }

                Point directionVector = ChooseDirection(upBias, rightBias, downBias, leftBias);
                currentX += directionVector.X;
                currentY += directionVector.Y;

                reachedEnd = newChunk.IsOutOfBounds(currentX, currentY);

                if (reachedEnd)
                {
                    end = new Point(currentX, currentY);
                }
            }
        }

        public static Point ChooseDirection(double north, double east, double south, double west)
        {
            var directions = new List<KeyValuePair<Point, double>>()
            {
                new KeyValuePair<Point, double>(new Point(0, -1), north),
                new KeyValuePair<Point, double>(new Point(1, 0), east),
                new KeyValuePair<Point, double>(new Point(0, 1), south),
                new KeyValuePair<Point, double>(new Point(-1, 0), west)
            };

            double total = directions.Sum(x => x.Value);
            double randomValue = rand.NextDouble() * total;

            // Shuffle the directions array
            for (int i = 0; i < directions.Count - 1; i++)
            {
                int j = rand.Next(i, directions.Count);
                var temp = directions[i];
                directions[i] = directions[j];
                directions[j] = temp;
            }

            // Choose a direction based on shuffled probabilities
            foreach (var kvp in directions)
            {
                if (randomValue < kvp.Value)
                {
                    return kvp.Key;
                }
                randomValue -= kvp.Value;
            }

            return new Point(0, 0); //this code should not be reached
        }

        public Tile GenerateTile(byte id)
        {
            Tile t = new Tile(id);
            t.TileType = TileType.Stone;
            return t;
        }

        private int Wrap(int min, int max, int x)
        {
            if (x < 0)
            {
                return max + x;
            }
            if (x < min)
            {
                return min + x % (max - min);
            }
            else if (x > max)
            {
                return min + x % (max - min);
            }

            return x;
        }

    }
}

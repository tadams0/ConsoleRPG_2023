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

        private int maxChunksHorizontal;
        private int maxChunksVertical;

        private long dungeonId;

        private string dungeonName;

        private Point dungeonEntrance;

        private static Random rand = new Random(2);

        public DungeonMap(int maxChunksHorizontal, int maxChunksVertical)
        {
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
                }

                FillChunk(chunk, startX, startY, out end);

                if (!chunkExists)
                {
                    this.AddChunkInChunkSpace(chunk);
                }

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
            int downBias = 20;
            int rightBias = 40;
            int leftBias = 40;
            int upBias = 50;

            bool reachedEnd = false;
            int currentX = startingX;
            int currentY = startingY;

            end = Point.Empty;

            while (!reachedEnd)
            {
                byte newTileId = newChunk.GetTileId(currentX, currentY);
                Tile room = GenerateTile(newTileId);

                newChunk.SetTileRelative(currentX, currentY, room);

                int chunkDirection = rand.Next(0, 101);
                if (chunkDirection < downBias)
                {
                    //Create downward Cell

                    currentY += 1;
                }
                else if (chunkDirection < downBias + rightBias)
                {
                    //Create rightward cell

                    currentX += 1;
                }
                else if (chunkDirection < downBias + rightBias + leftBias)
                {
                    //Create leftward cell

                    currentX -= 1;
                }
                else if (chunkDirection < downBias + rightBias + leftBias + upBias)
                {
                    //Create upward cell

                    currentY -= 1;
                }

                reachedEnd = newChunk.IsOutOfBounds(currentX, currentY);

                if (reachedEnd)
                {
                    end = new Point(currentX, currentY);
                }
            }
        }


        private MapChunk GenerateChunk(long id, int chunkWidth, int chunkHeight, int chunkX, int chunkY, int startingX, int startingY, out Point end)
        {
            MapChunk newChunk = new MapChunk(id, chunkX, chunkY, chunkWidth, chunkHeight);
            int downBias = 40;
            int rightBias = 40;
            int leftBias = 40;
            int upBias = 50;

            bool reachedEnd = false;
            int currentX = startingX;
            int currentY = startingY;

            end = Point.Empty;

            while (!reachedEnd)
            {
                byte newTileId = newChunk.GetTileId(currentX, currentY);
                Tile room = GenerateTile(newTileId);

                newChunk.SetTileRelative(currentX, currentY, room);

                int chunkDirection = rand.Next(0, 101);
                if (chunkDirection < downBias)
                {
                    //Create downward Cell

                    currentY += 1;
                }
                else if (chunkDirection < downBias + rightBias)
                {
                    //Create rightward cell

                    currentX += 1;
                }
                else if (chunkDirection < downBias + rightBias + leftBias)
                {
                    //Create leftward cell

                    currentX -= 1;
                }
                else if (chunkDirection < downBias + rightBias + leftBias + upBias)
                {
                    //Create upward cell

                    currentY -= 1;
                }

                reachedEnd = newChunk.IsOutOfBounds(currentX, currentY);

                if (reachedEnd)
                {
                    end = new Point(currentX,currentY);
                }
            }

            return newChunk;
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

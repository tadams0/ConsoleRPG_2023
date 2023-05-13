using ConsoleRPG_2023.RolePlayingGame.Maps;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
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

        private int brushThickness = 3;
        private bool circleBrush = false;

        private int maxBranchChunks = 10;

        private int branchChance = 11;

        private int maxChunksHorizontal;
        private int maxChunksVertical;

        private long dungeonId;

        private string dungeonName = "Unnamed Dungeon";

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

            GenerateMap();
        }

        /// <summary>
        /// Sets the location where the player will be set when they enter the dungeon.
        /// </summary>
        /// <param name="entranceLocation">The x and y position relative to within the dungeon.</param>
        public void SetEntrance(Point entranceLocation)
        {
            dungeonEntrance = entranceLocation;
        }

        private void GenerateMap()
        {
            /*
            int downBias = 40;
            int rightBias = 30;
            int leftBias = 30;
            int upBias = 25;
            */

            int downBias = 90;
            int rightBias = 10;
            int leftBias = 0;
            int upBias = 0;

            GenerateBranch(0, 0, 0, 0, downBias, rightBias, leftBias, upBias, maxChunksHorizontal, maxChunksVertical, true);
        }

        private void GenerateBranch(int currentChunkX, int currentChunkY, int startX, int startY, int downBias, int rightBias, int leftBias, int upBias, int maxChunksHorizontal, int maxChunksVertical, bool allowFurtherBranches)
        {
            int originalChunkX = currentChunkX;
            int originalChunkY = currentChunkY;
            bool willExceedBounds = false;

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
                    chunk = new MapChunk(chunkId, currentChunkX, currentChunkY, chunkWidth, chunkHeight, seed);
                    this.AddChunkInChunkSpace(chunk);
                }

                FillChunk(chunk, startX, startY, downBias, upBias, rightBias, leftBias, out end);

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

                willExceedBounds = Math.Abs(currentChunkX - originalChunkX) + 1 > maxChunksHorizontal
                    || Math.Abs(currentChunkY - originalChunkY) + 1 > maxChunksVertical;

                if (!willExceedBounds && allowFurtherBranches)
                {
                    //Check if we should make a new branch
                    if (rand.Next(1, 101) < branchChance)
                    {
                        //Setup new dungeon branch biases and generation variables.
                        int leftRightChance = rand.Next(1, 101);
                        int upDownChance = rand.Next(1, 101);

                        //Swap main branch biases.
                        int branchLeftBias = upBias;
                        int branchRightBias = downBias;
                        if (upDownChance > 50)
                        {
                            branchLeftBias = downBias;
                            branchRightBias = upBias;
                        }

                        int branchUpBias = leftBias;
                        int branchDownBias= rightBias;
                        if (leftRightChance > 50)
                        {
                            branchUpBias = rightBias;
                            branchDownBias = leftBias;
                        }

                        //Recursion
                        this.GenerateBranch(currentChunkX, currentChunkY, startX,startY, branchDownBias, branchRightBias, branchLeftBias, branchUpBias, maxBranchChunks, maxBranchChunks, false);
                    }
                }
            }
        }

        private void FillChunk(MapChunk chunk, int startingX, int startingY, int downBias, int upBias, int rightBias, int leftBias, out Point end)
        {
            MapChunk newChunk = chunk;

            bool reachedEnd = false;
            int currentX = startingX;
            int currentY = startingY;

            long worldX;
            long worldY;

            float halfBrushThicknessFloat = brushThickness / 2f;
            int halfBrushThickness = brushThickness / 2;
            int otherHalfBrushThickness = brushThickness - halfBrushThickness;

            end = Point.Empty;

            Tile room;
            while (!reachedEnd)
            {
                for (int i = -halfBrushThickness; i < otherHalfBrushThickness; i++)
                {
                    for (int j = -halfBrushThickness; j < otherHalfBrushThickness; j++)
                    {
                        if (circleBrush)
                        {
                            Vector2 currentXY = new Vector2(currentX + i, currentY + j);
                            Vector2 center = new Vector2(currentX + halfBrushThicknessFloat, currentY + halfBrushThicknessFloat);
                            float distance = Vector2.Distance(currentXY, center);
                            if (distance - 0.1f > halfBrushThicknessFloat)
                            {
                                continue;
                            }
                        }
                        worldX = newChunk.X * chunkWidth + currentX + i;
                        worldY = newChunk.Y * ChunkHeight + currentY + j;

                        MapChunk existingChunk = GetChunkAtWorldSpace(worldX, worldY);

                        if (existingChunk == null)
                        {//If the brush size goes beyond the newChunk,
                            //Then we must generate a new one to place the overflow in.
                            long chunkId = GetChunkIdFromWorldSpace(worldX, worldY);
                            Point chunkXY = GetLocalChunkXYFromWorldSpace(worldX, worldY);
                            existingChunk = new MapChunk(chunkId, chunkXY.X, chunkXY.Y, chunkWidth, chunkHeight, seed);
                            this.AddChunkInChunkSpace(existingChunk);
                        }

                        room = GenerateTile();

                        var currentTile = existingChunk.GetTileAtWorldCoordinates(worldX, worldY);
                        
                        if (i == 0 && j == 0)
                        {
                            room.TileType = TileType.HellRock;
                            existingChunk.SetTileByWorldCoordinates(worldX, worldY, room);
                        }
                        else if (currentTile.TileType != TileType.HellRock)
                        {
                            existingChunk.SetTileByWorldCoordinates(worldX, worldY, room);
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

        private Tile GenerateTile()
        {
            Tile t = new Tile();
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

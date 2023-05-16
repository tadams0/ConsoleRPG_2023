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
        /// The starting point of the dungeon in world coordinates.
        /// </summary>
        public PointL StartPoint
        {
            get { return dungeonStart; }
        }

        private int brushThickness = 3;
        private bool circleBrush = false;

        private int maxBranchChunks = 10;

        private int branchChance = 11;

        private int maxChunksHorizontal;
        private int maxChunksVertical;

        private PointL dungeonStart = PointL.Empty;
        private PointL dungeonEnd = PointL.Empty;

        private static Random rand = new Random(2);

        public DungeonMap(int maxChunksHorizontal, int maxChunksVertical, long startX, long startY)
            : base(0)
        {
            //chunkWidth = 16;
            //chunkHeight = 16;
            this.maxChunksHorizontal = maxChunksHorizontal;
            this.maxChunksVertical = maxChunksVertical;

            Name = "Test Dungeon";
            this.SetEntrancePoint(startX, startY);

            GenerateMap();
        }

        /// <summary>
        /// Sets the location where the player will be set when they enter the dungeon.
        /// </summary>
        /// <param name="entranceLocation">The x and y position relative to within the dungeon.</param>
        private void SetEntrancePoint(long startX, long startY)
        {
            dungeonStart = new PointL(startX, startY);
        }

        /// <summary>
        /// Creates generic map return objects at the entrance and end points of the dungeon.
        /// </summary>
        public void CreateEntranceExitReturns(string returningMapName)
        {
            MapReturnObj entrance = new MapReturnObj();
            entrance.X = dungeonStart.X;
            entrance.Y = dungeonStart.Y;
            entrance.ReturningMapName = $"Exit to {returningMapName}";

            MapReturnObj exit = new MapReturnObj();
            exit.X = dungeonEnd.X;
            exit.Y = dungeonEnd.Y;
            exit.ReturningMapName = $"Exit to {returningMapName}";

            this.AddObject(entrance);
            this.AddObject(exit);
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

            //Transforming the provided world space coordinates to a valid local chunk space starting position.
            Point chunkXY = GetLocalChunkXYFromWorldSpace(dungeonStart.X, dungeonStart.Y);
            Point chunkSpace = GetLocalChunkSpaceFromWorldSpace(dungeonStart.X, dungeonStart.Y);

            //Generating the actual dungeon map.
            PointL endPoint = GenerateBranch(chunkXY.X, chunkXY.Y, chunkSpace.X, chunkSpace.Y, downBias, rightBias, leftBias, upBias, maxChunksHorizontal, maxChunksVertical, branchChance);
            dungeonEnd = endPoint;
        }

        /// <summary>
        /// Generates a branch with the given parameters.
        /// </summary>
        /// <param name="currentChunkX">The horizontal chunk to start the branch on.</param>
        /// <param name="currentChunkY">The vertical chunk to start the branch on.</param>
        /// <param name="startX">The local position on the x-axis within the specified chunk to start the branch at.</param>
        /// <param name="startY">The local position on the y-axis within the specified chunk to start the branch at.</param>
        /// <param name="downBias">The percent chance of a downward position being picked every movement.</param>
        /// <param name="rightBias">The percent chance of a rightward position being picked every movement.</param>
        /// <param name="leftBias">The percent chance of a leftward position being picked every movement.</param>
        /// <param name="upBias">The percent chance of an upward position being picked every movement.</param>
        /// <param name="maxChunksHorizontal">The maximum number horizontal chunks until the branch is forcefully ended</param>
        /// <param name="maxChunksVertical">The maximum number vertical chunks until the branch is forcefully ended.</param>
        /// <param name="branchChance">The chance of additional branches coming off the main generated one.</param>
        /// <returns>The world position that the branch ends on.</returns>
        private PointL GenerateBranch(int currentChunkX, int currentChunkY, int startX, int startY, int downBias, int rightBias, int leftBias, int upBias, int maxChunksHorizontal, int maxChunksVertical, int branchChance)
        {
            int originalChunkX = currentChunkX;
            int originalChunkY = currentChunkY;
            bool willExceedBounds = false;

            bool chunkExists = true;
            Point previousEnd = Point.Empty;
            int previousChunkX = 0;
            int previousChunkY = 0;
            Point end = Point.Empty;

            while (!willExceedBounds)
            {
                previousEnd = end;
                previousChunkX = currentChunkX;
                previousChunkY = currentChunkY;

                long chunkId = GetChunkIdInChunkSpace(currentChunkX, currentChunkY);
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

                if (!willExceedBounds && branchChance > 0)
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

                        //Recursion to generate off-shoot branches.
                        //These cannot spawn additional branches by default as that could result in longer and uncontrolled maps.
                        this.GenerateBranch(currentChunkX, currentChunkY, startX,startY, branchDownBias, branchRightBias, branchLeftBias, branchUpBias, maxBranchChunks, maxBranchChunks, 0);
                    }
                }
            }

            //We use the previous end to generate the last world position, because "end" should represent the out of bounds end that caused the branch to stop generating.
            return GetWorldPositionFromChunkPosition(previousChunkX, previousChunkY, previousEnd.X, previousEnd.Y);
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

        private static Point ChooseDirection(double north, double east, double south, double west)
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

    }
}

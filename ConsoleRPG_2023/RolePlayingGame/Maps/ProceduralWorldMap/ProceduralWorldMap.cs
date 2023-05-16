using ConsoleRPG_2023.Dependencies;
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
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ConsoleRPG_2023.RolePlayingGame.Maps
{
    public class ProceduralWorldMap : Map
    {
        public double MaxMoisture
        {
            get { return maxMoisture; }
            set { maxMoisture = value; }
        }
        public double MinMoisture
        {
            get { return minMoisture; }
            set { minMoisture = value; }
        }

        public double MaxTemperature
        {
            get { return maxTemperature; }
            set { maxTemperature = value; }
        }
        public double MinTemperature
        {
            get { return minTemperature; }
            set { minTemperature = value; }
        }

        public double MaxHeight
        {
            get { return maxHeight; }
            set { maxHeight = value; }
        }

        public double MinHeight
        {
            get { return minHeight; }
            set { minHeight = value; }
        }

        /// <summary>
        /// The hierarchy should look something like 1 influence rect per 1 region. Any number of biome points per region. Each region has a voronoi diagram mapping its points.
        /// </summary>
        private LimitedSpatialHashmapRevised<BiomeRegionData> biomeRegions;

        /// <summary>
        /// The overarching biome region is a region which encompasses a full scan of regions. So a 3x3 scan would result in overarching biome regions the size of 3x3 influence rectangles.
        /// This get computationally expensive as they generate their own voronoi region to help between cross-biome regions. Overarching biome regions have a lot of overlap with each other.
        /// <br/>Essentially you can think of a single overarching biome region of a union of the 3x3 biome regions or the biome region of a total influence rectangle including 3x3 influence rects.
        /// </summary>
        private Dictionary<PointL, BiomeRegionData> overarchingBiomeRegions;


        /// <summary>
        /// The biomeInfluenceRectWidth and biomeInfluenceRectHeight determine the size of the rectangle used to 
        /// generate biome influence points. 
        /// <br/>The sizes are in multiples of the chunk size.
        /// <br/>The scan width/height uses the resulting influence rectangle as the scan size.
        /// <br/>Sizes less than 1 should increase the scan width/height to more than 3 to prevent dividing lines
        /// in noise generation.
        /// <br/>Most importantly, smaller values = more dense noise generation and higher CPU usage in map
        /// generation. Larger values = larger noise generation and reduced CPU usage in generation.
        /// </summary>
        private double biomeInfluenceRectWidth = 4;
        private double biomeInfluenceRectHeight = 4;

        private int biomesPerInfluenceRect = 1;

        private float blendThreshold = 90f; //threshold roughly in number of tiles when small < 6 while it exponentially gets larger.

        private double maxMoisture = 100;
        private double minMoisture = 0;
        private double moistureRange;

        private double maxTemperature = 410;
        private double minTemperature = 160;
        private double temperatureRange;

        private double maxHeight = 1000;
        private double minHeight = 0;
        private double heightRange;

        private double maxFertility = 125;
        private double minFertility = 0;
        private double fertilityRange;

        /// <summary>
        /// Determines whether chunks will be generated and added to the map if they don't yet exist.
        /// </summary>
        private bool generateChunksThatDontExist = true;

        private double biomeScale = 0.001;

        /// <summary>
        /// Chance of a biome copying the characteristics of its nearby biome exactly. This takes priority over influences.
        /// </summary>
        private double biomeAdoptionChance = 25;

        /// <summary>
        /// Chances of a biome taking on relative stats of its neighbor.
        /// </summary>
        private double biomeInfluenceChance = 80;

        /// <summary>
        /// The percent amount of the temperature/height/moisture/etc that varies when a biome influence occurs.
        /// </summary>
        private double biomeInfluencePercentMagnitude = 15;

        private BiomeCacher biomeCacher;

        #region Worley Noise Generation Variables
        /// <summary>
        /// The scan width/height are the number of influence rectangles to go through on each axis.
        /// <br/>So a scanWidth and scanHeight where both are 3 results in a 3x3 grid of influence points being picked up, or a total of 9 influence rectangles (1 center and 8 surrounding).
        /// <br/>A scan width and scanHeight where both are 4 results in a 4x4 grid, and so on.
        /// <br/>3 should be the minimum for things to function correctly.
        /// </summary>
        private int scanWidth = 3;
        private int scanHeight = 3;
        private int halfScanWidth;
        private int halfScanHeight;
        private long influenceRectWidth;
        private long influenceRectHeight;

        //Blend factor and magnitude effect the worley/vornoi noise generation when averaging the weights of nearby voronoi sites.
        private float blendFactor = 0;
        private float magnitude = 7;
        #endregion

        public ProceduralWorldMap(long id, long seed)
            : base(id, seed)
        {
            moistureRange = maxMoisture - minMoisture;
            temperatureRange = maxTemperature - minTemperature;
            heightRange = maxHeight - minHeight;
            fertilityRange = maxFertility - minFertility;

            halfScanWidth = (int)(scanWidth / 2d - 0.5);
            halfScanHeight = (int)(scanHeight / 2d - 0.5);
            influenceRectWidth = (long)(biomeInfluenceRectWidth * chunkWidth);
            influenceRectHeight = (long)(biomeInfluenceRectHeight * chunkHeight);
            long maximumTileWidth = maximumNumberOfChunks * chunkWidth;
            biomeRegions = new LimitedSpatialHashmapRevised<BiomeRegionData>(influenceRectWidth, influenceRectHeight, maximumTileWidth);
            //overarchingBiomeRegions = new LimitedSpatialHashmapRevised<BiomeRegionData>(influenceRectWidth * scanWidth, influenceRectHeight * scanHeight, maximumTileWidth);
            overarchingBiomeRegions = new Dictionary<PointL, BiomeRegionData>();

            //Technically the total number of voronoi regions would be something like ([maximum number of chunks] * [chunk width]) / [influence rectangle width]
            //But the spatial hashmap only uses the "maximum" as a way to generate hash keys. It doesn't need to be equal and can be higher than the max.
            //voronoiRegions = new LimitedSpatialHashmap<Voronoi>(influenceRectWidth, influenceRectHeight, maximumNumberOfChunks * influenceRectWidth);

            biomeCacher = new BiomeCacher();
            biomeCacher.Build();
        }


        private MapChunk GenerateOrGetChunk(int x, int y)
        {
            long chunkId = GetChunkIdInChunkSpace(x, y);

            if (mapChunkMapping.TryGetValue(chunkId, out var chunk))
            {
                return chunk;
            }

            chunk = GenerateChunk(x, y);

            mapChunkMapping[chunkId] = chunk;
            return chunk;
        }

        private MapChunk GenerateChunk(long worldX, long worldY)
        {
            Point localChunkXY = GetLocalChunkXYFromWorldSpace(worldX, worldY);
            long chunkId = GetChunkIdInChunkSpace(localChunkXY.X, localChunkXY.Y);
            double noiseX;
            double noiseY;
            long longNoiseX;
            long longNoiseY;
            long worldChunkX = localChunkXY.X * chunkWidth;
            long worldChunkY = localChunkXY.Y * chunkHeight;

            //Generate a new chunk.
            MapChunk chunk = new MapChunk(chunkId, localChunkXY.X, localChunkXY.Y, chunkWidth, chunkHeight, seed);

            if (localChunkXY.X == -1)
            {
                int test = 0;
            }
            //TEST ONLY
            //GRID
            /*
             
            for (int i = 0; i < chunkWidth; i++)
            {
                for (int j = 0; j < chunkHeight; j++)
                {
                    Tile t = new Tile();
                    if ((i + j) % 2 == 0)
                    {
                        t.TileType = TileType.Corrupted;
                    }
                    else
                    {
                        t.TileType = TileType.GrassMild;
                    }
                    chunk.SetTileRelative(i, j, t);
                }
            }

            return chunk;
            */
            
            // slash (/)
            /*
            if (localChunkXY.X < 0 && localChunkXY.Y < 0)
            {
                int TEST = 0;
            }
            else
            {
                int TEST = 0;
            }
            for (int j = 0; j < chunkHeight; j++)
            {
                for (int i = 0; i < chunkWidth; i++)
                {
                    Tile t = new Tile();
                    if ((i + j) % 4 == 0)
                    {
                        t.TileType = TileType.Corrupted;
                    }
                    else
                    {
                        t.TileType = TileType.GrassMild;
                    }
                    chunk.SetTileRelative(i, j, t);
                    Tile tileTest = chunk.GetTileRelativeCoordinates(i, j);
                    Tile worldTileTest = chunk.GetTileAtWorldCoordinates(worldChunkX +i, worldChunkY +j);
                    if (t != tileTest || t != worldTileTest)
                    {
                        int TESTONLY = 0;
                    }
                }
            }
            
            return chunk;
            */
            //END TEST

            //Before filling the chunk we data, we generate surrounding biome points so the worley noise can
            //generate and lerp between biomes.
            BiomeRegionData influencingBiomeRegion = GetBiomeRegionDataByChunk(chunk, true);

            Tile currentTile;
            BiomeData fullTileData;
            BiomeData localBiomeRegion;
            Biome blendBiome;
            BiomeType currentBiomeType;
            Biome currentBiome = biomeCacher.GetBiome(BiomeType.None);
            Vector2f normalizedPoint;
            for (int j = 0; j < chunkHeight; j++) //vertical
            {
                for (int i = 0; i < chunkWidth; i++) //horizontal
                {
                    currentTile = chunk.GetTileRelativeCoordinates(i, j);

                    noiseX = worldChunkX + i;
                    noiseY = worldChunkY + j;
                    longNoiseX = worldChunkX + i;
                    longNoiseY = worldChunkY + j;

                    //float noiseValue = OpenSimplex2S.Noise2_ImproveX(seed, noiseX * mapScale, noiseY * mapScale);
                    double noiseValue = 1;

                    //double noiseValue = WorleyNoise.GetNoise(longNoiseX, longNoiseY, points, 0);
                    float biomeValue = OpenSimplex2S.Noise2_ImproveX(seed, noiseX * biomeScale, noiseY * biomeScale);

                    //normalize x and y
                    normalizedPoint = WorleyNoise.NormalizePointLToF(longNoiseX, longNoiseY, influencingBiomeRegion.Region);

                    WorleyNoiseBiomeReturn[] weights = WorleyNoise.GetNoiseWeights(longNoiseX, longNoiseY, influencingBiomeRegion, blendFactor, magnitude);
                    fullTileData = GetBiomeDataByWorldCoordinates(longNoiseX, longNoiseY, weights);

                    var nearestSite = influencingBiomeRegion.Voronoi.FindClosestSite(normalizedPoint);

                    float nearestEdgeDistance;
                    var nearestEdge = nearestSite.GetClosestEdge(normalizedPoint, out nearestEdgeDistance);

                    float nonRelativeNearestEdgeDist = nearestEdgeDistance * (float)(influencingBiomeRegion.Region.Width * influencingBiomeRegion.Region.Height);

                    blendBiome = null;
                    TileType expectedTileType = TileType.None;
                    if (nonRelativeNearestEdgeDist < blendThreshold + chunk.Random.Next(5,15))
                    { //Chance to blend neighboring biome

                        bool isLeft = nearestSite.isLeft(normalizedPoint, nearestEdge);

                        double randValue = influencingBiomeRegion.Random.NextDouble();
                        bool blend = randValue > .1 + (nonRelativeNearestEdgeDist / blendThreshold) * 0.95;

                        if (blend)
                        {
                            BiomeType biomeType;
                            Biome leftBiome;
                            Biome rightBiome;
                            var leftSite = nearestEdge.Site(LR.RIGHT);
                            var rightSite = nearestEdge.Site(LR.LEFT);

                            //Assigning left biome.
                            biomeType = leftSite.Data.GetBiomeType();
                            leftBiome = biomeCacher.GetBiome(biomeType);

                            //Assigning right biome
                            biomeType = rightSite.Data.GetBiomeType();
                            rightBiome = biomeCacher.GetBiome(biomeType);

                            if (isLeft && !blend)
                            {//Site 1
                                expectedTileType = leftBiome.GetTileType(fullTileData, chunk.Random);
                                blendBiome = leftBiome;
                            }
                            else
                            {//Site 2
                                expectedTileType = rightBiome.GetTileType(fullTileData, chunk.Random);
                                blendBiome = rightBiome;
                            }

                            if ((!leftBiome.AllowAnyTileTypeBlending && !leftBiome.CanBlendTileType(expectedTileType))
                                || (!rightBiome.AllowAnyTileTypeBlending && !rightBiome.CanBlendTileType(expectedTileType)))
                            {//If the biome doesn't allow for blending of the selected tile type, then
                             //let's undo the blend.
                                expectedTileType = TileType.None;
                            }
                        }
                        
                    }

                    if (expectedTileType == TileType.None)
                    {//If no blending occured, we will select the tile based off the biome as per usual.

                        localBiomeRegion = GetGeneralBiomeData(longNoiseX, longNoiseY, influencingBiomeRegion);
                        currentBiomeType = localBiomeRegion.GetBiomeType();
                        currentBiome = biomeCacher.GetBiome(currentBiomeType);

                        expectedTileType = currentBiome.GetTileType(fullTileData, chunk.Random);

                        //Populate the objects now
                        var objects = currentBiome.GetSingleTileObjects(fullTileData, chunk.Random, currentTile, longNoiseX, longNoiseY);
                        chunk.AddRangeMapObject(longNoiseX, longNoiseY, objects);
                    }
                    else
                    {
                        //If a blend occured, we want to generate objects based on that blended biome.

                        var objects = blendBiome.GetSingleTileObjects(fullTileData, chunk.Random, currentTile, longNoiseX, longNoiseY);
                        chunk.AddRangeMapObject(longNoiseX, longNoiseY, objects);

                    }

                    currentTile.TileType = expectedTileType;

                }
            }

            return chunk;
        }

        /// <summary>
        /// Retrieves the overarching biome at the given world x and world y coordinates.
        /// </summary>
        private BiomeRegionData GetOverarchingBiome(long worldX, long worldY)
        {
            RectL rect = GetInfluenceRectRegion(worldX, worldY);
            PointL key = new PointL(rect.X, rect.Y);
            return overarchingBiomeRegions[key];
        }

        private PointL GetOverarchingBiomeKey(long worldX, long worldY)
        {
            RectL rect = GetInfluenceRectRegion(worldX, worldY);

            return new PointL(rect.X, rect.Y);
        }

        private RectL GetInfluenceRectRegion(long worldX, long worldY)
        {
            long influenceRectX = (MathL.Floor(worldX, influenceRectWidth)) - (influenceRectWidth * halfScanWidth) + worldX * influenceRectWidth;
            long influenceRectY = (MathL.Floor(worldY, influenceRectHeight)) - (influenceRectHeight * halfScanHeight) + worldY * influenceRectHeight;

            RectL influenceRect = new RectL(influenceRectX, influenceRectY, influenceRectWidth, influenceRectHeight);

            return influenceRect;
        }

        private BiomeRegionData GetBiomeRegionDataByChunk(MapChunk chunk, bool generateNonExisting)
        {
            List<BiomeRegionData> existingRegions;
            BiomeRegionData currentReigon;
            long worldX = (long)chunk.X * chunkWidth;
            long worldY = (long)chunk.Y * chunkHeight;
            PointL overarchingBiomeKey = new PointL(worldX, worldY);
            long influenceRectX;
            long influenceRectY;

            RectL influenceRect;
            RectL totalInfluenceRect = new RectL(worldX, worldY, influenceRectWidth, influenceRectHeight);

            BiomeRegionData overarchingBiome;

            bool hasOverarchingBiome = overarchingBiomeRegions.TryGetValue(overarchingBiomeKey, out overarchingBiome);

            if (!hasOverarchingBiome)
            {
                //Create the overarching biome region:
                //Generate the random number generator seed based off the influence rect starting x and y (upper left)
                influenceRectX = (MathL.Floor(worldX, influenceRectWidth)) - (influenceRectWidth * halfScanWidth);
                influenceRectY = (MathL.Floor(worldY, influenceRectHeight)) - (influenceRectHeight * halfScanHeight);
                int biomeRegionSeed = (int)((influenceRectX + influenceRectY * maximumNumberOfChunks + seed) % int.MaxValue);
                int tempseed = biomeRegionSeed;
                Random overarchingRegionRandom = new Random(biomeRegionSeed);

                List<BiomeRegionData> resultingRegions = new List<BiomeRegionData>();
                List<KeyValuePair<PointL, BiomeData>> totalDataPoints = new List<KeyValuePair<PointL, BiomeData>>(biomesPerInfluenceRect * scanWidth * scanHeight);
                for (int x = 0; x < scanWidth; x++)
                {
                    for (int y = 0; y < scanHeight; y++)
                    {
                        influenceRectX = (MathL.Floor(worldX, influenceRectWidth)) - (influenceRectWidth * halfScanWidth) + x * influenceRectWidth;
                        influenceRectY = (MathL.Floor(worldY, influenceRectHeight)) - (influenceRectHeight * halfScanHeight) + y * influenceRectHeight;

                        influenceRect = new RectL(influenceRectX, influenceRectY, influenceRectWidth, influenceRectHeight);
                        totalInfluenceRect = RectL.Union(influenceRect, totalInfluenceRect);

                        //Check if there are already generated regions within the influence rect.
                        existingRegions = biomeRegions.GetDataInsideGridRect(influenceRect);
                        if (generateNonExisting)
                        {
                            if (existingRegions.Count <= 0)
                            { //Create the new region

                                //Create the local biome region data random
                                biomeRegionSeed = (int)((influenceRectX + influenceRectY * maximumNumberOfChunks + seed) % int.MaxValue);
                                Random rand = new Random(biomeRegionSeed);

                                Dictionary<PointL, BiomeData> dataPoints = new Dictionary<PointL, BiomeData>(biomesPerInfluenceRect);
                                PointL biomePoint;
                                for (int i = 0; i < biomesPerInfluenceRect; i++)
                                {//generate a completely random biome.
                                    BiomeData newData = GenerateRandomBiomeData(rand);
                                    double adoptResult = rand.NextDouble() * 100;
                                    double influenceResult = rand.NextDouble() * 100;
                                    bool adoptBiome = adoptResult < biomeAdoptionChance;
                                    bool influenceByBiome = influenceResult < biomeInfluenceChance;
                                    if (adoptBiome || influenceByBiome)
                                    {//Adopt a nearby biome, if there is a nearby biome data to take from.

                                        if (totalDataPoints.Count > 0)
                                        {
                                            newData = totalDataPoints[rand.Next(0, totalDataPoints.Count)].Value;
                                        }
                                        else if (dataPoints.Count > 0)
                                        {
                                            newData = dataPoints.ElementAt(rand.Next(0, dataPoints.Count)).Value;
                                        }
                                        else
                                        {//If there were no newly generated or immediately nearby biomes, let's check the biomes within the total influence area.

                                            var neighboringRegions = biomeRegions.GetDataInsideGridRect(totalInfluenceRect);
                                            if (neighboringRegions.Count > 0)
                                            {
                                                var firstNearbyRegion = neighboringRegions.First();
                                                newData = firstNearbyRegion.GetFirstData();
                                            }
                                        }

                                        if (influenceByBiome)
                                        {
                                            double varianceDirection = 1;
                                            if (rand.NextDouble() < 0.5)
                                            {
                                                varianceDirection = -1;
                                            }

                                            double tempVariance = 1 + rand.NextDouble() * biomeInfluencePercentMagnitude * 0.01 * varianceDirection;
                                            double heightVariance = 1 + rand.NextDouble() * biomeInfluencePercentMagnitude * 0.01 * varianceDirection;
                                            double moistureVariance = 1 + rand.NextDouble() * biomeInfluencePercentMagnitude * 0.01 * varianceDirection;
                                            double fertilityVariance = 1 + rand.NextDouble() * biomeInfluencePercentMagnitude * 0.01 * varianceDirection;

                                            double resultingTemp = Math.Clamp(newData.Temperature * tempVariance, minTemperature, maxTemperature);
                                            double resultingMoisture = Math.Clamp(newData.Moisture * moistureVariance, minMoisture, maxMoisture);
                                            double resultingHeight = Math.Clamp(newData.Height * heightVariance, minHeight, maxHeight);
                                            double resultingFert = Math.Clamp(newData.Fertility * fertilityVariance, minFertility, maxFertility);

                                            newData = new BiomeData(resultingMoisture, resultingTemp, resultingHeight, resultingFert);
                                        }
                                        
                                    }

                                    do
                                    {
                                        biomePoint = RandomPointInRect(influenceRect, rand);
                                    }//Keep trying for a new biome point if you somehow selected one that already existed.
                                    while (dataPoints.ContainsKey(biomePoint));

                                    dataPoints[biomePoint] = newData;
                                }

                                totalDataPoints.AddRange(dataPoints);

                                currentReigon = new BiomeRegionData(influenceRect, dataPoints, rand);

                                biomeRegions.Add(influenceRectX, influenceRectY, currentReigon);

                                resultingRegions.Add(currentReigon);
                            }
                        }
                        if (existingRegions.Count > 0)
                        {
                            //Since there should only be a single existing regions (even though it's a list, ideal configuration gives us a single one),
                            //We can simply grab all the points in it. Even if there's multiple for some reason, we can grab those too.
                            totalDataPoints.AddRange(existingRegions.SelectMany(x => x.GetAllPointsAndData()));
                        }
                        resultingRegions.AddRange(existingRegions);
                    }
                }

                //The amount of resulting regions at this stage should at minimum be scanWidth times scanHeight or be default 3x3=9.
                //We must now combine these regions into one large region.
                //Combining the regions has the benefit of giving us as many points as we need to work within the provided chunk.
                //This is because we will have not only the biome points nearby/within the chunk, but also the neighbors to those points.
                //Assuming that the size of an influence rect is greater than or equal to a chunk.
                overarchingBiome = new BiomeRegionData(totalInfluenceRect, totalDataPoints, overarchingRegionRandom);

                //Add the overarching biome region incase it's needed in the future.
                overarchingBiomeRegions.Add(overarchingBiomeKey, overarchingBiome);
            }

            totalInfluenceRect = overarchingBiome.Region;

            return overarchingBiome;
        }

        public BiomeRegionData GetBiomeRegionData(PointL pointWithinRegion)
        {
            //There should only be 1 region per point.
            return biomeRegions.GetDataInsideGridRect(pointWithinRegion).First();
        }

        public BiomeRegionData GetBiomeRegionData(long worldX, long worldY)
        {
            //There should only be 1 region per point.
            return biomeRegions.GetDataInsideGridRect(worldX, worldY).First();
        }

        public BiomeData GetGeneralBiomeData(long worldX, long worldY)
        {
            BiomeRegionData overarchingBiomeRegion = GetBiomeRegionByWorldPosition(worldX, worldY, true);
            BiomeData biomeData = WorleyNoise.GetNearestBiome(worldX, worldY, overarchingBiomeRegion);
            //There should only be 1 region per point.
            return biomeData;
        }

        public BiomeData GetGeneralBiomeData(long worldX, long worldY, BiomeRegionData overarchingBiomeRegion)
        {
            BiomeData biomeData = WorleyNoise.GetNearestBiome(worldX, worldY, overarchingBiomeRegion);
            //There should only be 1 region per point.
            return biomeData;
        }

        private PointL RandomPointInRect(RectL rect, Random random)
        {
            return new PointL(rect.X + random.Next(0, (int)rect.Width), rect.Y + random.Next(0, (int)rect.Height));
        }

        private BiomeRegionData GetBiomeRegionByWorldPosition(long worldX, long worldY, bool generateNonExisting)
        {
            MapChunk chunk = GetChunkAtWorldSpace(worldX, worldY);

            return GetBiomeRegionDataByChunk(chunk, generateNonExisting);
        }

        public BiomeData GetBiomeDataForTile(long worldX, long worldY)
        {
            BiomeRegionData influencingBiomeRegion = GetBiomeRegionByWorldPosition(worldX, worldY, true);

            return GetBiomeDataByWorldCoordinates(worldX, worldY, influencingBiomeRegion);
        }

        public BiomeData GetBiomeDataByWorldCoordinates(long x, long y)
        {
            BiomeRegionData data = GetBiomeRegionByWorldPosition(x, y, false);

            WorleyNoiseBiomeReturn[] weights = WorleyNoise.GetNoiseWeights(x, y, data, blendFactor, magnitude);

            double normalizedTemperatureSum = 0;
            double normalizedMoistureSum = 0;
            double normalizedHeightSum = 0;
            double normalizedFertilitySum = 0;
            double summedWeights = 0;
            foreach (var weight in weights)
            {
                if (weight.ClampedValue < 1)
                {
                    normalizedTemperatureSum += (weight.BiomeData.Temperature - minTemperature) * (1 - weight.ClampedValue);
                    normalizedMoistureSum += (weight.BiomeData.Moisture - minMoisture) * (1 - weight.ClampedValue);
                    normalizedHeightSum += (weight.BiomeData.Height - minHeight) * (1 - weight.ClampedValue);
                    normalizedFertilitySum += (weight.BiomeData.Fertility - minFertility) * (1 - weight.ClampedValue);
                    summedWeights += 1 - weight.ClampedValue;
                }
            }

            if (summedWeights <= 0) //Should probably use an epsilon value for this comparison.
            {//This should rarely be the case, but if the magnitude is high enough it can become more common.
                double minValue = double.MaxValue;
                BiomeData closestData = weights[0].BiomeData;
                foreach (var weight in weights)
                {
                    if (minValue > weight.UnclampedValue)
                    {
                        minValue = weight.UnclampedValue;
                        closestData = weight.BiomeData;
                    }
                }
                normalizedTemperatureSum = closestData.Temperature - minTemperature;
                normalizedMoistureSum = closestData.Moisture - minTemperature;
                normalizedHeightSum = closestData.Height - minHeight;
                normalizedFertilitySum = closestData.Fertility - minFertility;
            }
            else
            {//Otherwise the normal way is the average the results.
                normalizedTemperatureSum /= summedWeights;
                normalizedMoistureSum /= summedWeights;
                normalizedHeightSum /= summedWeights;
                normalizedFertilitySum /= summedWeights;
            }

            //Since the values were normalized to be the range starting from 0, we need to add the minimum temperature back in.
            var blendedResult = new BiomeData(minMoisture + normalizedMoistureSum, minTemperature + normalizedTemperatureSum
                , minHeight + normalizedHeightSum, minFertility + normalizedFertilitySum);

            return blendedResult;
        }

        public BiomeData GetBiomeDataByWorldCoordinates(long x, long y, WorleyNoiseBiomeReturn[] weights)
        {
            double normalizedTemperatureSum = 0;
            double normalizedMoistureSum = 0;
            double normalizedHeightSum = 0;
            double normalizedFertilitySum = 0;
            double summedWeights = 0;
            foreach (var weight in weights)
            {
                if (weight.ClampedValue < 1)
                {
                    normalizedTemperatureSum += (weight.BiomeData.Temperature - minTemperature) * (1 - weight.ClampedValue);
                    normalizedMoistureSum += (weight.BiomeData.Moisture - minMoisture) * (1 - weight.ClampedValue);
                    normalizedHeightSum += (weight.BiomeData.Height - minHeight) * (1 - weight.ClampedValue);
                    normalizedFertilitySum += (weight.BiomeData.Fertility - minFertility) * (1 - weight.ClampedValue);
                    summedWeights += 1 - weight.ClampedValue;
                }
            }

            if (summedWeights <= 0) //Should probably use an epsilon value for this comparison.
            {//This should rarely be the case, but if the magnitude is high enough it can become more common.
                double minValue = double.MaxValue;
                BiomeData closestData = weights[0].BiomeData;
                foreach (var weight in weights)
                {
                    if (minValue > weight.UnclampedValue)
                    {
                        minValue = weight.UnclampedValue;
                        closestData = weight.BiomeData;
                    }
                }
                normalizedTemperatureSum = closestData.Temperature - minTemperature;
                normalizedMoistureSum = closestData.Moisture - minTemperature;
                normalizedHeightSum = closestData.Height - minHeight;
                normalizedFertilitySum = closestData.Fertility - minFertility;
            }
            else
            {//Otherwise the normal way is the average the results.
                summedWeights = 1 / summedWeights;

                normalizedTemperatureSum *= summedWeights;
                normalizedMoistureSum *= summedWeights;
                normalizedHeightSum *= summedWeights;
                normalizedFertilitySum *= summedWeights;
            }

            //Since the values were normalized to be the range starting from 0, we need to add the minimum temperature back in.
            var blendedResult = new BiomeData(minMoisture + normalizedMoistureSum, minTemperature + normalizedTemperatureSum
                , minHeight + normalizedHeightSum, minFertility + normalizedFertilitySum);

            return blendedResult;
        }

        public BiomeData GetBiomeDataByWorldCoordinates(long x, long y, BiomeRegionData data)
        {
            WorleyNoiseBiomeReturn[] weights = WorleyNoise.GetNoiseWeights(x, y, data, blendFactor, magnitude);

            double normalizedTemperatureSum = 0;
            double normalizedMoistureSum = 0;
            double normalizedHeightSum = 0;
            double normalizedFertilitySum = 0;
            double summedWeights = 0;
            foreach (var weight in weights )
            {
                if (weight.ClampedValue < 1)
                {
                    normalizedTemperatureSum += (weight.BiomeData.Temperature - minTemperature) * (1 - weight.ClampedValue);
                    normalizedMoistureSum += (weight.BiomeData.Moisture - minMoisture) * (1 - weight.ClampedValue);
                    normalizedHeightSum += (weight.BiomeData.Height - minHeight) * (1 - weight.ClampedValue);
                    normalizedFertilitySum += (weight.BiomeData.Fertility - minFertility) * (1 - weight.ClampedValue);
                    summedWeights += 1 - weight.ClampedValue;
                }
            }

            if (summedWeights <= 0) //Should probably use an epsilon value for this comparison.
            {//This should rarely be the case, but if the magnitude is high enough it can become more common.
                double minValue = double.MaxValue;
                BiomeData closestData = weights[0].BiomeData;
                foreach (var weight in weights)
                {
                    if (minValue > weight.UnclampedValue)
                    {
                        minValue = weight.UnclampedValue;
                        closestData = weight.BiomeData;
                    }
                }
                normalizedTemperatureSum = closestData.Temperature - minTemperature;
                normalizedMoistureSum = closestData.Moisture - minTemperature;
                normalizedHeightSum = closestData.Height - minHeight;
                normalizedFertilitySum = closestData.Fertility - minFertility;
            }
            else
            {//Otherwise the normal way is the average the results.
                normalizedTemperatureSum /= summedWeights;
                normalizedMoistureSum /= summedWeights;
                normalizedHeightSum /= summedWeights;
                normalizedFertilitySum /= summedWeights;
            }

            //Since the values were normalized to be the range starting from 0, we need to add the minimum temperature back in.
            var blendedResult = new BiomeData(minMoisture + normalizedMoistureSum, minTemperature + normalizedTemperatureSum
                , minHeight + normalizedHeightSum, minFertility + normalizedFertilitySum);

            return blendedResult;
        }

        private BiomeData GenerateRandomBiomeData(Random random)
        {
            double moisture = Math.Clamp(random.NextDouble() * moistureRange + minMoisture, minMoisture, maxMoisture);

            double summedTemp = random.NextDouble() * temperatureRange
                                + random.NextDouble() * temperatureRange
                                + random.NextDouble() * temperatureRange
                                + random.NextDouble() * temperatureRange;

            double temperature = Math.Clamp(summedTemp * 0.25 + minTemperature, minTemperature, maxTemperature);

            double summedFert = random.NextDouble() * fertilityRange
                + random.NextDouble() * fertilityRange;

            double fertility = Math.Clamp(summedFert * 0.5 + minFertility, minFertility, maxFertility);

            double height = Math.Clamp(random.NextDouble() * heightRange + minHeight, minHeight, maxHeight);

            return new BiomeData(moisture, temperature,height, fertility);
        }

        /// <summary>
        /// Gets the chunk at the given <b>world</b> space. Which is dependent on the chunk width/height.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public override MapChunk GetChunkAtWorldSpace(long x, long y)
        {
            long chunkId = GetChunkIdFromWorldSpace(x, y);

            bool chunkExists = mapChunkMapping.TryGetValue(chunkId,out var chunk);

            if (generateChunksThatDontExist && !chunkExists)
            {
                chunk = GenerateChunk(x, y);
                mapChunkMapping[chunkId] = chunk;
            }

            return chunk;
        }

        public static RectL GetRectangleInGrid(long x, long y, long width, long height)
        {
            long left = MathL.Floor(x, width);
            long top = MathL.Floor(y, height);

            return new RectL(left, top, width, height);
        }


    }
}

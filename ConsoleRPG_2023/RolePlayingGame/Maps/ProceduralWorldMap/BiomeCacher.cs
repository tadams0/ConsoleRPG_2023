using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleRPG_2023.RolePlayingGame.Maps
{
    public class BiomeCacher
    {
        private Dictionary<BiomeType, Biome> biomes = new Dictionary<BiomeType, Biome>();
        private Biome defaultBiome;

        private Random random;

        public BiomeCacher(long seed)
        {
            random = new Random((int)seed);
        }

        public Biome GetBiome(BiomeType biomeType)
        {
            if (biomes.TryGetValue(biomeType, out var biome))
            { 
                return biome; 
            }
            return defaultBiome;
        }

        /// <summary>
        /// Builds all biomes.
        /// </summary>
        public void Build()
        {
            //TODO: build  a framework around generating a handful of random numbers each tile no matter what.
            //This way the world generation is more consistent per seed.
            BiomeType biomeType = BiomeType.Plains;
            Biome newBiome = new Biome(biomeType);
            newBiome.AddTileType(0, (x) =>
            {
                if (x.Height < 200)
                {
                    return TileType.Water;
                }
                int rand = random.Next(0, 100);
                int rand2 = random.Next(505, 515);
                int witherThreshold = random.Next(317, 323);
                if (x.Height > rand2)
                {
                    if (rand < Math.Min(95,(x.Height - 500) / 2))
                    {
                        return TileType.MountainRock;
                    }
                    rand = random.Next(0, 100);
                    if (rand < 70 || x.Fertility > 60)
                    {
                        if (x.Temperature > witherThreshold)
                        {//hot
                            return TileType.GrassWithered;
                        }
                        else
                        {
                            return TileType.GrassMild;
                        }
                    }
                    else if (x.Temperature < witherThreshold && rand < 95)
                    {
                        return TileType.GrassTall;
                    }
                    else
                    {
                        return TileType.GrassSparse;
                    }
                }

                if (x.Temperature > witherThreshold)
                {
                    if (rand < 70)
                    {
                        return TileType.GrassWithered;
                    }
                    else
                    {
                        return TileType.GrassSparse;
                    }
                }
                else
                {
                    if (rand < 70)
                    {
                        return TileType.GrassMild;
                    }
                    else
                    {
                        return TileType.GrassTall;
                    }
                }
            });

            newBiome.AddObjectInitSingleTileFunction(0, (biomeData, tile) => random.Next(0, 1000) < 3 ? new MapTree(TreeType.Cedar) : null);
            newBiome.AddObjectInitSingleTileFunction(1, (biomeData, tile) => random.Next(0, 10000) < 5 ? new MapTree(TreeType.Oak) : null);
            biomes[biomeType] = newBiome;

            biomeType = BiomeType.None;
            newBiome = new Biome(biomeType);
            newBiome.AddTileType(0, x => TileType.None);
            biomes[biomeType] = newBiome;
            defaultBiome = newBiome;

            biomeType = BiomeType.Ocean;
            newBiome = new Biome(biomeType);
            newBiome.AllowAnyTileTypeBlending = false;
            newBiome.AddTileTypeForBlending(TileType.Ice);
            newBiome.AddTileType(0, (x) => x.Height > 200 ? TileType.Beach : TileType.None);
            newBiome.AddTileType(1, x => x.Temperature < 270 ? TileType.Ice : TileType.None);
            newBiome.AddTileType(2, x => TileType.Water);
            biomes[biomeType] = newBiome;

            biomeType = BiomeType.Woodland;
            newBiome = new Biome(biomeType);
            newBiome.AddTileType(0, x => x.Temperature < random.Next(317, 323) ? TileType.GrassTall : TileType.GrassWithered);
            newBiome.AddObjectInitSingleTileFunction(0, (biomeData, tile) => random.Next(0, 100) < 10 ? new MapTree(TreeType.Cedar) : null);
            newBiome.AddObjectInitSingleTileFunction(1, (biomeData, tile) => random.Next(0, 100) < 4 ? new MapTree(TreeType.Oak) : null);
            newBiome.AddObjectInitSingleTileFunction(2, (biomeData, tile) => random.Next(0, 100) < 6 ? new MapTree(TreeType.Birch) : null);
            biomes[biomeType] = newBiome;

            biomeType = BiomeType.Forest;
            newBiome = new Biome(biomeType);
            newBiome.AddTileType(0, x => x.Temperature < random.Next(337, 343) ? TileType.GrassThick : TileType.GrassWithered);
            newBiome.AddObjectInitSingleTileFunction(0, (biomeData, tile) => random.Next(0, 100) < 20 ? new MapTree(TreeType.Cedar) : null);
            newBiome.AddObjectInitSingleTileFunction(1, (biomeData, tile) => random.Next(0, 100) < 15 ? new MapTree(TreeType.Oak) : null);
            newBiome.AddObjectInitSingleTileFunction(2, (biomeData, tile) => random.Next(0, 100) < 15 ? new MapTree(TreeType.Fir) : null);
            biomes[biomeType] = newBiome;

            biomeType = BiomeType.Mountains;
            newBiome = new Biome(biomeType);
            newBiome.AddTileType(0, x => TileType.MountainRock);
            biomes[biomeType] = newBiome;

            biomeType = BiomeType.Desert;
            newBiome = new Biome(biomeType);
            newBiome.AddTileType(0, x => TileType.FineSand);
            newBiome.AddObjectInitSingleTileFunction(0, (biomeData, tile) => random.Next(0, 1000) < 2 ? new MapTree(TreeType.Grately) : null);
            biomes[biomeType] = newBiome;

            biomeType = BiomeType.IntenseDesert;
            newBiome = new Biome(biomeType);
            newBiome.AddTileType(0, x => random.Next(0, 100) < 5 ? TileType.HellRock : TileType.None);
            newBiome.AddTileType(1, x => x.Temperature > 380 + random.Next(0, 10) ? TileType.Lava : TileType.None);
            newBiome.AddTileType(2, x => TileType.FineSand);
            newBiome.AddObjectInitSingleTileFunction(0, (biomeData, tile) => random.Next(0, 20000) < 5 ? new MapTree(TreeType.Grately) : null);
            biomes[biomeType] = newBiome;

            biomeType = BiomeType.SnowyPlains;
            newBiome = new Biome(biomeType);
            newBiome.AddTileType(0, x => TileType.Snow);
            biomes[biomeType] = newBiome;

            biomeType = BiomeType.FrozenOcean;
            newBiome = new Biome(biomeType);
            newBiome.AddTileType(0, x => x.Temperature > 270 ? TileType.Water : TileType.None);
            newBiome.AddTileType(1, x => TileType.Ice);
            biomes[biomeType] = newBiome;

            biomeType = BiomeType.DemonicHellscape;
            newBiome = new Biome(biomeType);
            newBiome.AddTileType(0, x => TileType.HellRock);

            newBiome.AddObjectInitSingleTileFunction(0, (biomeData, tile) => random.Next(0, 1000) < 100 ? new MapTree(TreeType.Hell) : null);
            biomes[biomeType] = newBiome;

            biomeType = BiomeType.SnowyWoodland;
            newBiome = new Biome(biomeType);
            newBiome.AddTileType(0, x => TileType.IcyGrassMild);
            newBiome.AddObjectInitSingleTileFunction(0, (biomeData, tile) => random.Next(0, 100) < 15 ? new MapTree(TreeType.Oak) : null);
            newBiome.AddObjectInitSingleTileFunction(1, (biomeData, tile) => random.Next(0, 100) < 15 ? new MapTree(TreeType.Fir) : null);
            biomes[biomeType] = newBiome;

            biomeType = BiomeType.CorruptedFirelands;
            newBiome = new Biome(biomeType);
            newBiome.AddTileType(0, x => TileType.FireSand);
            newBiome.AddObjectInitSingleTileFunction(0, (biomeData, tile) => random.Next(0, 1000) < 10 ? new MapTree(TreeType.Hell) : null);
            newBiome.AddObjectInitSingleTileFunction(1, (biomeData, tile) => random.Next(0, 20000) < 6 ? new MapTree(TreeType.GreaterCorrupted) : null);
            biomes[biomeType] = newBiome;

            biomeType = BiomeType.Bog;
            newBiome = new Biome(biomeType);
            newBiome.AddTileType(0, x => random.Next(0,100) < 15 ? TileType.MurkeyWater : TileType.None);
            newBiome.AddTileType(1, x => TileType.Mud);
            newBiome.AddObjectInitSingleTileFunction(0, (biomeData, tile) => random.Next(0, 100) < 15 ? new MapTree(TreeType.Willow) : null);
            newBiome.AddObjectInitSingleTileFunction(1, (biomeData, tile) => random.Next(0, 100) < 5 ? new MapTree(TreeType.Cypress) : null);
            biomes[biomeType] = newBiome;

            biomeType = BiomeType.Swamp;
            newBiome = new Biome(biomeType);
            newBiome.AddTileType(0, x => random.Next(0, 100) < 20 ? TileType.MurkeyWater : TileType.None);
            newBiome.AddTileType(1, x => random.Next(0,100) < 90 ? TileType.Mud : TileType.None);
            newBiome.AddTileType(2, x => TileType.Water);
            newBiome.AddObjectInitSingleTileFunction(0, (biomeData, tile) => random.Next(0, 100) < 10 ? new MapTree(TreeType.Willow) : null);
            newBiome.AddObjectInitSingleTileFunction(1, (biomeData, tile) => random.Next(0, 100) < 10 ? new MapTree(TreeType.Cypress) : null);
            biomes[biomeType] = newBiome;

            biomeType = BiomeType.DenseForest;
            newBiome = new Biome(biomeType);
            newBiome.AddTileType(0, x => TileType.GrassThick);
            newBiome.AddObjectInitSingleTileFunction(0, (biomeData, tile) => biomeData.Fertility > 100 ? new MapTree(TreeType.Oak) : null);
            newBiome.AddObjectInitSingleTileFunction(1, (biomeData, tile) => random.Next(0, 100) < 80 ? new MapTree(TreeType.Oak) : null);
            newBiome.AddObjectInitSingleTileFunction(2, (biomeData, tile) => random.Next(0, 100) < 10 ? new MapTree(TreeType.Cedar) : null);
            biomes[biomeType] = newBiome;

            biomeType = BiomeType.CorruptedDesert;
            newBiome = new Biome(biomeType);
            newBiome.AddTileType(0, x => random.Next(0,100) < 50 ? TileType.CorruptedSand : TileType.FineSand);
            biomes[biomeType] = newBiome;

            biomeType = BiomeType.CorruptedPlains;
            newBiome = new Biome(biomeType);
            newBiome.AddTileType(0, x => random.Next(0, 100) < 10 ? TileType.Corrupted : TileType.None);
            newBiome.AddTileType(1, x => random.Next(1, 100) < 20 ? TileType.CorruptedGrassTall : TileType.None);
            newBiome.AddTileType(2, x => TileType.CorruptedGrassMild);
            biomes[biomeType] = newBiome;

            biomeType = BiomeType.CorruptedForest;
            newBiome = new Biome(biomeType);
            newBiome.AddTileType(0, x => random.Next(0, 100) < 10 ? TileType.Corrupted : TileType.None);
            newBiome.AddTileType(1, x => random.Next(1, 100) < 20 ? TileType.CorruptedGrassMild : TileType.None);
            newBiome.AddTileType(2, x => TileType.CorruptedGrassTall);
            newBiome.AddObjectInitSingleTileFunction(0, (biomeData, tile) => random.Next(0, 100) < 10 ? new MapTree(TreeType.Corrupted) : null);
            newBiome.AddObjectInitSingleTileFunction(1, (biomeData, tile) => random.Next(0, 20000) < 4 ? new MapTree(TreeType.GreaterCorrupted) : null);
            biomes[biomeType] = newBiome;

            biomeType = BiomeType.Highlands;
            newBiome = new Biome(biomeType);
            newBiome.AddTileType(0, x => random.Next(0, 100) < 10 ? TileType.GrassSparse : TileType.None);
            newBiome.AddTileType(1, x => random.Next(1, 100) < 70 ? TileType.GrassMild : TileType.None);
            newBiome.AddTileType(2, x => TileType.MountainRock);
            newBiome.AddObjectInitSingleTileFunction(0, (biomeData, tile) => random.Next(0, 100) < 4 ? new MapTree(TreeType.Fir) : null);
            newBiome.AddObjectInitSingleTileFunction(1, (biomeData, tile) => random.Next(0, 100) < 4 ? new MapTree(TreeType.Oak) : null);
            biomes[biomeType] = newBiome;

            biomeType = BiomeType.Volcanic;
            newBiome = new Biome(biomeType);
            newBiome.AddTileType(0, x => x.Height < random.Next(395,404) ? TileType.MountainRock : TileType.None);
            newBiome.AddTileType(1, x => x.Temperature > random.Next(360, 365) && x.Temperature < 370 ? TileType.HeatStone : TileType.None);
            newBiome.AddTileType(2, x => x.Height > 415 && x.Temperature > 370 ? TileType.Lava : TileType.None);
            newBiome.AddTileType(3, x => TileType.DryMagmaStone);
            biomes[biomeType] = newBiome;

            biomeType = BiomeType.CorruptedTundra;
            newBiome = new Biome(biomeType);
            newBiome.AddTileType(0, x => TileType.CorruptedIcyGrassMild);
            newBiome.AddObjectInitSingleTileFunction(0, (biomeData, tile) => random.Next(0, 5000) < 9 ? new MapTree(TreeType.Ice) : null);
            biomes[biomeType] = newBiome;

            biomeType = BiomeType.ExtremeMountains;
            newBiome = new Biome(biomeType);
            newBiome.AddTileType(0, (x) => {
                    if (x.Height > 900 && x.Moisture > 45 )
                    {
                        return TileType.MountainsSnow;
                    }
                    else if (x.Height > 680 + random.Next(0,40))
                    {
                        return TileType.MountainsPeakRock;
                    }
                    else
                    {
                        return TileType.MountainRock;
                    }
                });
            newBiome.AddObjectInitSingleTileFunction(0, (biomeData, tile) => random.Next(0, 100) < 4 ? new MapTree(TreeType.Fir) : null);
            newBiome.AddObjectInitSingleTileFunction(1, (biomeData, tile) => random.Next(0, 100) < 4 ? new MapTree(TreeType.Oak) : null);
            biomes[biomeType] = newBiome;
        }

    }
}

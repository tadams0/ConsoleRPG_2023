using ConsoleRPG_2023.RolePlayingGame.Noise;
using csDelaunay;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleRPG_2023.RolePlayingGame.Maps
{
    /// <summary>
    /// Defines a class that holds the state of a single biome influence region slice.
    /// </summary>
    public class BiomeRegionData
    {
        /// <summary>
        /// The biome region's voronoi diagram.
        /// </summary>
        public Voronoi<BiomeData> Voronoi { get; }
        
        /// <summary>
        /// Determines if the voronoi diagram was generated for this region.
        /// <br/>Note that regions with 2 or less points will not have a diagram.
        /// </summary>
        public bool HasVoronoi
        {
            get { return Voronoi != null; }
        }

        /// <summary>
        /// The total encompassing region of the biome data.
        /// </summary>
        public RectL Region 
        { 
            get { return region; }
            set { SetRegion(value); }
        }

        /// <summary>
        /// Gets the number of data and Voronoi sites.
        /// </summary>
        public int NumberOfData
        {
            get { return biomeData.Count; }
        }

        public float InverseRegionWidth { get; private set; }

        public float InverseRegionHeight { get; private set; }

        /// <summary>
        /// Gets the random generator assigned to the biome region.
        /// </summary>
        public Random Random
        {
            get { return random; }
        }


        private Dictionary<PointL, BiomeData> biomeData = new Dictionary<PointL, BiomeData>();

        private RectL region;

        private Random random;

        public BiomeRegionData(RectL region, List<KeyValuePair<PointL, BiomeData>> dataPoints, Random random)
        {
            if (dataPoints.Count > 2)
            {
                Voronoi = WorleyNoise.CreateVoronoi(dataPoints, region);
            }

            this.random = random;

            SetRegion(region);
            foreach (var pair in dataPoints)
            {
                biomeData.Add(pair.Key, pair.Value);
            }
        }

        public BiomeRegionData(RectL region, IEnumerable<KeyValuePair<PointL, BiomeData>> dataPoints, Random random)
        {
            if (dataPoints.Count() > 2)
            {
                Voronoi = WorleyNoise.CreateVoronoi(dataPoints.ToList(), region);
            }

            this.random = random;

            SetRegion(region);
            foreach (var pair in dataPoints)
            {
                biomeData.Add(pair.Key, pair.Value);
            }
        }

        private void SetRegion(RectL region)
        {
            this.region = region;
            InverseRegionHeight = 1.0f / region.Height;
            InverseRegionWidth = 1.0f / region.Width;
        }

        /// <summary>
        /// Gets the biome data located at the given voronoi point. The point should be in world space.
        /// </summary>
        /// <param name="vector">The world based point.</param>
        public BiomeData GetBiomeDataFromPoint(PointL point)
        {
            return biomeData[point];
        }

        /// <summary>
        /// Gets the biome data located at the given voronoi point. This assumes a relative
        /// point where 0,0 is the top left of this <see cref="BiomeRegionData"/>'s region.
        /// </summary>
        /// <param name="vector">The relative point.</param>
        public BiomeData GetBiomeDataFromPoint(Vector2f vector)
        {
            PointL point = new PointL((long)(Region.X + (vector.x * Region.Width)), (long)(Region.Y + (vector.y * Region.Height)));
            
            return biomeData[point];
        }

        /// <summary>
        /// Gets all the points within this instance. The points are in world space.
        /// </summary>
        public List<PointL> GetAllPoints()
        {
            return biomeData.Select(x=>x.Key).ToList();
        }

        /// <summary>
        /// Gets all the points and their associated data within this instance. The points are in world space.
        /// </summary>
        public Pair<PointL, BiomeData>[] GetAllPointsAndDataInPairs()
        {
            Pair<PointL, BiomeData>[] result = new Pair<PointL, BiomeData>[biomeData.Count];
            int index = 0;
            foreach (var pair in biomeData)
            {
                result[index] = new Pair<PointL, BiomeData>(pair.Key, pair.Value);
                index++;
            }
            return result;
        }

        /// <summary>
        /// Gets all the points and their associated data within this instance. The points are in world space.
        /// </summary>
        public List<KeyValuePair<PointL, BiomeData>> GetAllPointsAndData()
        {
            return biomeData.ToList();
        }

        /// <summary>
        /// Adds biome data to the given point.
        /// </summary>
        /// <param name="point">The world space point to add the data to.</param>
        /// <param name="data">The data to add.</param>
        public void AddBiomeDataAtPoint(PointL point, BiomeData data)
        {
            biomeData.Add(point, data);
        }

        public BiomeData GetFirstData()
        {
            return biomeData.First().Value;
        }

    }
}

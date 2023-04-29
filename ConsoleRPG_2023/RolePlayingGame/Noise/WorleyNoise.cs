using ConsoleRPG_2023.RolePlayingGame.Maps;
using ConsoleRPG_2023.RolePlayingGame.Structs;
using csDelaunay;
using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using System.Text;

namespace ConsoleRPG_2023.RolePlayingGame.Noise
{
    /// <summary>
    /// Defines a class that is capable of generating worley-type noise.
    /// </summary>
    public static class WorleyNoise
    {
        /// <summary>
        /// Normalizes the list of KVP points with biome data into a lossy <see cref="Point"/> for compatability with a <see cref="Delaunator"/>.
        /// </summary>
        /// <param name="points">The points to normalize.</param>
        /// <param name="pointRegion">The rectangular region to normalize to where the X and Y become the 0,0.</param>
        public static Vector2[] Normalize(List<KeyValuePair<PointL, BiomeData>> points, RectL pointRegion)
        {
            Vector2[] normalizedPoints = new Vector2[points.Count];
            for (int i = 0; i < points.Count; i++)
            {
                var point = points[i];
                Vector2 normalizedPoint = new Vector2((point.Key.X - pointRegion.X) / (float)pointRegion.Width, (point.Key.Y - pointRegion.Y) / (float)pointRegion.Height);
                normalizedPoints[i] = normalizedPoint;
            }
            return normalizedPoints;
        }

        /// <summary>
        /// Normalizes the list of KVP points with biome data into a lossy <see cref="Point"/> for compatability with a <see cref="Delaunator"/>.
        /// </summary>
        /// <param name="points">The points to normalize.</param>
        /// <param name="pointRegion">The rectangular region to normalize to where the X and Y become the 0,0.</param>
        public static List<Vector2f> NormalizeF(List<KeyValuePair<PointL, BiomeData>> points, RectL pointRegion)
        {
            List<Vector2f> normalizedPoints = new List<Vector2f>(points.Count);
            for (int i = 0; i < points.Count; i++)
            {
                var point = points[i];
                Vector2f normalizedPoint = new Vector2f((point.Key.X - pointRegion.X) / (float)pointRegion.Width, (point.Key.Y - pointRegion.Y) / (float)pointRegion.Height);
                normalizedPoints.Add(normalizedPoint);
            }
            return normalizedPoints;
        }

        public static Vector2f NormalizePointLToF(PointL point, BiomeRegionData region)
        {
            float normalizedX = (point.X - region.Region.X) * region.InverseRegionWidth;
            float normalizedY = (point.Y - region.Region.Y) * region.InverseRegionHeight;
            return new Vector2f(normalizedX, normalizedY);
        }

        public static Vector2f NormalizePointLToF(PointL point, RectL pointRegion)
        {
            return new Vector2f((point.X - pointRegion.X) / (float)pointRegion.Width, (point.Y - pointRegion.Y) / (float)pointRegion.Height); 
        }

        public static Vector2f NormalizePointLToF(long x, long y, RectL pointRegion)
        {
            return new Vector2f((x - pointRegion.X) / (float)pointRegion.Width, (y - pointRegion.Y) / (float)pointRegion.Height);
        }

        public static Voronoi<BiomeData> CreateVoronoi(List<KeyValuePair<PointL, BiomeData>> points, RectL pointRegion)
        {
            List<KeyValuePair<Vector2f, BiomeData>> normalizedData = new List<KeyValuePair<Vector2f, BiomeData>>(points.Count);

            //Normalize the points to be relative to the point region where top left is 0,0
            for (int i = 0; i < points.Count; i++)
            {
                var point = points[i];
                Vector2f normalizedPoint = new Vector2f((point.Key.X - pointRegion.X) / (float)pointRegion.Width, (point.Key.Y - pointRegion.Y) / (float)pointRegion.Height);
                normalizedData.Add(new KeyValuePair<Vector2f, BiomeData>(normalizedPoint, points[i].Value));
            }

            //"Normalizing" the point region to 0,0
            pointRegion.X = 0;
            pointRegion.Y = 0;

            Voronoi<BiomeData> v = new Voronoi<BiomeData>(normalizedData, pointRegion.RectLToF());

            return v;
        }

        public static WorleyNoiseBiomeReturn[] GetNoiseWeights(long x, long y, BiomeRegionData data, float blendFactor, float magnitude)
        {
            WorleyNoiseBiomeReturn[] results = new WorleyNoiseBiomeReturn[data.NumberOfData];

            //normalize x and y
            Vector2f normalizedPoint = NormalizePointLToF(x, y, data.Region);
            Pair<PointL, BiomeData>[] pointsAndData = data.GetAllPointsAndDataInPairs();
            Pair<PointL, BiomeData> currentPoint;
            BiomeData pointData;
            float distance;
            float blendedDistance;
            float clampedValue;
            float distMag;
            Vector2f normalizedVoronoiPoint;

            for (int i = 0; i < pointsAndData.Length; i++)
            {
                currentPoint = pointsAndData[i];
                pointData = currentPoint.Value2;
                normalizedVoronoiPoint = NormalizePointLToF(currentPoint.Value1, data);
                distance = normalizedVoronoiPoint.DistanceSquare(normalizedPoint);
                blendedDistance = distance * (1f - blendFactor);
                distMag = blendedDistance * magnitude;
                clampedValue = Clamp01F(distMag);

                results[i] = new WorleyNoiseBiomeReturn(pointData, clampedValue, distMag);
            }

            return results;
        }

        public static BiomeData GetNearestBiome(long x, long y, BiomeRegionData data)
        {
            Vector2f normalizedPoint = NormalizePointLToF(x, y, data.Region);
            Pair<PointL, BiomeData>[] pointsAndData = data.GetAllPointsAndDataInPairs();
            BiomeData pointData = new BiomeData();
            float distance;
            float shortestDistance = float.MaxValue;
            Vector2f normalizedVoronoiPoint;

            for (int i = 0; i < pointsAndData.Length; i++)
            {
                normalizedVoronoiPoint = NormalizePointLToF(pointsAndData[i].Value1, data);
                distance = normalizedVoronoiPoint.DistanceSquare(normalizedPoint);
                if (distance < shortestDistance)
                {
                    pointData = pointsAndData[i].Value2;
                    shortestDistance = distance;
                }
            }

            return pointData;
        }

        public static double GetNoiseWithVoronoi(long x, long y, Voronoi<BiomeData> v, RectL pointRegion, float blendFactor, float magnitude = 5)
        {
            double closestDistance = double.MaxValue;
            double secondClosestDistance = double.MaxValue;

            //normalize x and y
            Vector2f normalizedPoint = NormalizePointLToF(x,y, pointRegion);

            foreach (Vector2f voronoiPoint in v.SiteCoords())
            {
                float distance = voronoiPoint.DistanceSquare(normalizedPoint);
                //float distance = Vector2.Distance(voronoiPoint.VectorFTo2(), normalizedPoint.VectorFTo2());
                if (distance < closestDistance)
                {
                    secondClosestDistance = closestDistance;
                    closestDistance = distance;
                }
                else if (distance < secondClosestDistance)
                {
                    secondClosestDistance = distance;
                }
            }

            double blendedDistance = closestDistance * (1f - blendFactor) + secondClosestDistance * blendFactor;

            return InverseLerp(0f, 1f, blendedDistance * magnitude);
        }

        public static double GetNoise(long x, long y, List<PointL> points, RectL pointRegion, double blendFactor)
        {
            double closestDistance = double.MaxValue;
            double secondClosestDistance = double.MaxValue;

            //normalize x and y
            double dx = (x - pointRegion.X) / (double)pointRegion.Width; 
            double dy = (y - pointRegion.Y) / (double)pointRegion.Height;

            foreach (PointL point in points)
            {
                Vector2 normalizedPoint = new Vector2((point.X - pointRegion.X) / (float)pointRegion.Width, (point.Y - pointRegion.Y) / (float)pointRegion.Height);
                double distance = Vector2.Distance(normalizedPoint, new Vector2((float)dx, (float)dy));
                if (distance < closestDistance)
                {
                    secondClosestDistance = closestDistance;
                    closestDistance = distance;
                }
                else if (distance < secondClosestDistance)
                {
                    secondClosestDistance = distance;
                }
            }

            double blendedDistance = closestDistance * (1f - blendFactor) + secondClosestDistance * blendFactor;

            return InverseLerp(0f, 1f, blendedDistance);
        }

        private static double Lerp(double a, double b, double f)
        {
            return a * (1.0 - f) + (b * f);
        }

        private static double InverseLerp(double a, double b, double value)
        {
            if (a != b)
            {
                return Clamp01((value - a) / (b - a));
            }
            else
            {
                return 0f;
            }
        }

        private static double Clamp01(double value)
        {
            return value < 0f ? 0f : (value > 1f ? 1f : value);
        }

        private static float Clamp01F(float value)
        {
            return value < 0f ? 0f : (value > 1f ? 1f : value);
        }

    }
}

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace ConsoleRPG_2023.RolePlayingGame.Collections
{
    /// <summary>
    /// Defines a spacial hashmap used to store <see cref="PointL"/> for quick retrieval with hashes. It is limited by using a hash based off a maximum width.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class LimitedSpatialHashmap<T>
    {
        private readonly long maximumWidth;
        private readonly long cellWidth;
        private readonly long cellHeight;
        private readonly Dictionary<long, List<KeyValuePair<PointL, T>>> _hashTable;

        public LimitedSpatialHashmap(long cellWidth, long cellHeight, long maximumWidth)
        {
            this.maximumWidth = maximumWidth;
            this.cellWidth = cellWidth;
            this.cellHeight = cellHeight;

            _hashTable = new Dictionary<long, List<KeyValuePair<PointL, T>>>();
        }

        public void Add(PointL point, T value)
        {
            long cellX = MathL.Floor(point.X, cellWidth) / cellWidth;
            long cellY = MathL.Floor(point.Y, cellHeight) / cellHeight;
            long cellHash = Hash(cellX, cellY);

            if (!_hashTable.TryGetValue(cellHash, out var list))
            {
                list = new List<KeyValuePair<PointL, T>>();
                _hashTable[cellHash] = list;
            }

            list.Add(new KeyValuePair<PointL, T>(point, value));
        }

        public void AddRange(IEnumerable<KeyValuePair<PointL, T>> points)
        {
            foreach (var point in points)
            {
                Add(point.Key, point.Value);
            }
        }
        public List<KeyValuePair<PointL, T>> GetPointsInsideGridRect(RectL gridLockedRect)
        {
            List<KeyValuePair<PointL, T>> result = new List<KeyValuePair<PointL, T>>();
            long startX = (gridLockedRect.X / cellWidth);
            long endX = ((gridLockedRect.X + gridLockedRect.Width) / cellWidth);
            long startY = (gridLockedRect.Y / cellHeight);
            long endY = ((gridLockedRect.Y + gridLockedRect.Height) / cellHeight);

            for (long y = startY; y < endY; y++)
            {
                for (long x = startX; x < endX; x++)
                {
                    long cellHash = Hash(x, y);
                    if (_hashTable.TryGetValue(cellHash, out var cell))
                    {
                        foreach (var kvp in cell)
                        {
                            result.Add(kvp);
                        }
                    }
                }
            }

            return result;
        }

        public List<KeyValuePair<PointL, T>> GetPointsInsideBox(RectL box)
        {
            List<KeyValuePair<PointL, T>> result = new List<KeyValuePair<PointL, T>>();
            long startX = (box.X / cellWidth);
            long endX = ((box.X + box.Width) / cellWidth);
            long startY = (box.Y / cellHeight);
            long endY = ((box.Y + box.Height) / cellHeight);

            for (long y = startY; y <= endY; y++)
            {
                for (long x = startX; x <= endX; x++)
                {
                    long cellHash = Hash(x, y);
                    if (_hashTable.TryGetValue(cellHash, out var cell))
                    {
                        foreach (var kvp in cell)
                        {
                            if (box.Contains(kvp.Key))
                            {
                                result.Add(kvp);
                            }
                        }
                    }
                }
            }

            return result;
        }


        private long Hash(long x, long y)
        {
            return y * maximumWidth + x;
        }
    }
    
}

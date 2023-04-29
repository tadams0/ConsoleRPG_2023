using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace ConsoleRPG_2023.RolePlayingGame.Collections
{
    /// <summary>
    /// Defines a spacial hashmap used to store <see cref="PointL"/> for quick retrieval with hashes.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SpatialHashTable<T>
    {
        private readonly long _cellSize;
        private readonly Dictionary<long, List<KeyValuePair<PointL, T>>> _hashTable;

        public SpatialHashTable(long cellSize)
        {
            _cellSize = cellSize;
            _hashTable = new Dictionary<long, List<KeyValuePair<PointL, T>>>();
        }

        public void Add(PointL point, T value)
        {
            long cellX = MathL.Floor(point.X, _cellSize) / _cellSize;
            long cellY = MathL.Floor(point.Y, _cellSize) / _cellSize;
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

        public List<KeyValuePair<PointL, T>> GetPointsInsideBox(RectL box)
        {
            List<KeyValuePair<PointL, T>> result = new List<KeyValuePair<PointL, T>>();
            long startX = (box.X / _cellSize);
            long endX = ((box.X + box.Width) / _cellSize);
            long startY = (box.Y / _cellSize);
            long endY = ((box.Y + box.Height) / _cellSize);

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
            return (x * 397) ^ y;
        }
    }
    
}

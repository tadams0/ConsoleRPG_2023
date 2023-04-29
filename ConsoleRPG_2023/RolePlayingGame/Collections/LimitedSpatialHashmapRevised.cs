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
    public class LimitedSpatialHashmapRevised<T>
    {
        private readonly long maximumWidth;
        private readonly long cellWidth;
        private readonly long cellHeight;
        private readonly Dictionary<long,List<T>> _hashTable;

        public LimitedSpatialHashmapRevised(long cellWidth, long cellHeight, long maximumWidth)
        {
            this.maximumWidth = maximumWidth;
            this.cellWidth = cellWidth;
            this.cellHeight = cellHeight;

            _hashTable = new Dictionary<long, List<T>>();
        }

        public void Add(PointL point, T value)
        {
            long cellX = MathL.Floor(point.X, cellWidth) / cellWidth;
            long cellY = MathL.Floor(point.Y, cellHeight) / cellHeight;
            long cellHash = Hash(cellX, cellY);

            if (!_hashTable.TryGetValue(cellHash, out var list))
            {
                list = new List<T>();
                _hashTable[cellHash] = list;
            }

            list.Add(value);
        }

        public void Add(long x, long y, T value)
        {
            long cellX = MathL.Floor(x, cellWidth) / cellWidth;
            long cellY = MathL.Floor(y, cellHeight) / cellHeight;
            long cellHash = Hash(cellX, cellY);

            if (!_hashTable.TryGetValue(cellHash, out var list))
            {
                list = new List<T>();
                _hashTable[cellHash] = list;
            }

            list.Add(value);
        }

        public List<T> GetDataInsideGridRect(PointL point)
        {
            return GetDataInsideGridRect(point.X, point.Y);
        }

        public List<T> GetDataInsideGridRect(long x, long y)
        {
            List<T> result = new List<T>();

            long cellX = MathL.Floor(x, cellWidth) / cellWidth;
            long cellY = MathL.Floor(y, cellHeight) / cellHeight;
            long cellHash = Hash(cellX, cellY);

            if (_hashTable.TryGetValue(cellHash, out var cell))
            {
                foreach (var kvp in cell)
                {
                    result.Add(kvp);
                }
            }

            return result;
        }

        public List<T> GetDataInsideGridRect(RectL gridLockedRect)
        {
            List<T> result = new List<T>();
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

        private long Hash(long x, long y)
        {
            return y * maximumWidth + x;
        }
    }
    
}

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace ConsoleRPG_2023.RolePlayingGame
{
    /// <summary>
    /// A reimplementation of the System.Drawing Rectangle class using the <see cref="long"/> datatype instead of the <see cref="int"/>.
    /// </summary>
    public struct RectL
    {

        public static RectL Union(RectL a, RectL b)
        {
            long x1 = Math.Min(a.X, b.X);
            long x2 = Math.Max(a.X + a.Width, b.X + b.Width);
            long y1 = Math.Min(a.Y, b.Y);
            long y2 = Math.Max(a.Y + a.Height, b.Y + b.Height);

            return new RectL(x1, y1, x2 - x1, y2 - y1);
        }

        public long X { get; set; }
        public long Y { get; set;  }
        public long Width { get; set; }
        public long Height { get; set; }

        public PointL Location
        {
            get { return new PointL(X, Y); }
        }

        public RectL(long x, long y, long width, long height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        public bool Contains(Point point)
        {
            return point.X >= X && point.X <= X + Width && point.Y >= Y && point.Y <= Y + Height;
        }

        public bool Contains(PointL point)
        {
            return point.X >= X && point.X <= X + Width && point.Y >= Y && point.Y <= Y + Height;
        }

        public bool Intersects(RectL other)
        {
            return (X < other.X + other.Width &&
                    X + Width > other.X &&
                    Y < other.Y + other.Height &&
                    Y + Height > other.Y);
        }

        public override string ToString()
        {
            return $"{X}, {Y}, {Width}, {Height}";
        }
    }
}

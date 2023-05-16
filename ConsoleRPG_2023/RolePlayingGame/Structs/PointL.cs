using System;
using System.Net.Security;

namespace ConsoleRPG_2023.RolePlayingGame
{
    /// <summary>
    /// Defines a vector which uses longs.
    /// </summary>
    public struct PointL
    {
        /// <summary>
        /// Defines an empty <see cref="PointL"/> with the values X: 0, Y: 0.
        /// </summary>
        public static PointL Empty { get; } = new PointL(0, 0);

        public static double Distance(PointL v1, PointL v2)
        {
            double dx = v1.x - v2.x;
            double dy = v1.y - v2.y;
            return Math.Sqrt(dx * dx + dy * dy);
        }

        public static long IntegerSqrt(long n)
        {
            if (n < 0)
            {
                throw new ArgumentException("Square root of negative number is not defined");
            }

            if (n == 0)
            {
                return 0;
            }

            long x = n;
            long y = (x + 1) / 2;
            while (y < x)
            {
                x = y;
                y = (x + n / x) / 2;
            }

            return x;
        }

        public long X 
        { 
            get { return x; } 
            set { x = value; }
        }

        public long Y
        {
            get { return y; }
            set { y = value; }
        }

        private long x;
        private long y;

        public PointL(long x, long y)
        {
            this.x = x;
            this.y = y;
        }

        public static PointL operator +(PointL a, PointL b)
        {
            return new PointL(a.x + b.x, a.y + b.y);
        }

        public static PointL operator -(PointL a, PointL b)
        {
            return new PointL(a.x - b.x, a.y - b.y);
        }

        public static PointL operator *(PointL a, long b)
        {
            return new PointL(a.x * b, a.y * b);
        }

        public static PointL operator *(long b, PointL a)
        {
            return a * b;
        }

        public static long Dot(PointL a, PointL b)
        {
            return a.x * b.x + a.y * b.y;
        }

        public long Magnitude()
        {
            return (long)Math.Sqrt(x * x + y * y);
        }

        public double Distance(PointL other)
        {
            double dx = x - other.x;
            double dy = y - other.y;
            return Math.Sqrt(dx * dx + dy * dy);
        }

        public void Normalize()
        {
            long magnitude = Magnitude();
            if (magnitude != 0)
            {
                x /= magnitude;
                y /= magnitude;
            }
        }

        public override string ToString()
        {
            return "(" + x + ", " + y + ")";
        }
    }
}

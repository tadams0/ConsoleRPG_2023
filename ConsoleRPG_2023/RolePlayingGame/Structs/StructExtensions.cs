using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleRPG_2023.RolePlayingGame
{
    public static class StructExtensions
    {

        /// <summary>
        /// Converts this <see cref="RectL"/> type to a <see cref="Rectf"/>. Note that this conversion can be lossy at high values.
        /// </summary>
        public static Rectf RectLToF(this RectL rect)
        {
            return new Rectf((float)rect.X, (float)rect.Y, (float)rect.Width, (float)rect.Height);
        }

        /// <summary>
        /// Converts this <see cref="PointL"/> type to the csDelaunay type <see cref="Vector2f"/>. Note that this conversion can be lossy at high values.
        /// </summary>
        public static Vector2f Vector2ToF(this PointL point)
        {
            return new Vector2f(point.X, point.Y);
        }

        /// <summary>
        /// Converts this <see cref="Vector2"/> type to the csDelaunay type <see cref="Vector2f"/>.
        /// </summary>
        public static Vector2f Vector2ToF(this Vector2 vector)
        {
            return new Vector2f(vector.X, vector.Y);
        }

        /// <summary>
        /// Converts this csDelaunay type <see cref="Vector2f"/> to the type <see cref="Vector2"/>.
        /// </summary>
        public static Vector2 VectorFTo2(this Vector2f vector)
        {
            return new Vector2(vector.x, vector.y);
        }
    }
}

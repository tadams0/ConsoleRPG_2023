using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleRPG_2023.RolePlayingGame
{
    public class Pair<T1, T2>
    {
        private static readonly StringBuilder builder = new StringBuilder();

        private T1 value1;
        private T2 value2;

        public Pair(T1 value1, T2 value2)
        {
            this.value1 = value1;
            this.value2 = value2;
        }

        public T1 Value1
        {
            get { return value1; }
        }

        public T2 Value2
        {
            get { return value2; }
        }

        public override string ToString()
        {
            builder.Clear();
            builder.Append('[');
            if (value1 != null)
            {
                builder.Append(value1.ToString());
            }
            builder.Append(", ");
            if (value2 != null)
            {
                builder.Append(value2.ToString());
            }
            builder.Append(']');
            return builder.ToString();
        }
    }
}

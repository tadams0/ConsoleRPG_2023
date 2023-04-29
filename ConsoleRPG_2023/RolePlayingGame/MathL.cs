using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleRPG_2023.RolePlayingGame
{
    public static class MathL
    {

        public static long Floor(long dividend, long divisor)
        {
            long remainder = dividend % divisor;

            if (remainder < 0)
            {
                remainder += divisor;
            }

            return dividend - remainder;
        }
    }
}

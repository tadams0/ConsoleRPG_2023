using csDelaunay;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleRPG_2023.RolePlayingGame.Structs
{
    public class WorleyNoiseBiomeReturn
    {

        public BiomeData BiomeData { get; set; }

        public float ClampedValue { get; set; }

        public float UnclampedValue { get; set; }

        public WorleyNoiseBiomeReturn(BiomeData biomeData, float clampedValue, float unclampedValue)
        {
            BiomeData = biomeData;
            ClampedValue = clampedValue;
            UnclampedValue = unclampedValue;
        }

    }
}

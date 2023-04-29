using ConsoleRPG_2023.RolePlayingGame.Maps;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace ConsoleRPG_2023.RolePlayingGame
{
    public struct BiomeData
    {
        /// <summary>
        /// The humidity of the region.
        /// </summary>
        public double Moisture { get; set; }

        /// <summary>
        /// The temperature or heat of the land. Low values are cold. High values are hot.
        /// </summary>
        public double Temperature { get; set; }

        /// <summary>
        /// How high the land is.
        /// </summary>
        public double Height { get; set; }

        /// <summary>
        /// How good the land is for growing and supporting life.
        /// </summary>
        public double Fertility { get; set; }

        public BiomeData(double moisture, double temperature, double height, double fertility)
        {
            this.Moisture = moisture;
            this.Temperature = temperature;
            this.Height = height;
            this.Fertility = fertility;
        }


        public BiomeType GetBiomeType()
        {
            if (Fertility < 18)
            {//corrupted lands.
                if (Temperature > 365)
                {
                    return BiomeType.DemonicHellscape;
                }
                else if (Temperature > 350)
                {
                    return BiomeType.CorruptedFirelands;
                }
                else if (Temperature > 330)
                {
                    return BiomeType.CorruptedDesert;
                }
                else if (Temperature < 310 && Temperature > 278 && Moisture > 70)
                {
                    return BiomeType.CorruptedForest;
                }
                else if (Temperature < 260 && Fertility < 14)
                {
                    return BiomeType.CorruptedTundra;
                }
                else if (Temperature > 255)
                {
                    return BiomeType.CorruptedPlains;
                }
            }

            if (Temperature < 273) //frozen 32 degrees F
            {
                if (Moisture > 70 && Height > 200 && Fertility > 70)
                {
                    return BiomeType.SnowyWoodland;
                }
                else if (Moisture > 60 && Height > 200)
                {
                    return BiomeType.SnowyPlains;
                }
            }
            else if (Temperature > 310) //100 degrees F
            {
                if (Moisture < 60 && Height > 600 && Temperature > 330)
                {
                    return BiomeType.Volcanic;
                }
                else if (Temperature > 340)
                {
                    return BiomeType.IntenseDesert;
                }
            }
            else if (Height < 676)
            {
                if (Moisture > 70 && Height > 200 && Fertility > 75)
                {
                    return BiomeType.DenseForest;
                }
                else if (Moisture > 60 && Height > 200 && Fertility > 50)
                {
                    return BiomeType.Forest;
                }
                else if (Moisture > 40 && Height > 200 && Fertility > 35)
                {
                    return BiomeType.Woodland;
                }
                else if (Moisture > 30 && Height > 200 && Fertility > 20)
                {
                    return BiomeType.Plains;
                }
            }

            if (Height > 900)
            {
                return BiomeType.ExtremeMountains;
            }
            else if (Height > 775)
            {
                return BiomeType.Mountains;
            }
            else if (Height > 675 && Temperature > 283)
            {
                return BiomeType.Highlands;
            }
            else if (Moisture < 45 && Temperature > 310)
            {
                return BiomeType.Desert;
            }
            else if (Temperature > 270 && Height < 200)
            {
                return BiomeType.Ocean;
            }
            else if (Temperature < 270 && Height < 200)
            {
                return BiomeType.FrozenOcean;
            }
            else if (Moisture > 70 && Fertility > 59 && Temperature > 300 && Height < 400)
            {
                return BiomeType.Swamp;
            }
            else if (Moisture > 70 && Fertility > 59 && Temperature > 300 && Height > 399)
            {
                return BiomeType.Bog;
            }
            else if (Moisture > 70 && Fertility > 90 && Temperature > 280 && Temperature < 310)
            {
                return BiomeType.FertileFarmLand;
            }

            return BiomeType.Plains;
        }

    }
}

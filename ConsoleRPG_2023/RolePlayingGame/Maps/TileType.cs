using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleRPG_2023.RolePlayingGame.Maps
{
    /// <summary>
    /// Defines the types of biomes or tile available.
    /// </summary>
    public enum TileType : ushort
    {
        None = 0,

        GrassMild = 1,
        Ice = 2,
        FineSand = 3,
        Snow = 4,
        Corrupted = 5,
        Beach = 6,
        Water = 7,
        MurkeyWater = 8,
        CorruptedWater = 9,


        MountainRock = 10,
        MountainsSnow = 11,
        MountainsPeakRock = 12,

        LeafPile = 41,
        GrassThick = 42,
        GrassSparse = 43,
        GrassTall = 44,
        IcyGrassMild = 45,
        GrassVibrant = 46,

        Mud = 55,

        HellRock = 100,
        DryMagmaStone = 101,
        Lava = 102,
        FireSand = 103,
        HeatStone = 104,

        CorruptedIce = 203,
        CorruptedGrassMild = 204,
        CorruptedGrassTall = 205,
        CorruptedIcyGrassMild = 206,
        CorruptedSand = 207,

        //Dungeon tile types
        Stone = 500,
        CarvedStone = 501,
    }
}

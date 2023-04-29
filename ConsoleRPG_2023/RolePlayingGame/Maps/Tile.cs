using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleRPG_2023.RolePlayingGame.Maps
{
    public class Tile
    {
        public byte Id { get; }
        public TileType TileType { get; set; } = TileType.GrassMild;

        public Tile(byte id) 
        { 
            Id = id;
        }

    }
}

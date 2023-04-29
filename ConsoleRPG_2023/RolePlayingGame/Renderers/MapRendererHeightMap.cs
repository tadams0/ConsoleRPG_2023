using ConsoleRPG_2023.RolePlayingGame.Maps;
using CustomConsoleColors;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleRPG_2023.RolePlayingGame.Renderers
{
    public class MapRendererHeightMap : MapRendererExtraColorRange
    {
        public MapRendererHeightMap(Map map, GameState gameState, GameSettings settings, bool useSettingsForWindowSize) 
            : base(map, gameState, settings, useSettingsForWindowSize)
        {
        }

        protected override string GetTileBackgroundColor(Tile tile, BiomeData data, ProceduralWorldMap map)
        {
            int range = (int)(map.MaxTemperature - map.MinHeight);
            int tileTemp = (int)(data.Height - map.MinHeight);
            int rgb = (int)((((double)tileTemp / range) * 255));
            rgb = (int)Math.Clamp(rgb, 0, 255);
            Color tileColor = Color.FromArgb(rgb, rgb, rgb);

            return VirtualConsoleSequenceBuilder.GetColorBackgroundSequence(tileColor);
        }

    }
}

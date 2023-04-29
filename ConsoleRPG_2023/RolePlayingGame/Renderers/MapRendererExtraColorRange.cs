using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using ConsoleRPG_2023.RolePlayingGame.Maps;
using ConsoleRPG_2023.RolePlayingGame.Menus;
using CustomConsoleColors;

namespace ConsoleRPG_2023.RolePlayingGame.Renderers
{
    public class MapRendererExtraColorRange : MapRenderer
    {
        private static readonly Dictionary<TileType, string> tileTypeToColor = new Dictionary<TileType, string>();

        static MapRendererExtraColorRange()
        {
            AddNewTileTypeColorMapping(TileType.Ice, Color.FromArgb(188, 232, 247));
            AddNewTileTypeColorMapping(TileType.Snow, Color.FromArgb(220, 220, 220));
            AddNewTileTypeColorMapping(TileType.LeafPile, Color.FromArgb(93, 189, 9));
            AddNewTileTypeColorMapping(TileType.MountainsSnow, Color.FromArgb(250, 250, 250));
            AddNewTileTypeColorMapping(TileType.MountainsPeakRock, Color.FromArgb(255, 255, 255));
            AddNewTileTypeColorMapping(TileType.Corrupted, Color.FromArgb(130, 40, 190));
            AddNewTileTypeColorMapping(TileType.CorruptedWater, Color.FromArgb(195, 145, 250));
            AddNewTileTypeColorMapping(TileType.CorruptedIce, Color.FromArgb(175, 153, 207));
            AddNewTileTypeColorMapping(TileType.CorruptedGrassMild, Color.FromArgb(214, 123, 232));
            AddNewTileTypeColorMapping(TileType.CorruptedGrassTall, Color.FromArgb(182, 97, 199));
            AddNewTileTypeColorMapping(TileType.CorruptedIcyGrassMild, Color.FromArgb(148, 112, 179));
            AddNewTileTypeColorMapping(TileType.Water, Color.FromArgb(5, 100, 255));
            AddNewTileTypeColorMapping(TileType.MurkeyWater, Color.FromArgb(93, 125, 50));
            AddNewTileTypeColorMapping(TileType.GrassTall, Color.FromArgb(52, 190, 2));
            AddNewTileTypeColorMapping(TileType.GrassThick, Color.FromArgb(40, 138, 5));
            AddNewTileTypeColorMapping(TileType.GrassSparse, Color.FromArgb(150, 180, 80));
            AddNewTileTypeColorMapping(TileType.GrassMild, Color.FromArgb(62, 200, 12));
            AddNewTileTypeColorMapping(TileType.GrassVibrant, Color.FromArgb(130, 255, 77));
            AddNewTileTypeColorMapping(TileType.Mud, Color.FromArgb(148, 192, 15));
            AddNewTileTypeColorMapping(TileType.MountainRock, Color.FromArgb(125, 125, 130));
            AddNewTileTypeColorMapping(TileType.FineSand, Color.FromArgb(232, 207, 130));
            AddNewTileTypeColorMapping(TileType.Beach, Color.FromArgb(252, 243, 184));
            AddNewTileTypeColorMapping(TileType.HellRock, Color.FromArgb(210, 54, 54));
            AddNewTileTypeColorMapping(TileType.IcyGrassMild, Color.FromArgb(157, 237, 200));
            AddNewTileTypeColorMapping(TileType.FireSand, Color.FromArgb(230, 166, 126));
            AddNewTileTypeColorMapping(TileType.DryMagmaStone, Color.FromArgb(120, 105, 100));
            AddNewTileTypeColorMapping(TileType.Lava, Color.FromArgb(255, 0, 0));
            AddNewTileTypeColorMapping(TileType.HeatStone, Color.FromArgb(189, 108, 81));
            AddNewTileTypeColorMapping(TileType.CorruptedSand, Color.FromArgb(176, 42, 165));
            AddNewTileTypeColorMapping(TileType.Stone, Color.FromArgb(105, 105, 110));
            AddNewTileTypeColorMapping(TileType.CarvedStone, Color.FromArgb(145, 145, 145));


            AddNewTileTypeColorMapping(TileType.None, Color.Black);

        }

        private static void AddNewTileTypeColorMapping(TileType tileType, Color color)
        {
            tileTypeToColor[tileType] = VirtualConsoleSequenceBuilder.GetColorBackgroundSequence(color);
        }

        public MapRendererExtraColorRange(Map map, GameState gameState, GameSettings settings, bool useSettingsForWindowSize)
            : base(map, gameState, settings, useSettingsForWindowSize)
        {
        }

        public override void DrawMapCenterOnCoordinates(long x, long y)
        {
            Color previousForegroundColor = ConsoleColorScheme.GetConsoleColor(Console.ForegroundColor, false);
            Color previousBackgroundColor = ConsoleColorScheme.GetConsoleColor(Console.BackgroundColor, false);

            int whiteSpaceNeeded = (Console.WindowWidth - ViewWidth);
            leftPadding = whiteSpaceNeeded / 2;
            rightPadding = whiteSpaceNeeded - leftPadding;

            //Save the view that was rendered for any other purpose.
            lastRenderView = GetRenderView(x, y);

            string leftPaddingString = new string(' ', leftPadding);
            string rightPaddingString = new string(' ', rightPadding);

            Color defaultColor = ConsoleColorScheme.GetConsoleColor(ConsoleColor.Gray, false);
            MapChunk currentChunk;
            Tile currentTile;
            List<MapObject> tileObjects;
            MapObject lastTileObject;
            string defaultForegroundColor = VirtualConsoleSequenceBuilder.GetColorForegroundSequence(defaultColor);
            string defaultBackgroundColor = VirtualConsoleSequenceBuilder.GetColorBackgroundSequence(defaultColor);
            string prevForegroundColor = defaultForegroundColor;
            string prevBackgroundColor = defaultBackgroundColor;
            string currentForegroundColor = prevForegroundColor;
            string currentBackgroundColor = prevBackgroundColor;

            string nullChunkBackgroundColor = tileTypeToColor[TileType.None];

            BiomeData data;
            bool usesBiomeData = map is ProceduralWorldMap;
            ProceduralWorldMap procMap = map as ProceduralWorldMap;

            string charToAdd = string.Empty;
            StringBuilder stringBuilder = new StringBuilder();

            for (long j = lastRenderView.Top; j < lastRenderView.Bottom; j++)
            {
                if (leftPadding > 0)
                {
                    RenderStretch(prevBackgroundColor, prevForegroundColor, stringBuilder.ToString());
                    stringBuilder.Clear();
                    RenderPadding(leftPaddingString);

                    prevForegroundColor = defaultForegroundColor;
                    prevBackgroundColor = defaultBackgroundColor;
                }
                for (long i = lastRenderView.Left; i < lastRenderView.Right; i++)
                {
                    charToAdd = defaultTileChar;
                    currentChunk = map.GetChunkAtWorldSpace(i, j);
                    if (currentChunk == null)
                    {
                        currentBackgroundColor = nullChunkBackgroundColor;
                    }
                    else
                    {
                        currentTile = currentChunk.GetTileAtWorldCoordinates(i, j);

                        if (usesBiomeData)
                        {
                            data = procMap.GetBiomeDataForTile(i, j);
                            currentBackgroundColor = GetTileBackgroundColor(currentTile, data, procMap);
                        }
                        else
                        {
                            currentBackgroundColor = GetTileBackgroundColor(currentTile);
                        }

                        tileObjects = currentChunk.GetAllObjectsAtWorldCoordinates(i, j);
                        if (tileObjects != null && tileObjects.Count > 0)
                        {
                            lastTileObject = tileObjects.Last();

                            MapObjectRenderer renderer = GetRenderer(lastTileObject);
                            currentForegroundColor = renderer.GetForegroundColor(lastTileObject, currentTile, gameState);
                            charToAdd = renderer.GetDisplayCharacter(lastTileObject, currentTile, gameState);
                        }
                    }
                    if (currentBackgroundColor != prevBackgroundColor)
                    {
                        //Add on to the stretch.
                        stringBuilder.Append(currentBackgroundColor);
                        prevBackgroundColor = currentBackgroundColor;
                    }
                    if (currentForegroundColor != prevForegroundColor)
                    {
                        //Add on to the stretch.
                        stringBuilder.Append(currentForegroundColor);
                        prevForegroundColor = currentForegroundColor;
                    }

                    stringBuilder.Append(charToAdd);
                }
                if (rightPadding > 0)
                {
                    RenderStretch(prevBackgroundColor, prevForegroundColor, stringBuilder.ToString());
                    stringBuilder.Clear();
                    RenderPadding(rightPaddingString);

                    prevForegroundColor = defaultForegroundColor;
                    prevBackgroundColor = defaultBackgroundColor;
                }
            }

            if (stringBuilder.Length > 0)
            {//Render the final stretch string if needed.
                RenderStretch(prevBackgroundColor, prevForegroundColor, stringBuilder.ToString());
            }

            //Restoring the previous colors.
            Console.Write(VirtualConsoleSequenceBuilder.GetColorBackgroundSequence(previousBackgroundColor));
            Console.Write(VirtualConsoleSequenceBuilder.GetColorForegroundSequence(previousForegroundColor));
        }

        protected virtual string GetTileBackgroundColor(Tile tile, BiomeData data, ProceduralWorldMap map)
        {
            return tileTypeToColor[tile.TileType];
        }

        protected virtual string GetTileBackgroundColor(Tile tile)
        {
            return tileTypeToColor[tile.TileType];
        }

        public void RenderStretch(string backgroundColor, string foregroundColor, string str)
        {
            Console.Write(backgroundColor + foregroundColor+ str);
        }

    }
}

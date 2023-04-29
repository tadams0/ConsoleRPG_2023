using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using ConsoleRPG_2023.RolePlayingGame.Maps;
using ConsoleRPG_2023.RolePlayingGame.Menus;

namespace ConsoleRPG_2023.RolePlayingGame.Renderers
{
    public class MapRendererLegacy : MapRenderer
    {

        /// <summary>
        /// If true, then for the size of a chunk, a bunch of black tiles will be rendered instead. Without this, misalignment may occur if an empty chunk appears on-screen.
        /// But without it, things would speed up.
        /// </summary>
        private bool renderEmptyChunks = true;

        public MapRendererLegacy(Map map, GameState gameState, GameSettings settings, bool useSettingsForWindowSize)
            : base(map, gameState, settings, useSettingsForWindowSize)
        {
        }

        public override void DrawMapCenterOnCoordinates(long x, long y)
        {
            ConsoleColor previousForegroundColor = Console.ForegroundColor;
            ConsoleColor previousBackgroundColor = Console.BackgroundColor;

            int whiteSpaceNeeded = (Console.WindowWidth - ViewWidth);
            leftPadding = whiteSpaceNeeded / 2;
            rightPadding = whiteSpaceNeeded - leftPadding;

            //Save the view that was rendered for any other purpose.
            lastRenderView = GetRenderView(x, y);

            string leftPaddingString = new string(' ', leftPadding);
            string rightPaddingString = new string(' ', rightPadding);

            MapChunk currentChunk;
            Tile currentTile;
            MapObject currentObj;
            ConsoleColor prevForegroundColor = ConsoleColor.Gray;
            ConsoleColor prevBackgroundColor = ConsoleColor.Gray;
            ConsoleColor currentForegroundColor = ConsoleColor.Gray;
            ConsoleColor currentBackgroundColor = ConsoleColor.Gray;
            string charToAdd = string.Empty;
            StringBuilder stringBuilder = new StringBuilder();

            for (long j = lastRenderView.Top; j < lastRenderView.Bottom; j++)
            {
                if (leftPadding > 0)
                {
                    RenderStretch(prevBackgroundColor, prevForegroundColor, stringBuilder.ToString());
                    stringBuilder.Clear();
                    RenderPadding(leftPaddingString);
                }
                for (long i = lastRenderView.Left; i < lastRenderView.Right; i++)
                {
                    charToAdd = defaultTileChar;
                    currentChunk = map.GetChunkAtWorldSpace(i, j);
                    if (currentChunk == null)
                    {
                        currentBackgroundColor = TileTypeToColor(TileType.None);
                    }
                    else
                    {
                        currentTile = currentChunk.GetTileAtWorldCoordinates(i, j);
                        currentBackgroundColor = TileTypeToColor(currentTile.TileType);

                        currentObj = currentChunk.GetObjectAtWorldCoordinates(i, j);
                        if (currentObj != null)
                        {
                            MapObjectRenderer renderer = GetRenderer(currentObj);
                            currentForegroundColor = renderer.GetDisplayColor(currentObj, currentTile, gameState);
                            charToAdd = renderer.GetDisplayCharacter(currentObj, currentTile, gameState);
                        }
                    }

                    if (currentBackgroundColor == prevBackgroundColor
                        && currentForegroundColor == prevForegroundColor)
                    {
                        //Add on to the stretch.
                        stringBuilder.Append(charToAdd);
                    }
                    else
                    {
                        //Render the old stretch
                        RenderStretch(prevBackgroundColor, prevForegroundColor, stringBuilder.ToString());

                        //Begin new stretch (the tile in the current iteration needs to be accounted for so it begins with a char).
                        stringBuilder.Clear();
                        stringBuilder.Append(charToAdd);

                        prevBackgroundColor = currentBackgroundColor;
                        prevForegroundColor = currentForegroundColor;
                    }
                }
                if (rightPadding > 0)
                {
                    RenderStretch(prevBackgroundColor, prevForegroundColor, stringBuilder.ToString());
                    stringBuilder.Clear();
                    RenderPadding(rightPaddingString);
                }
            }

            if (stringBuilder.Length > 0)
            {//Render the final stretch string if needed.
                RenderStretch(prevBackgroundColor, prevForegroundColor, stringBuilder.ToString());
            }

            //Restoring the previous colors.
            Console.ForegroundColor = previousForegroundColor;
            Console.BackgroundColor = previousBackgroundColor;
        }

        public void RenderStretch(ConsoleColor backgroundColor, ConsoleColor foregroundColor, string str)
        {
            Console.BackgroundColor = backgroundColor;
            if (!string.IsNullOrWhiteSpace(str))
            {//No need to swap the forground color out for a stretch that contains no objects / foreground.
                Console.ForegroundColor = foregroundColor;
            }
            Console.Write(str);
        }


    }
}

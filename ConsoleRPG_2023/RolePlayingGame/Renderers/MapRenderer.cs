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
    public class MapRenderer
    {
        protected static readonly string defaultTileChar = " ";
        protected static readonly ConsoleColor defaultTileForegroundColor = ConsoleColor.DarkGray;
        protected static readonly ConsoleColor paddingBackgroundColor = ConsoleColor.Black;

        /// <summary>
        /// A mapping of an object type, and the corresponding renderer to use.
        /// </summary>
        private static Dictionary<Type, MapObjectRenderer> typeToRenderer = new Dictionary<Type, MapObjectRenderer>();

        private static Dictionary<TileType, ConsoleColor> tileColorMapping = new Dictionary<TileType, ConsoleColor>();

        static MapRenderer()
        {
            GenerateTileColorMapping();
        }

        private static void GenerateTileColorMapping()
        {
            tileColorMapping[TileType.None] = ConsoleColor.Gray;
            tileColorMapping[TileType.Corrupted] = ConsoleColor.DarkMagenta;
            tileColorMapping[TileType.FineSand] = ConsoleColor.DarkYellow;
            tileColorMapping[TileType.GrassTall] = ConsoleColor.DarkGreen;
            tileColorMapping[TileType.GrassThick] = ConsoleColor.DarkGreen;
            tileColorMapping[TileType.GrassMild] = ConsoleColor.Green;
            tileColorMapping[TileType.Snow] = ConsoleColor.White;
            tileColorMapping[TileType.Beach] = ConsoleColor.Yellow;
            tileColorMapping[TileType.MountainRock] = ConsoleColor.DarkGray;
            tileColorMapping[TileType.MountainsSnow] = ConsoleColor.White;
            tileColorMapping[TileType.MountainsPeakRock] = ConsoleColor.Red; //TODO: Change back to white or some other color.
            tileColorMapping[TileType.Water] = ConsoleColor.DarkBlue;
        }

        public int ViewWidth { get; set; }
        public int ViewHeight { get; set; }

        /// <summary>
        /// Gets or sets the map that is set to be rendered.
        /// </summary>
        public Map Map
        {
            get { return map; }
            set { map = value; }
        }

        protected GameState gameState;
        protected GameSettings settings;

        /// <summary>
        /// The map that is being rendered.
        /// </summary>
        protected Map map;

        /// <summary>
        /// The view of the last map render. Used for calculation purposes of the view.
        /// <br/>For example: Going from map space to Console screen space.
        /// </summary>
        protected RenderView lastRenderView;

        //Padding used to center the map view if it's smaller than the console width.
        protected int leftPadding = 0;
        protected int rightPadding = 0;

        /// <summary>
        /// The default type to use if no renderer is found.
        /// </summary>
        protected Type defaultRenderType = typeof(MapObjectRenderer);

        protected bool useSettingsForWindowSize = false;

        public MapRenderer(Map map, GameState gameState, GameSettings settings, bool useSettingsForWindowSize)
        {
            this.map = map;
            this.gameState = gameState;
            this.settings = settings;
            this.useSettingsForWindowSize = useSettingsForWindowSize;

            if (useSettingsForWindowSize)
            {
                ViewHeight = settings.MapViewHeight;
                ViewWidth = settings.MapViewWidth;
            }

            if (typeToRenderer.Count == 0)
            {//Build the mapping if there are no entries.
                //We only need to build it once for all map renderers, so it's a static variable type.
                BuildTypeToRendererMapping();

                //Creating the default renderer.
                typeToRenderer[defaultRenderType] = new MapObjectRenderer();
            }
        }

        private void BuildTypeToRendererMapping()
        {
            var allAssemblyTypes = Assembly.GetExecutingAssembly().GetTypes();
            //checks the executing assembly (think .exe kinda) for all of its types. Then looks through (where) all those types and adds only the ones that are a class and have the parent of MapObjectRenderer.
            Type[] rendererTypes = allAssemblyTypes.Where(x => x.IsClass && x.IsSubclassOf(typeof(MapObjectRenderer))).ToArray();

            foreach (Type type in rendererTypes)
            {
                string capitalizedTypeName = type.Name.ToUpperInvariant();

                //This generates the new renderer instance from the type above.
                MapObjectRenderer newRenderer = (MapObjectRenderer)Activator.CreateInstance(type);

                //Complete the mapping.
                typeToRenderer[newRenderer.RenderedType] = newRenderer;
            }
        }

        public virtual PointL GetConsoleSpaceFromWorldSpace(long x, long y)
        {
            //0,0 of the console would be the left,top of the last render view.
            //long worldX = (lastRenderView.Left % ViewWidth) + x;
            //long worldY = (lastRenderView.Top % ViewHeight) + y;

            long worldX = lastRenderView.Left - x + leftPadding;
            long worldY = lastRenderView.Top - y;

            return new PointL(worldX, worldY);
        }

        public virtual PointL GetWorldSpaceFromConsoleSpace(long x, long y)
        {
            //0,0 of the console would be the left,top of the last render view.

            return new PointL(x + lastRenderView.Left - leftPadding, y + lastRenderView.Top);
        }

        public virtual Tile GetTileAtConsoleSpace(long x, long y)
        {
            PointL worldPosition = GetWorldSpaceFromConsoleSpace(x, y);
            return map.GetTileAtWorldSpace(worldPosition.X, worldPosition.Y);
        }

        /// <summary>
        /// Generates the expected render view for the given world x and world y coordinates.
        /// </summary>
        protected virtual RenderView GetRenderView(long x, long y)
        {
            if (useSettingsForWindowSize)
            {
                ViewHeight = settings.MapViewHeight;
                ViewWidth = settings.MapViewWidth;
            }

            long leftViewSide = x - ViewWidth / 2;
            long rightViewSide = x + ViewWidth / 2;
            long topViewSide = y - ViewHeight / 2;
            long bottomViewSide = y + ViewHeight / 2;

            return new RenderView(leftViewSide, rightViewSide, topViewSide, bottomViewSide);
        }

        public virtual void DrawMapCenterOnCoordinates(long x, long y)
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

            for (long j = lastRenderView.Top; j < lastRenderView.Bottom; j++)
            {
                RenderPadding(leftPaddingString);
                for (long i = lastRenderView.Left; i < lastRenderView.Right; i++)
                {
                    currentChunk = map.GetChunkAtWorldSpace(i, j);
                    if (currentChunk == null)
                    {
                        RenderTile();
                    }
                    else
                    {
                        currentTile = currentChunk.GetTileAtWorldCoordinates(i, j);
                        currentObj = currentChunk.GetObjectAtWorldCoordinates(i, j);
                        if (currentObj != null)
                        {
                            RenderObject(currentObj, currentTile);
                        }
                        else
                        {
                            RenderTile(currentTile);
                        }
                    }
                }
                RenderPadding(rightPaddingString);
            }

            //Restoring the previous colors.
            Console.ForegroundColor = previousForegroundColor;
            Console.BackgroundColor = previousBackgroundColor;
        }

        public virtual void RenderPadding(string paddingStr)
        {
            Console.BackgroundColor = paddingBackgroundColor;
            Console.Write(paddingStr);
        }

        public virtual void RenderTile()
        {
            Console.BackgroundColor = TileTypeToColor(TileType.None);
           // Console.ForegroundColor = defaultTileForegroundColor;
            Console.Write(defaultTileChar);
        }

        public virtual void RenderTile(Tile t)
        {
            Console.BackgroundColor = TileTypeToColor(t.TileType);
            //Console.ForegroundColor = defaultTileForegroundColor;
            Console.Write(defaultTileChar);
        }

        public virtual void RenderObject(MapObject obj, Tile t)
        {
            Console.BackgroundColor = TileTypeToColor(t.TileType);
            MapObjectRenderer renderer = GetRenderer(obj);
            Console.ForegroundColor = renderer.GetDisplayColor(obj, t, gameState);
            Console.Write(renderer.GetDisplayCharacter(obj, t, gameState));
        }

        /// <summary>
        /// Gets the renderer for the given object.
        /// </summary>
        /// <param name="obj">The object whose renderer will be returned.</param>
        protected MapObjectRenderer GetRenderer(MapObject obj)
        {
            Type type = obj.GetType();
            if (typeToRenderer.TryGetValue(type, out var renderer))
            {
                return renderer;
            }
            return typeToRenderer[defaultRenderType];
        }

        /// <summary>
        /// Returns the mapped color to the given biome type.
        /// </summary>
        protected ConsoleColor TileTypeToColor(TileType tileType)
        {
            return tileColorMapping[tileType];
        }

    }
}

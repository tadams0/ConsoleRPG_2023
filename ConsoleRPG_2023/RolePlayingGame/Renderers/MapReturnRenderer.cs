using ConsoleRPG_2023.RolePlayingGame.Dungeons;
using ConsoleRPG_2023.RolePlayingGame.Maps;
using ConsoleRPG_2023.RolePlayingGame.Text;
using CustomConsoleColors;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace ConsoleRPG_2023.RolePlayingGame.Renderers
{
    /// <summary>
    /// Defines a class speciallized to render the <see cref="MapReturnObj"/>.
    /// </summary>
    public class MapReturnRenderer : MapEntranceRenderer
    {
        public MapReturnRenderer() 
        {
            renderedType = typeof(MapReturnObj);
        }

    }
}

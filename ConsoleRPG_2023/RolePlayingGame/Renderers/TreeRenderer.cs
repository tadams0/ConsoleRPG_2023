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
    /// A <see cref="MapObjectRenderer"/> specialized for rendering <see cref="MapTree"/> instances.
    /// </summary>
    public class TreeRenderer : MapObjectRenderer
    {
        public static readonly string treeDisplayChar = "T";

        private static readonly Color defaultTreeColor = Color.ForestGreen;

        private Dictionary<TreeType, Color> treeColors = new Dictionary<TreeType, Color>();

        public TreeRenderer() 
        {
            renderedType = typeof(MapTree);

            treeColors[TreeType.Oak] = Color.FromArgb(132, 240, 43);
            treeColors[TreeType.Birch] = Color.FromArgb(247, 247, 171);
            treeColors[TreeType.Cedar] = Color.FromArgb(62, 242, 41);
            treeColors[TreeType.Redwood] = Color.FromArgb(242, 172, 73);
            treeColors[TreeType.Grately] = Color.FromArgb(227, 192, 127);
            treeColors[TreeType.Fir] = Color.FromArgb(115, 224, 65);
            treeColors[TreeType.Hell] = Color.FromArgb(237, 128, 50);
            treeColors[TreeType.Willow] = Color.FromArgb(23, 194, 77);
            treeColors[TreeType.Cypress] = Color.FromArgb(171, 217, 33);
            treeColors[TreeType.Ice] = Color.FromArgb(33, 180, 217);
            treeColors[TreeType.Corrupted] = Color.FromArgb(33, 66, 217);
            treeColors[TreeType.GreaterCorrupted] = Color.FromArgb(88, 27, 242);
        }

        public override string GetDisplayCharacter(MapObject obj, Tile tile, GameState state)
        {
            return treeDisplayChar;
        }

        public override ConsoleColor GetDisplayColor(MapObject obj, Tile tile, GameState state)
        {
            return ConsoleColor.Green;
        }

        public override string GetForegroundColor(MapObject obj, Tile tile, GameState state)
        {
            Color c;
            MapTree tree = (MapTree)obj;
            if (!treeColors.TryGetValue(tree.TreeType, out c))
            {
                c = defaultTreeColor;
            }
            return VirtualConsoleSequenceBuilder.GetColorForegroundSequence(c);
        }

    }
}

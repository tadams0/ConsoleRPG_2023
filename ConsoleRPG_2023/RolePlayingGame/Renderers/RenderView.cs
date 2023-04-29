namespace ConsoleRPG_2023.RolePlayingGame.Renderers
{
    /// <summary>
    /// Defines a struct containing 4 long types each representing one corner of a quadrant.
    /// </summary>
    public struct RenderView
    {
        public long Left;

        public long Right;

        public long Top;

        public long Bottom;

        public RenderView(long left, long right, long top, long bottom)
        {
            Left = left;
            Right = right;
            Top = top;
            Bottom = bottom;
        }

    }
}

namespace ConsoleRPG_2023.RolePlayingGame.Maps.MapObjects
{
    /// <summary>
    /// Defines an interface for updatable types within the map.
    /// </summary>
    public interface IUpdatable
    {

        /// <summary>
        /// Runs a single update on the instance.
        /// </summary>
        public void Update(GameState state, Map map);

    }
}

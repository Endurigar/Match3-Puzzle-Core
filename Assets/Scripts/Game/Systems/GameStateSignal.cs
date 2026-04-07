namespace Assets.Scripts.Game.Systems
{
    /// <summary>
    /// Signal sent when the game state changes (active/inactive).
    /// </summary>
    public class GameStateSignal
    {
        public bool IsGameActive { get; set; }
    }
}

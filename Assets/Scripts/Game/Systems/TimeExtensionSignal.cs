namespace Assets.Scripts.Game.Systems
{
    /// <summary>
    /// Signal sent to add time to the game timer.
    /// </summary>
    public class TimeExtensionSignal
    {
        public float TimeToAdd { get; set; }
    }
}

namespace Assets.Scripts.Game.Systems
{
    /// <summary>
    /// Signal sent when the player's score is updated.
    /// </summary>
    public class ScoreUpdatedSignal
    {
        /// <summary>
        /// Gets or sets the current score.
        /// </summary>
        public int CurrentScore { get; set; }
    }
}

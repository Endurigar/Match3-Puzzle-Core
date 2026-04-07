namespace Assets.Scripts.Game.Systems
{
    /// <summary>
    /// Signal sent when the high score is updated.
    /// </summary>
    public class HighScoreUpdatedSignal
    {
        /// <summary>
        /// Gets or sets the new high score.
        /// </summary>
        public int HighScore { get; set; }
    }
}
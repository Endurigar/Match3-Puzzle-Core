using Assets.Scripts.Game.Systems;
using Assets.Scripts.Game.UI.Base;
using TMPro;
using Zenject;

namespace Assets.Scripts.Game.UI
{
    /// <summary>
    /// UI component that displays the level's high score.
    /// Listens for HighScoreUpdatedSignal to update the display.
    /// </summary>
    public class HighScoreUI : TextSignalUI<HighScoreUpdatedSignal>
    {
        private HighScoreManager _highScoreManager;

        /// <summary>
        /// Injects the high score manager dependency.
        /// </summary>
        [Inject]
        public void Construct(HighScoreManager highScoreManager)
        {
            _highScoreManager = highScoreManager;
        }

        protected override void Start()
        {
            base.Start();
            var textField = GetComponent<TextMeshProUGUI>();
            UpdateText(new HighScoreUpdatedSignal { HighScore = _highScoreManager.HighScore }, textField);
        }

        /// <summary>
        /// Updates the text display when a high score signal is received.
        /// </summary>
        /// <param name="signal">The signal containing the new high score.</param>
        /// <param name="textField">The text component to update.</param>
        protected override void UpdateText(HighScoreUpdatedSignal signal, TextMeshProUGUI textField)
        {
            textField.text = $"Best: {signal.HighScore}";
        }
    }
}
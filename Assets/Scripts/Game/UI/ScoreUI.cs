using Assets.Scripts.Game.Systems;
using Assets.Scripts.Game.UI.Base;
using TMPro;

namespace Assets.Scripts.Game.UI
{
    /// <summary>
    /// UI component that displays the current game score.
    /// Listens for ScoreUpdatedSignal to update the display.
    /// </summary>
    public class ScoreUI : TextSignalUI<ScoreUpdatedSignal>
    {
        /// <summary>
        /// Updates the text display when a score update signal is received.
        /// </summary>
        /// <param name="signal">The signal containing the new score.</param>
        /// <param name="textField">The text component to update.</param>
        protected override void UpdateText(ScoreUpdatedSignal signal, TextMeshProUGUI textField)
        {
            textField.text = $"Score: {signal.CurrentScore}";
        }
    }
}
 village
using Assets.Scripts.Game.Systems;
using Assets.Scripts.Game.UI.Base;
using TMPro;
using UnityEngine;

namespace Assets.Scripts.Game.UI
{
    /// <summary>
    /// UI component that displays the remaining game time.
    /// Listens for TimerSignal to update the timer display in MM:SS format.
    /// </summary>
    public class TimerUI : TextSignalUI<TimerSignal>
    {
        /// <summary>
        /// Updates the timer text display when a timer signal is received.
        /// </summary>
        /// <param name="signal">The signal containing the remaining time.</param>
        /// <param name="textField">The text component to update.</param>
        protected override void UpdateText(TimerSignal signal, TextMeshProUGUI textField)
        {
            int minutes = Mathf.FloorToInt(signal.TimeLeft / 60f);
            int seconds = Mathf.FloorToInt(signal.TimeLeft % 60f);

            textField.text = $"Time: {minutes:00}:{seconds:00}";
        }
    }
}
 village
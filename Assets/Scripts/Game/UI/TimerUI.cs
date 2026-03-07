using Assets.Scripts.Game.Systems;
using Assets.Scripts.Game.UI.Base;
using TMPro;
using UnityEngine;

namespace Assets.Scripts.Game.UI
{
    public class TimerUI : TextSignalUI<TimerSignal>
    {
        protected override void UpdateText(TimerSignal signal, TextMeshProUGUI textField)
        {
            int minutes = Mathf.FloorToInt(signal.TimeLeft / 60f);
            int seconds = Mathf.FloorToInt(signal.TimeLeft % 60f);

            textField.text = $"Time: {minutes:00}:{seconds:00}";
        }
    }
}
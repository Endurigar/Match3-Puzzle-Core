using Assets.Scripts.Game.Systems;
using Assets.Scripts.Game.UI.Base;
using TMPro;

namespace Assets.Scripts.Game.UI
{
    public class ScoreUI : TextSignalUI<ScoreUpdatedSignal>
    {
        protected override void UpdateText(ScoreUpdatedSignal signal, TextMeshProUGUI textField)
        {
            textField.text = $"Score: {signal.CurrentScore}";
        }
    }
}
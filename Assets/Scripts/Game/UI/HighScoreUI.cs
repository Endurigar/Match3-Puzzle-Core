using Assets.Scripts.Game.Systems;
using Assets.Scripts.Game.UI.Base;
using TMPro;
using Zenject;

namespace Assets.Scripts.Game.UI
{
    public class HighScoreUI : TextSignalUI<HighScoreUpdatedSignal>
    {
        private HighScoreManager _highScoreManager;

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

        protected override void UpdateText(HighScoreUpdatedSignal signal, TextMeshProUGUI textField)
        {
            textField.text = $"Best: {signal.HighScore}";
        }
    }
}
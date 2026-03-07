using Zenject;

namespace Assets.Scripts.Game.Systems
{
    public class ScoreManager : IInitializable
    {
        private readonly SignalBus _signalBus;
        private readonly LevelData _levelData;

        private int _currentScore = 0;
        private const int TIME_EXTENSION_THRESHOLD = 50;
        private const float TIME_EXTENSION_AMOUNT = 5.0f;

        public int CurrentScore => _currentScore;

        public ScoreManager(SignalBus signalBus, LevelData levelData)
        {
            _signalBus = signalBus;
            _levelData = levelData;
        }

        public void Initialize()
        {
            _signalBus.Fire(new ScoreUpdatedSignal { CurrentScore = _currentScore });
        }

        public int GetStarCount()
        {
            if (_levelData.TargetScore <= 0) return 3;

            float percentage = (float)_currentScore / _levelData.TargetScore;

            if (percentage >= 1.0f) return 3;
            if (percentage >= 0.6f) return 2;
            if (percentage >= 0.3f) return 1;

            return 0;
        }

        public void AddScore(int points, int multiplier = 1)
        {
            int finalPoints = points * multiplier;
            _currentScore += finalPoints;

            if (_levelData.IsEndlessMode && _currentScore > 0 && _currentScore % TIME_EXTENSION_THRESHOLD == 0)
            {
                _signalBus.Fire(new TimeExtensionSignal { TimeToAdd = TIME_EXTENSION_AMOUNT });
            }

            _signalBus.Fire(new ScoreUpdatedSignal { CurrentScore = _currentScore });
        }

        public void ResetScore()
        {
            _currentScore = 0;
            _signalBus.Fire(new ScoreUpdatedSignal { CurrentScore = _currentScore });
        }
    }
}
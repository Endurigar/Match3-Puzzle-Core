using Zenject;

namespace Assets.Scripts.Game.Systems
{
    /// <summary>
    /// Manages the game score, including adding points, calculating star counts, and resetting the score.
    /// Fired ScoreUpdatedSignal on changes.
    /// </summary>
    public class ScoreManager : IInitializable
    {
        private readonly SignalBus _signalBus;
        private readonly LevelData _levelData;

        private int _currentScore = 0;
        private const int TIME_EXTENSION_THRESHOLD = 50;
        private const float TIME_EXTENSION_AMOUNT = 5.0f;

        /// <summary>
        /// Gets the current score of the game.
        /// </summary>
        public int CurrentScore => _currentScore;

        /// <summary>
        /// Initializes a new instance of the <see cref="ScoreManager"/> class.
        /// </summary>
        /// <param name="signalBus">The Zenject signal bus.</param>
        /// <param name="levelData">The current level data.</param>
        public ScoreManager(SignalBus signalBus, LevelData levelData)
        {
            _signalBus = signalBus;
            _levelData = levelData;
        }

        /// <summary>
        /// Initializes the score manager, firing the initial score update signal.
        /// </summary>
        public void Initialize()
        {
            _signalBus.Fire(new ScoreUpdatedSignal { CurrentScore = _currentScore });
        }

        /// <summary>
        /// Calculates the number of stars earned based on the current score and target score.
        /// </summary>
        /// <returns>A number from 0 to 3 representing the star count.</returns>
        public int GetStarCount()
        {
            if (_levelData.TargetScore <= 0) return 3;

            float percentage = (float)_currentScore / _levelData.TargetScore;

            if (percentage >= 1.0f) return 3;
            if (percentage >= 0.6f) return 2;
            if (percentage >= 0.3f) return 1;

            return 0;
        }

        /// <summary>
        /// Adds points to the current score, potentially applying a multiplier.
        /// Fires a time extension signal if thresholds are met in endless mode.
        /// </summary>
        /// <param name="points">The base points to add.</param>
        /// <param name="multiplier">The multiplier to apply to the points.</param>
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

        /// <summary>
        /// Resets the current score to zero and fires an update signal.
        /// </summary>
        public void ResetScore()
        {
            _currentScore = 0;
            _signalBus.Fire(new ScoreUpdatedSignal { CurrentScore = _currentScore });
        }
    }
}
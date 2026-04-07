using UnityEngine;
using Zenject;

namespace Assets.Scripts.Game.Systems
{
    public class HighScoreManager : IInitializable
    {
        private readonly LevelData _levelData;
        private readonly SignalBus _signalBus;

        private string _highScoreKey;
        private int _highScore;

        /// <summary>
        /// Gets the current high score for the level.
        /// </summary>
        public int HighScore => _highScore;

        /// <summary>
        /// Initializes a new instance of the HighScoreManager class.
        /// </summary>
        public HighScoreManager(LevelData levelData, SignalBus signalBus)
        {
            _levelData = levelData;
            _signalBus = signalBus;
        }

        /// <summary>
        /// Initializes the high score key and loads the initial score.
        /// </summary>
        public void Initialize()
        {
            _highScoreKey = $"HighScore_{_levelData.Name}";
            LoadHighScore();
        }

        /// <summary>
        /// Loads the high score from PlayerPrefs and fires an update signal.
        /// </summary>
        public void LoadHighScore()
        {
            _highScore = PlayerPrefs.GetInt(_highScoreKey, 0);
            _signalBus.Fire(new HighScoreUpdatedSignal { HighScore = _highScore });
            Debug.Log($"Loaded High Score for {_levelData.Name}: {_highScore}");
        }

        /// <summary>
        /// Saves a new score if it's higher than the current high score.
        /// </summary>
        /// <param name="currentScore">The score value to check and potentially save.</param>
        public void SaveHighScore(int currentScore)
        {
            if (currentScore > _highScore)
            {
                _highScore = currentScore;
                PlayerPrefs.SetInt(_highScoreKey, _highScore);
                PlayerPrefs.Save();
                Debug.Log($"New High Score for {_levelData.Name}: {_highScore}");
                _signalBus.Fire(new HighScoreUpdatedSignal { HighScore = _highScore });
            }
        }
    }
}
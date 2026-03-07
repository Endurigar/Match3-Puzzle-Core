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

        public int HighScore => _highScore;

        public HighScoreManager(LevelData levelData, SignalBus signalBus)
        {
            _levelData = levelData;
            _signalBus = signalBus;
        }

        public void Initialize()
        {
            _highScoreKey = $"HighScore_{_levelData.Name}";
            LoadHighScore();
        }

        public void LoadHighScore()
        {
            _highScore = PlayerPrefs.GetInt(_highScoreKey, 0);
            _signalBus.Fire(new HighScoreUpdatedSignal { HighScore = _highScore });
            Debug.Log($"Loaded High Score for {_levelData.Name}: {_highScore}");
        }

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
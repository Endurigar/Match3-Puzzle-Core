using UnityEngine;
using Zenject;

namespace Assets.Scripts.Game.Systems
{
    public class GameExitHandler : MonoBehaviour
    {
        [Inject] private ScoreManager _scoreManager;
        [Inject] private HighScoreManager _highScoreManager;

        private void OnApplicationQuit()
        {
            if (_highScoreManager != null && _scoreManager != null)
            {
                _highScoreManager.SaveHighScore(_scoreManager.CurrentScore);
            }
        }
    }
}
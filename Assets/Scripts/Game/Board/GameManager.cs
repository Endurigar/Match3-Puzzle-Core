using System;
using UnityEngine;
using Zenject;
using Assets.Scripts.Game.Systems;
using Assets.Scripts.Game.Core;

namespace Assets.Scripts.Game.Board
{
    public class GameManager : IInitializable, ITickable, IDisposable
    {
        private readonly LevelData _levelData;
        private readonly ScoreManager _scoreManager;
        private readonly BoardState _boardState;
        private readonly SignalBus _signalBus;
        private readonly HighScoreManager _highScoreManager;
        private readonly AudioManager _audioManager;
        private readonly LevelProgressManager _progressManager;

        private float _currentTime;
        private bool _isGameActive;

        public event Action OnGameWin;
        public event Action OnGameLose;

        /// <summary>
        /// Initializes a new instance of the GameManager class.
        /// </summary>
        public GameManager(LevelData levelData, ScoreManager scoreManager, BoardState boardState,
                           SignalBus signalBus, HighScoreManager highScoreManager, AudioManager audioManager,
                           LevelProgressManager progressManager)
        {
            _levelData = levelData;
            _scoreManager = scoreManager;
            _boardState = boardState;
            _signalBus = signalBus;
            _highScoreManager = highScoreManager;
            _audioManager = audioManager;
            _progressManager = progressManager;
        }

        /// <summary>
        /// Initializes the game session, starts the timer, and subscribes to events.
        /// </summary>
        public void Initialize()
        {
            GameFlow.StartGame();
            _currentTime = _levelData.TimeLimit;
            _isGameActive = true;

            _audioManager.PlayMusic(MusicType.Gameplay);

            _signalBus.Subscribe<TimeExtensionSignal>(OnTimeExtension);
            _signalBus.Subscribe<ScoreUpdatedSignal>(OnScoreUpdated);
            _signalBus.Fire(new GameStateSignal { IsGameActive = true });
        }

        /// <summary>
        /// Disposes of the game session, saves the high score, and unsubscribes from events.
        /// </summary>
        public void Dispose()
        {
            _isGameActive = false;
            GameFlow.StopGame();

            _highScoreManager.SaveHighScore(_scoreManager.CurrentScore);

            _signalBus.Unsubscribe<TimeExtensionSignal>(OnTimeExtension);
            _signalBus.Unsubscribe<ScoreUpdatedSignal>(OnScoreUpdated);
        }

        /// <summary>
        /// Updates the game timer every frame.
        /// </summary>
        public void Tick()
        {
            if (!_isGameActive) return;

            _currentTime -= Time.deltaTime;
            _signalBus.Fire(new TimerSignal { TimeLeft = _currentTime });

            if (_currentTime <= 0)
            {
                _currentTime = 0;
                EndGame();
            }
        }

        /// <summary>
        /// Restarts the game by resetting the score, timer, and board.
        /// </summary>
        public void RestartGame()
        {
            _scoreManager.ResetScore();
            GameFlow.StartGame();
            _isGameActive = true;
            _currentTime = _levelData.TimeLimit;
            _audioManager.PlayMusic(MusicType.Gameplay);
            _boardState.Initialize();
        }

        /// <summary>
        /// Callback for when the score is updated. Checks for win condition in non-endless mode.
        /// </summary>
        private void OnScoreUpdated(ScoreUpdatedSignal signal)
        {
            if (!_levelData.IsEndlessMode && _isGameActive)
            {
                if (signal.CurrentScore >= _levelData.TargetScore) EndGame();
            }
        }

        /// <summary>
        /// Callback for when a time extension is granted.
        /// </summary>
        private void OnTimeExtension(TimeExtensionSignal signal)
        {
            if (!_isGameActive) return;
            _currentTime += signal.TimeToAdd;
            _signalBus.Fire(new TimerSignal { TimeLeft = _currentTime });
        }

        /// <summary>
        /// Ends the current game session and triggers win/loss logic.
        /// </summary>
        private void EndGame()
        {
            _isGameActive = false;
            GameFlow.IsGameActive = false;

            _highScoreManager.SaveHighScore(_scoreManager.CurrentScore);
            _audioManager.StopMusic();

            if (_levelData.IsEndlessMode)
            {
                _audioManager.PlaySFX(SFXType.Lose);
                OnGameLose?.Invoke();
            }
            else
            {
                int stars = _scoreManager.GetStarCount();
                if (stars >= 1)
                {
                    HandleWin(stars);
                }
                else
                {
                    _audioManager.PlaySFX(SFXType.Lose);
                    OnGameLose?.Invoke();
                }
            }
            _signalBus.Fire(new GameStateSignal { IsGameActive = false });
        }

        /// <summary>
        /// Handles the win logic, including saving progress and unlocking the next level.
        /// </summary>
        private void HandleWin(int stars)
        {
            _audioManager.PlaySFX(SFXType.Win);
            Debug.Log($"Level Won! Stars: {stars}");

            _progressManager.SaveLevelStars(_levelData.Name, stars);

            if (_levelData.NextLevel != null)
            {
                _progressManager.UnlockLevel(_levelData.NextLevel.Name);
            }
            else
            {
                Debug.LogWarning("No Next Level assigned!");
            }

            OnGameWin?.Invoke();
        }
    }
}
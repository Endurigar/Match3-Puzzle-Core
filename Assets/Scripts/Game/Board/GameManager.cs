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

        public void Dispose()
        {
            _isGameActive = false;
            GameFlow.StopGame();

            _highScoreManager.SaveHighScore(_scoreManager.CurrentScore);

            _signalBus.Unsubscribe<TimeExtensionSignal>(OnTimeExtension);
            _signalBus.Unsubscribe<ScoreUpdatedSignal>(OnScoreUpdated);
        }

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

        public void RestartGame()
        {
            _scoreManager.ResetScore();
            GameFlow.StartGame();
            _isGameActive = true;
            _currentTime = _levelData.TimeLimit;
            _audioManager.PlayMusic(MusicType.Gameplay);
            _boardState.Initialize();
        }

        private void OnScoreUpdated(ScoreUpdatedSignal signal)
        {
            if (!_levelData.IsEndlessMode && _isGameActive)
            {
                if (signal.CurrentScore >= _levelData.TargetScore) EndGame();
            }
        }

        private void OnTimeExtension(TimeExtensionSignal signal)
        {
            if (!_isGameActive) return;
            _currentTime += signal.TimeToAdd;
            _signalBus.Fire(new TimerSignal { TimeLeft = _currentTime });
        }

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
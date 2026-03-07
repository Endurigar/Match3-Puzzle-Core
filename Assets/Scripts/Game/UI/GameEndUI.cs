using System.Collections.Generic;
using Assets.Scripts.Game.Board;
using Assets.Scripts.Game.Systems;
using Assets.Scripts.Game.Systems.Ads;
using Assets.Scripts.Game.UI.Base;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Zenject;

namespace Assets.Scripts.Game.UI
{
    public class GameEndUI : BaseMenu
    {
        [Header("Visuals")]
        [SerializeField] private List<Image> _starImages;
        [SerializeField] private Sprite _starFilled;
        [SerializeField] private Sprite _starEmpty;

        [Header("Buttons")]
        [SerializeField] private Button _nextLevelButton;
        [SerializeField] private Button _restartButton;
        [SerializeField] private Button _menuButton;

        [Header("Settings")]
        [SerializeField] private float _starAnimationDelay = 0.4f;

        private GameManager _gameManager;
        private ScoreManager _scoreManager;
        private AudioManager _audioManager;
        private IAdsService _adsService;

        [Inject]
        public void Construct(GameManager gameManager, ScoreManager scoreManager, AudioManager audioManager, IAdsService adsService)
        {
            _gameManager = gameManager;
            _scoreManager = scoreManager;
            _audioManager = audioManager;
            _adsService = adsService;
        }

        private void Start()
        {
            _gameManager.OnGameWin += OnGameWin;
            _gameManager.OnGameLose += OnGameLose;

            SetupButton(_restartButton, RestartGame);
            SetupButton(_menuButton, LoadMainMenu);
            SetupButton(_nextLevelButton, LoadNextLevel);

            HideMenuImmediate();
        }

        private void OnDestroy()
        {
            if (_gameManager != null)
            {
                _gameManager.OnGameWin -= OnGameWin;
                _gameManager.OnGameLose -= OnGameLose;
            }

            if (_starImages != null)
            {
                foreach (var star in _starImages)
                {
                    if (star != null)
                    {
                        star.transform.DOKill();
                    }
                }
            }
        }

        private void SetupButton(Button btn, UnityEngine.Events.UnityAction action)
        {
            if (btn != null) btn.onClick.AddListener(action);
        }

        private void OnGameWin()
        {
            HandleGameEnd(true);
        }

        private void OnGameLose()
        {
            HandleGameEnd(false);
        }

        private void HandleGameEnd(bool isWin)
        {
            _adsService.ShowBanner(true);
            ResetStars();
            ShowMenu(pauseTime: true);

            if (_restartButton) _restartButton.gameObject.SetActive(true);
            if (_menuButton) _menuButton.gameObject.SetActive(true);

            if (_nextLevelButton)
            {
                bool hasNext = isWin && LevelManager.Instance?.SelectedLevelData?.NextLevel != null;
                _nextLevelButton.gameObject.SetActive(hasNext);
            }

            if (isWin)
            {
                AnimateStars(_scoreManager.GetStarCount());
            }
        }

        private void LoadNextLevel()
        {
            PrepareForSceneChange();
            if (LevelManager.Instance?.SelectedLevelData?.NextLevel != null)
            {
                LevelManager.Instance.SelectLevel(LevelManager.Instance.SelectedLevelData.NextLevel);
                ReloadScene();
            }
        }

        private void RestartGame()
        {
            PrepareForSceneChange();
            ReloadScene();
        }

        private void LoadMainMenu()
        {
            PrepareForSceneChange();
            SceneManager.LoadScene("MainMenu");
        }

        private void PrepareForSceneChange()
        {
            _adsService.ShowBanner(false);
            _audioManager.PlaySFX(SFXType.Click);
            Time.timeScale = 1f;
        }

        private void ReloadScene()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        private void ResetStars()
        {
            foreach (var starImg in _starImages)
            {
                starImg.sprite = _starEmpty;
                starImg.transform.localScale = Vector3.one;
            }
        }

        private void AnimateStars(int count)
        {
            Sequence sequence = DOTween.Sequence().SetUpdate(true);
            sequence.AppendInterval(0.3f);

            for (int i = 0; i < count; i++)
            {
                if (i >= _starImages.Count) break;

                int index = i; // Local copy for closure
                sequence.AppendCallback(() =>
                {
                    var star = _starImages[index];
                    star.sprite = _starFilled;
                    _audioManager.PlaySFX(SFXType.Match);

                    star.transform.DOScale(1.5f, 0.2f)
                        .SetEase(Ease.OutBack)
                        .OnComplete(() => star.transform.DOScale(1f, 0.1f));
                });

                sequence.AppendInterval(_starAnimationDelay);
            }
        }
    }
}
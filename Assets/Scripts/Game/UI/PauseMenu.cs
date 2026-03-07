using Assets.Scripts.Game.Board;
using Assets.Scripts.Game.UI.Base;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Zenject;

namespace Assets.Scripts.Game.UI
{
    public class PauseMenu : BaseMenu
    {
        [Header("Buttons")]
        [SerializeField] private Button _resumeButton;
        [SerializeField] private Button _restartButton;
        [SerializeField] private Button _exitButton;
        [SerializeField] private Button _openPauseButton;

        private GameManager _gameManager;

        [Inject]
        public void Construct(GameManager gameManager)
        {
            _gameManager = gameManager;
        }

        private void Start()
        {
            if (_resumeButton) _resumeButton.onClick.AddListener(Resume);
            if (_restartButton) _restartButton.onClick.AddListener(Restart);
            if (_exitButton) _exitButton.onClick.AddListener(LoadMainMenu);
            if (_openPauseButton) _openPauseButton.onClick.AddListener(ShowPause);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (IsActive()) Resume();
                else ShowPause();
            }
        }

        private void ShowPause()
        {
            if (IsActive()) return;
            ShowMenu("PAUSED", pauseTime: true);
        }

        private void Resume()
        {
            HideMenu(resumeTime: true);
        }

        private void Restart()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        private void LoadMainMenu()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene("MainMenu");
        }
    }
}
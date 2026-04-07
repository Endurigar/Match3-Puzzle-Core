using Assets.Scripts.Game.Board;
using Assets.Scripts.Game.UI.Base;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Zenject;

namespace Assets.Scripts.Game.UI
{
    /// <summary>
    /// Controller for the pause menu UI.
    /// Handles pausing the game, resuming, restarting, and returning to the main menu.
    /// </summary>
    public class PauseMenu : BaseMenu
    {
        [Header("Buttons")]
        [SerializeField] private Button _resumeButton;
        [SerializeField] private Button _restartButton;
        [SerializeField] private Button _exitButton;
        [SerializeField] private Button _openPauseButton;

        private GameManager _gameManager;

        /// <summary>
        /// Injects the game manager dependency.
        /// </summary>
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

        /// <summary>
        /// Displays the pause menu and pauses the game time.
        /// </summary>
        private void ShowPause()
        {
            if (IsActive()) return;
            ShowMenu("PAUSED", pauseTime: true);
        }

        /// <summary>
        /// Hides the pause menu and resumes the game time.
        /// </summary>
        private void Resume()
        {
            HideMenu(resumeTime: true);
        }

        /// <summary>
        /// Restarts the current level and resumes the game time.
        /// </summary>
        private void Restart()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        /// <summary>
        /// Returns to the main menu scene and resumes the game time.
        /// </summary>
        private void LoadMainMenu()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene("MainMenu");
        }
    }
}
 village
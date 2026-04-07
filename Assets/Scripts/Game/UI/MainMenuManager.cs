using Assets.Scripts.Game.Systems;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Zenject;

namespace Assets.Scripts.Game.UI
{
    /// <summary>
    /// Manages the main menu UI, including level selection, endless mode, and exiting the game.
    /// Handles transitions between menu panels and triggers music playback.
    /// </summary>
    public class MainMenuManager : MonoBehaviour
    {
        [Header("Menus")]
        [SerializeField] private GameObject _mainMenuPanel;
        [SerializeField] private GameObject _levelSelectPanel;

        [Header("Buttons")]
        [SerializeField] private Button _playButton;
        [SerializeField] private Button _endlessButton;
        [SerializeField] private Button _exitButton;
        [SerializeField] private Button _backButton;

        [Header("Data")]
        [SerializeField] private LevelData _endlessLevelData;

        private AudioManager _audioManager;

        /// <summary>
        /// Injects the audio manager dependency.
        /// </summary>
        [Inject]
        public void Construct(AudioManager audioManager)
        {
            _audioManager = audioManager;
        }

        private void Awake()
        {
            Time.timeScale = 1f;
        }

        private void Start()
        {
            if (_playButton) _playButton.onClick.AddListener(OpenLevelSelect);
            if (_endlessButton) _endlessButton.onClick.AddListener(StartEndlessMode);
            if (_exitButton) _exitButton.onClick.AddListener(ExitGame);
            if (_backButton) _backButton.onClick.AddListener(BackToMain);

            _audioManager.PlayMusic(MusicType.MainMenu);
        }

        /// <summary>
        /// Opens the level selection panel and plays a click sound.
        /// </summary>
        private void OpenLevelSelect()
        {
            PlayClickSound();
            ToggleMenus(false);
        }

        /// <summary>
        /// Starts the game in endless mode using the configured level data.
        /// </summary>
        private void StartEndlessMode()
        {
            PlayClickSound();
            if (LevelManager.Instance != null && _endlessLevelData != null)
            {
                LevelManager.Instance.SelectLevel(_endlessLevelData);
                SceneManager.LoadScene("GameScene");
            }
            else
            {
                Debug.LogError("[MainMenu] LevelManager or EndlessLevelData is missing.");
            }
        }

        /// <summary>
        /// Returns to the main menu panel.
        /// </summary>
        private void BackToMain()
        {
            PlayClickSound();
            ToggleMenus(true);
        }

        /// <summary>
        /// Toggles between the main menu and level selection panels.
        /// </summary>
        /// <param name="showMain">True to show the main menu, false for level select.</param>
        private void ToggleMenus(bool showMain)
        {
            if (_mainMenuPanel) _mainMenuPanel.SetActive(showMain);
            if (_levelSelectPanel) _levelSelectPanel.SetActive(!showMain);
        }

        /// <summary>
        /// Exits the application.
        /// </summary>
        private void ExitGame()
        {
            Application.Quit();
        }

        /// <summary>
        /// Plays the generic click sound effect.
        /// </summary>
        private void PlayClickSound()
        {
            _audioManager.PlaySFX(SFXType.Click);
        }
    }
}
 village
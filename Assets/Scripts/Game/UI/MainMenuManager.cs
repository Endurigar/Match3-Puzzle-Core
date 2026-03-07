using Assets.Scripts.Game.Systems;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Zenject;

namespace Assets.Scripts.Game.UI
{
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

        private void OpenLevelSelect()
        {
            PlayClickSound();
            ToggleMenus(false);
        }

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

        private void BackToMain()
        {
            PlayClickSound();
            ToggleMenus(true);
        }

        private void ToggleMenus(bool showMain)
        {
            if (_mainMenuPanel) _mainMenuPanel.SetActive(showMain);
            if (_levelSelectPanel) _levelSelectPanel.SetActive(!showMain);
        }

        private void ExitGame()
        {
            Application.Quit();
        }

        private void PlayClickSound()
        {
            _audioManager.PlaySFX(SFXType.Click);
        }
    }
}
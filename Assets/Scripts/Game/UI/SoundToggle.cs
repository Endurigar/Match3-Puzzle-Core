using Assets.Scripts.Game.Systems;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Assets.Scripts.Game.UI
{
    /// <summary>
    /// UI component that handles toggling music and sound effects (SFX) on and off.
    /// Updates its icons based on the current mute state in the AudioManager.
    /// </summary>
    public class SoundToggle : MonoBehaviour
    {
        [Header("Music UI")]
        [SerializeField] private Button _musicButton;
        [SerializeField] private Image _musicIcon;
        [SerializeField] private Sprite _musicOnSprite;
        [SerializeField] private Sprite _musicOffSprite;

        [Header("SFX UI")]
        [SerializeField] private Button _sfxButton;
        [SerializeField] private Image _sfxIcon;
        [SerializeField] private Sprite _sfxOnSprite;
        [SerializeField] private Sprite _sfxOffSprite;

        private AudioManager _audioManager;

        /// <summary>
        /// Injects the audio manager dependency.
        /// </summary>
        [Inject]
        public void Construct(AudioManager audioManager)
        {
            _audioManager = audioManager;
        }

        private void Start()
        {
            _musicButton.onClick.AddListener(ToggleMusic);
            _sfxButton.onClick.AddListener(ToggleSFX);
            UpdateVisuals();
        }

        private void OnEnable()
        {
            if (_audioManager != null) UpdateVisuals();
        }

        /// <summary>
        /// Toggles the music mute state and updates the UI visuals.
        /// </summary>
        private void ToggleMusic()
        {
            _audioManager.ToggleMusic();
            _audioManager.PlaySFX(SFXType.Click);
            UpdateVisuals();
        }

        /// <summary>
        /// Toggles the SFX mute state and updates the UI visuals.
        /// </summary>
        private void ToggleSFX()
        {
            _audioManager.ToggleSFX();
            if (!_audioManager.IsSFXMuted)
            {
                _audioManager.PlaySFX(SFXType.Click);
            }
            UpdateVisuals();
        }

        /// <summary>
        /// Updates the music and SFX icons based on their current mute status.
        /// </summary>
        private void UpdateVisuals()
        {
            if (_audioManager == null) return;

            if (_musicIcon)
                _musicIcon.sprite = _audioManager.IsMusicMuted ? _musicOffSprite : _musicOnSprite;

            if (_sfxIcon)
                _sfxIcon.sprite = _audioManager.IsSFXMuted ? _sfxOffSprite : _sfxOnSprite;
        }
    }
}
 village
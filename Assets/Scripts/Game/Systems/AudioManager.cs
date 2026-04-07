using DG.Tweening;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.Game.Systems
{
    public class AudioManager : MonoBehaviour
    {
        /// <summary>
        /// Gets the singleton instance of the AudioManager.
        /// </summary>
        public static AudioManager Instance { get; private set; }

        /// <summary>
        /// Gets whether the music is currently muted.
        /// </summary>
        public bool IsMusicMuted { get; private set; }

        /// <summary>
        /// Gets whether the sound effects are currently muted.
        /// </summary>
        public bool IsSFXMuted { get; private set; }

        /// <summary>
        /// Injects the audio library dependency.
        /// </summary>
        [Inject]
        public void Construct(AudioLibrary audioLibrary)
        {
            _audioLibrary = audioLibrary;
            ApplySettings();
        }

        /// <summary>
        /// Initializes the singleton instance and loads audio settings.
        /// </summary>
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);

                LoadSettingsStateOnly();

                if (_musicSource == null)
                {
                    _musicSource = gameObject.AddComponent<AudioSource>();
                    _musicSource.loop = true;
                    _musicSource.playOnAwake = false;
                }

                if (_sfxSource == null)
                {
                    _sfxSource = gameObject.AddComponent<AudioSource>();
                    _sfxSource.loop = false;
                    _sfxSource.playOnAwake = false;
                }

                ApplySettings();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        /// <summary>
        /// Loads the muted state for music and SFX from PlayerPrefs.
        /// </summary>
        private void LoadSettingsStateOnly()
        {
            IsMusicMuted = PlayerPrefs.GetInt(MUSIC_MUTE_KEY, 0) == 1;
            IsSFXMuted = PlayerPrefs.GetInt(SFX_MUTE_KEY, 0) == 1;
        }

        /// <summary>
        /// Applies the current mute settings to the audio sources.
        /// </summary>
        private void ApplySettings()
        {
            if (_musicSource != null) _musicSource.mute = IsMusicMuted;
            if (_sfxSource != null) _sfxSource.mute = IsSFXMuted;
        }

        /// <summary>
        /// Toggles the music mute state and saves the setting.
        /// </summary>
        public void ToggleMusic()
        {
            IsMusicMuted = !IsMusicMuted;
            if (_musicSource != null) _musicSource.mute = IsMusicMuted;
            PlayerPrefs.SetInt(MUSIC_MUTE_KEY, IsMusicMuted ? 1 : 0);
            PlayerPrefs.Save();
        }

        /// <summary>
        /// Toggles the SFX mute state and saves the setting.
        /// </summary>
        public void ToggleSFX()
        {
            IsSFXMuted = !IsSFXMuted;
            if (_sfxSource != null) _sfxSource.mute = IsSFXMuted;
            PlayerPrefs.SetInt(SFX_MUTE_KEY, IsSFXMuted ? 1 : 0);
            PlayerPrefs.Save();
        }

        /// <summary>
        /// Plays a sound effect of the specified type.
        /// </summary>
        /// <param name="type">The type of sound effect to play.</param>
        public void PlaySFX(SFXType type)
        {
            if (IsSFXMuted || _sfxSource == null || _audioLibrary == null) return;

            var data = _audioLibrary.GetSFX(type);
            if (data != null && data.Clip != null)
            {
                _sfxSource.pitch = 1f;
                _sfxSource.PlayOneShot(data.Clip, data.Volume);
            }
        }

        /// <summary>
        /// Plays the specified music track, cross-fading from the current one.
        /// </summary>
        /// <param name="type">The type of music to play.</param>
        public void PlayMusic(MusicType type)
        {
            if (_musicSource == null || _audioLibrary == null) return;

            var data = _audioLibrary.GetMusic(type);
            if (data == null || data.Clip == null) return;

            if (_musicSource.clip == data.Clip && _musicSource.isPlaying) return;

            _musicSource.DOKill();
            _musicSource.DOFade(0f, 0.5f).SetUpdate(true).OnComplete(() =>
            {
                if (_musicSource == null) return;
                _musicSource.clip = data.Clip;
                _musicSource.volume = data.Volume;
                _musicSource.loop = true;
                _musicSource.Play();
                _musicSource.DOFade(data.Volume, 0.5f).SetUpdate(true);
            });
        }

        /// <summary>
        /// Stops the currently playing music track with a fade-out effect.
        /// </summary>
        public void StopMusic()
        {
            if (_musicSource == null) return;
            _musicSource.DOFade(0f, 0.5f).SetUpdate(true).OnComplete(() =>
            {
                if (_musicSource != null) _musicSource.Stop();
            });
        }
    }
}
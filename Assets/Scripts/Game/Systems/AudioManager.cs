using DG.Tweening;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.Game.Systems
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }

        [Header("Sources")]
        [SerializeField] private AudioSource _musicSource;
        [SerializeField] private AudioSource _sfxSource;

        private AudioLibrary _audioLibrary;

        private const string MUSIC_MUTE_KEY = "MusicMuted";
        private const string SFX_MUTE_KEY = "SFXMuted";

        public bool IsMusicMuted { get; private set; }
        public bool IsSFXMuted { get; private set; }

        [Inject]
        public void Construct(AudioLibrary audioLibrary)
        {
            _audioLibrary = audioLibrary;
            ApplySettings();
        }

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

        private void LoadSettingsStateOnly()
        {
            IsMusicMuted = PlayerPrefs.GetInt(MUSIC_MUTE_KEY, 0) == 1;
            IsSFXMuted = PlayerPrefs.GetInt(SFX_MUTE_KEY, 0) == 1;
        }

        private void ApplySettings()
        {
            if (_musicSource != null) _musicSource.mute = IsMusicMuted;
            if (_sfxSource != null) _sfxSource.mute = IsSFXMuted;
        }

        public void ToggleMusic()
        {
            IsMusicMuted = !IsMusicMuted;
            if (_musicSource != null) _musicSource.mute = IsMusicMuted;
            PlayerPrefs.SetInt(MUSIC_MUTE_KEY, IsMusicMuted ? 1 : 0);
            PlayerPrefs.Save();
        }

        public void ToggleSFX()
        {
            IsSFXMuted = !IsSFXMuted;
            if (_sfxSource != null) _sfxSource.mute = IsSFXMuted;
            PlayerPrefs.SetInt(SFX_MUTE_KEY, IsSFXMuted ? 1 : 0);
            PlayerPrefs.Save();
        }

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
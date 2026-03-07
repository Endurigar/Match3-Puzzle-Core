using Assets.Scripts.Game.Systems;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.Game.Core
{
    public class MenuInstaller : MonoInstaller
    {
        [Header("References")]
        [SerializeField] private AudioManager _audioManager;

        public override void InstallBindings()
        {
            Container.Bind<LevelProgressManager>().AsSingle();
            BindAudio();
        }

        private void BindAudio()
        {
            if (AudioManager.Instance != null)
            {
                if (_audioManager != null && _audioManager != AudioManager.Instance)
                {
                    Destroy(_audioManager.gameObject);
                }
                Container.Bind<AudioManager>().FromInstance(AudioManager.Instance).AsSingle();
            }
            else
            {
                Container.Bind<AudioManager>().FromInstance(_audioManager).AsSingle();
                Container.QueueForInject(_audioManager);
            }
        }
    }
}
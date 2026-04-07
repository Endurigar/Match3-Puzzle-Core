using Assets.Scripts.Game.Board;
using Assets.Scripts.Game.Entities;
using Assets.Scripts.Game.Systems;
using Assets.Scripts.Game.Systems.Ads;
using Assets.Scripts.Game.UI;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.Game.Core
{
    public class CoreInstaller : MonoInstaller
    {
        [Header("Scene References")]
        [SerializeField] private Transform _boardHolder;
        [SerializeField] private VFXManager _vfxManager;
        [SerializeField] private AudioManager _audioManager;
        [SerializeField] private FloatingTextManager _floatingTextManager;
        [SerializeField] private CameraEffectManager _cameraEffectManager;

        /// <summary>
        /// Installs all dependencies for the core game scene.
        /// </summary>
        public override void InstallBindings()
        {
            BindLevelData();
            BindSignals();
            BindSystems();
            BindUI();
            BindSceneComponents();
            BindAudio();
        }

        /// <summary>
        /// Binds the level data to be used by the board and game manager.
        /// Defaults to "DefaultLevelData" if no level is selected.
        /// </summary>
        private void BindLevelData()
        {
            var selectedLevel = LevelManager.Instance != null ? LevelManager.Instance.SelectedLevelData : null;
            if (selectedLevel == null)
            {
                selectedLevel = Resources.Load<LevelData>("DefaultLevelData");
            }

            Container.Bind<LevelData>().FromInstance(selectedLevel).AsSingle();
        }

        /// <summary>
        /// Configures the Zenject signal bus and declares all necessary signals.
        /// </summary>
        private void BindSignals()
        {
            SignalBusInstaller.Install(Container);

            Container.DeclareSignal<TimerSignal>();
            Container.DeclareSignal<GameStateSignal>();
            Container.DeclareSignal<TimeExtensionSignal>();
            Container.DeclareSignal<HighScoreUpdatedSignal>();
            Container.DeclareSignal<ScoreUpdatedSignal>();
        }

        /// <summary>
        /// Binds all core game systems and logic controllers.
        /// </summary>
        private void BindSystems()
        {
            Container.BindInterfacesAndSelfTo<GameManager>().AsSingle();
            Container.BindInterfacesAndSelfTo<BoardState>().AsSingle();
            Container.BindInterfacesAndSelfTo<ScoreManager>().AsSingle();
            Container.BindInterfacesAndSelfTo<HighScoreManager>().AsSingle();
            Container.BindInterfacesAndSelfTo<LevelProgressManager>().AsSingle();

            Container.BindInterfacesAndSelfTo<SwapHandler>().AsSingle();
            Container.BindInterfacesAndSelfTo<PowerupProcessor>().AsSingle();
            Container.BindInterfacesAndSelfTo<HintSystem>().AsSingle();
            Container.BindInterfacesAndSelfTo<MatchAnimator>().AsSingle();

            Container.BindInterfacesAndSelfTo<UnityAdsService>().AsSingle().NonLazy();
        }

        /// <summary>
        /// Binds UI components found in the scene hierarchy.
        /// </summary>
        private void BindUI()
        {
            Container.Bind<TimerUI>().FromComponentInHierarchy().AsSingle();
            Container.Bind<HighScoreUI>().FromComponentInHierarchy().AsSingle();
            Container.Bind<ScoreUI>().FromComponentInHierarchy().AsSingle();
            Container.Bind<PauseMenu>().FromComponentInHierarchy().AsSingle();
            Container.Bind<GameEndUI>().FromComponentInHierarchy().AsSingle();
        }

        /// <summary>
        /// Binds visual and scene-specific components like managers and holders.
        /// </summary>
        private void BindSceneComponents()
        {
            Container.Bind<Transform>().WithId("Holder").FromInstance(_boardHolder).AsSingle();
            Container.Bind<VFXManager>().FromInstance(_vfxManager).AsSingle();
            Container.Bind<FloatingTextManager>().FromInstance(_floatingTextManager).AsSingle();
            Container.Bind<CameraEffectManager>().FromInstance(_cameraEffectManager).AsSingle();

            Container.Bind<GemPoolManager>().FromNewComponentOnNewGameObject().AsSingle().NonLazy();
        }

        /// <summary>
        /// Configures the audio manager binding, handling both singleton and scene-local instances.
        /// </summary>
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
using System.Collections.Generic;
using Assets.Scripts.Game.Systems;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.Game.UI
{
    /// <summary>
    /// Spawns level selection buttons based on a list of level data.
    /// Handles first level unlocking and dependency injection for the buttons.
    /// </summary>
    public class LevelButtonsSpawner : MonoBehaviour
    {
        [SerializeField] private GameObject _levelButtonPrefab;
        [SerializeField] private Transform _container;
        [SerializeField] private List<LevelData> _levels;

        private DiContainer _diContainer;
        private LevelProgressManager _progressManager;

        /// <summary>
        /// Injects dependencies for spawner and progress management.
        /// </summary>
        [Inject]
        public void Construct(DiContainer diContainer, LevelProgressManager progressManager)
        {
            _diContainer = diContainer;
            _progressManager = progressManager;
        }

        private void Start()
        {
            if (_levels != null && _levels.Count > 0)
            {
                _progressManager.UnlockLevel(_levels[0].Name);
            }

            SpawnButtons();
        }

        /// <summary>
        /// Instantiates level button prefabs into the specified container.
        /// </summary>
        private void SpawnButtons()
        {
            if (_levels == null) return;

            foreach (var level in _levels)
            {
                var buttonObj = _diContainer.InstantiatePrefab(_levelButtonPrefab, _container);

                if (buttonObj.TryGetComponent(out LevelMenu menuScript))
                {
                    menuScript.SetLevelInfo(level);
                }
            }
        }
    }
}
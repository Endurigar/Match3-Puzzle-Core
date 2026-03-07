using System.Collections.Generic;
using Assets.Scripts.Game.Systems;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.Game.UI
{
    public class LevelButtonsSpawner : MonoBehaviour
    {
        [SerializeField] private GameObject _levelButtonPrefab;
        [SerializeField] private Transform _container;
        [SerializeField] private List<LevelData> _levels;

        private DiContainer _diContainer;
        private LevelProgressManager _progressManager;

        [Inject]
        public void Construct(DiContainer diContainer, LevelProgressManager progressManager)
        {
            _diContainer = diContainer;
            _progressManager = progressManager;
        }

        private void Start()
        {
            if (_levels.Count > 0)
            {
                _progressManager.UnlockLevel(_levels[0].Name);
            }

            SpawnButtons();
        }

        private void SpawnButtons()
        {
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
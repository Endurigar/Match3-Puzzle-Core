using Assets.Scripts.Game.Board;
using Assets.Scripts.Game.Entities;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using Zenject;

namespace Assets.Scripts.Game.Systems
{
    public class GemPoolManager : MonoBehaviour
    {
        [SerializeField] private int _defaultCapacity = 20;
        [SerializeField] private int _maxPoolSize = 100;

        private GemsScriptableObject _gemsData;
        private Transform _holder;

        private readonly Dictionary<GemType, ObjectPool<GemController>> _gemPools = new();
        private readonly Dictionary<BombType, ObjectPool<BombController>> _bombPools = new();

        private void Awake()
        {
            GameObject poolRoot = new GameObject("GemPool_Inactive");
            poolRoot.transform.SetParent(transform);
            _holder = poolRoot.transform;
        }

        [Inject]
        public void Construct(GemsScriptableObject gemsData)
        {
            _gemsData = gemsData;
            InitializePools();
        }

        private void InitializePools()
        {
            if (_gemsData == null)
            {
                Debug.LogError("[GemPoolManager] GemsData is missing!");
                return;
            }

            foreach (GemType type in System.Enum.GetValues(typeof(GemType)))
            {
                var prefab = _gemsData.GetGemByType(type);
                if (prefab != null)
                {
                    _gemPools[type] = CreatePool(prefab);
                }
            }

            foreach (BombType type in System.Enum.GetValues(typeof(BombType)))
            {
                var prefab = _gemsData.GetBombByType(type);
                if (prefab != null)
                {
                    _bombPools[type] = CreatePool(prefab);
                }
            }
        }

        private ObjectPool<T> CreatePool<T>(T prefab) where T : BoardEntity
        {
            return new ObjectPool<T>(
                createFunc: () => Instantiate(prefab, _holder),
                actionOnGet: (item) => item.gameObject.SetActive(true),
                actionOnRelease: (item) =>
                {
                    item.gameObject.SetActive(false);
                    item.transform.SetParent(_holder);
                    item.transform.localScale = Vector3.one;
                },
                actionOnDestroy: (item) => Destroy(item.gameObject),
                collectionCheck: false,
                defaultCapacity: _defaultCapacity,
                maxSize: _maxPoolSize
            );
        }

        public GemController GetGem(GemType type)
        {
            if (_gemPools.TryGetValue(type, out var pool))
            {
                return pool.Get();
            }
            Debug.LogError($"Pool for GemType {type} not found!");
            return null;
        }

        public BombController GetBomb(BombType type)
        {
            if (_bombPools.TryGetValue(type, out var pool))
            {
                return pool.Get();
            }
            Debug.LogError($"Pool for BombType {type} not found!");
            return null;
        }

        public void Release(BoardEntity entity)
        {
            if (entity == null) return;

            if (entity is GemController gem)
            {
                if (_gemPools.TryGetValue(gem.GemType, out var pool))
                {
                    pool.Release(gem);
                    return;
                }
            }
            else if (entity is BombController bomb)
            {
                if (_bombPools.TryGetValue(bomb.BombType, out var pool))
                {
                    pool.Release(bomb);
                    return;
                }
            }

            Destroy(entity.gameObject);
        }
    }
}
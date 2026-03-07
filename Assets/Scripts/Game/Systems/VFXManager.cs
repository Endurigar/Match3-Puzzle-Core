using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Assets.Scripts.Game.Entities;
using UnityEngine;
using UnityEngine.Pool;

namespace Assets.Scripts.Game.Systems
{
    public class VFXManager : MonoBehaviour
    {
        [Header("Prefabs")]
        [SerializeField] private ParticleSystem _genericBombExplosion;
        [SerializeField] private ParticleSystem _lineBlastEffect;
        [SerializeField] private ParticleSystem _colorBombExplosion;
        [SerializeField] private ParticleSystem _gemDestroyEffect;

        [Header("Hint Settings")]
        [SerializeField] private ParticleSystem _hintEffectPrefab;

        [Header("Durations (sec)")]
        [SerializeField] private float _gemDestroyDuration = 0.4f;
        [SerializeField] private float _bonusActivationDuration = 0.9f;
        [SerializeField] private float _colorBombDuration = 1.1f;

        private readonly Dictionary<ParticleSystem, ObjectPool<ParticleSystem>> _pools = new();
        private ObjectPool<ParticleSystem> _hintPool;

        private int _activeEffects = 0;

        public static VFXManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;

            RegisterPool(_genericBombExplosion);
            RegisterPool(_lineBlastEffect);
            RegisterPool(_colorBombExplosion);
            RegisterPool(_gemDestroyEffect);

            if (_hintEffectPrefab != null)
            {
                _hintPool = new ObjectPool<ParticleSystem>(
                    createFunc: () => Instantiate(_hintEffectPrefab, transform),
                    actionOnGet: ps => ps.gameObject.SetActive(true),
                    actionOnRelease: ps => ps.gameObject.SetActive(false),
                    actionOnDestroy: ps => Destroy(ps.gameObject),
                    collectionCheck: false,
                    defaultCapacity: 2,
                    maxSize: 5
                );
            }
        }

        // --- Hint System Methods ---

        public ParticleSystem PlayHintLoop(Vector3 position)
        {
            if (_hintPool == null) return null;

            var instance = _hintPool.Get();
            instance.transform.position = position;
            instance.Play(true);

            return instance;
        }

        public void StopHintLoop(ParticleSystem instance)
        {
            if (instance == null || _hintPool == null) return;

            instance.Stop(true, ParticleSystemStopBehavior.StopEmitting);
            _hintPool.Release(instance);
        }

        public void RestartHintLoop(ParticleSystem instance)
        {
            if (instance == null) return;
            instance.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            instance.Play(true);
        }

        // --- Main Game Effects ---

        private Task PlayBombEffectInternal(Vector3 position, BombType type)
        {
            ParticleSystem prefab = type switch
            {
                BombType.VerticalBomb or BombType.HorizontalBomb => _lineBlastEffect,
                BombType.ColorBomb => _colorBombExplosion,
                _ => _genericBombExplosion
            };

            float duration = type switch
            {
                BombType.ColorBomb => _colorBombDuration,
                _ => _bonusActivationDuration
            };

            return PlayAsync(prefab, position, duration, true);
        }

        public Task PlayActivationEffectAsync(Vector3 position, BombType type) => PlayBombEffectInternal(position, type);
        public Task PlayBonusDestroyAsync(Vector3 position, BombType type) => PlayBombEffectInternal(position, type);
        public void PlayActivationEffect(Vector3 position, BombType type) => _ = PlayActivationEffectAsync(position, type);
        public void PlayBonusDestroy(Vector3 position, BombType type) => _ = PlayBonusDestroyAsync(position, type);

        public Task PlayGemDestroyAsync(Vector3 position)
        {
            return PlayAsync(_gemDestroyEffect, position, _gemDestroyDuration, false);
        }

        public void PlayGemDestroy(Vector3 position) => _ = PlayGemDestroyAsync(position);

        private Task PlayAsync(ParticleSystem prefab, Vector3 position, float duration, bool applyScale)
        {
            if (prefab == null || !_pools.ContainsKey(prefab))
                return Task.CompletedTask;

            var pool = _pools[prefab];
            var instance = pool.Get();

            instance.transform.SetParent(transform, false);
            instance.transform.position = position;
            instance.transform.localScale = applyScale ? Vector3.one * 1.3f : Vector3.one;

            instance.gameObject.SetActive(true);
            instance.Clear(true);
            instance.Play(true);

            _activeEffects++;

            var tcs = new TaskCompletionSource<object>();
            StartCoroutine(ReleaseAfter(instance, pool, Mathf.Max(0.05f, duration), tcs));
            return tcs.Task;
        }

        private IEnumerator ReleaseAfter(ParticleSystem instance, ObjectPool<ParticleSystem> pool, float delay, TaskCompletionSource<object> tcs)
        {
            yield return new WaitForSeconds(delay);
            if (instance != null)
                pool.Release(instance);

            _activeEffects = Mathf.Max(0, _activeEffects - 1);
            tcs.TrySetResult(null);
        }

        private void RegisterPool(ParticleSystem prefab)
        {
            if (prefab == null || _pools.ContainsKey(prefab)) return;

            var pool = new ObjectPool<ParticleSystem>(
                () => Instantiate(prefab, transform),
                ps => ps.gameObject.SetActive(true),
                ps => ps.gameObject.SetActive(false),
                ps => Destroy(ps.gameObject),
                false, 4, 30
            );

            _pools[prefab] = pool;
        }

        public bool IsAnyVFXPlaying() => _activeEffects > 0;

        public async Task WaitForAllVFXToEnd()
        {
            while (_activeEffects > 0)
                await Task.Yield();
        }
    }
}
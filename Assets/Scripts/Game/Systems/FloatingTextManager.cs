using UnityEngine;
using UnityEngine.Pool;

namespace Assets.Scripts.Game.Systems
{
    public class FloatingTextManager : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private FloatingTextController _textPrefab;
        [SerializeField] private int _defaultPoolSize = 20;
        [SerializeField] private int _maxPoolSize = 100;

        [Header("Colors")]
        [SerializeField] private Color _matchColor = Color.white;
        [SerializeField] private Color _bonusColor = Color.yellow;
        [SerializeField] private Color _comboColor = Color.red;

        private ObjectPool<FloatingTextController> _pool;

        private void Awake()
        {
            _pool = new ObjectPool<FloatingTextController>(
                createFunc: () => {
                    var instance = Instantiate(_textPrefab, transform);
                    instance.SetPool(_pool);
                    return instance;
                },
                actionOnGet: (item) => item.gameObject.SetActive(true),
                actionOnRelease: (item) => item.gameObject.SetActive(false),
                actionOnDestroy: (item) => Destroy(item.gameObject),
                collectionCheck: false,
                defaultCapacity: _defaultPoolSize,
                maxSize: _maxPoolSize
            );
        }

        public void ShowMatchScore(Vector3 position, int score)
        {
            var instance = _pool.Get();
            instance.Initialize(score.ToString(), position, _matchColor);
        }

        public void ShowBonusScore(Vector3 position, int score)
        {
            var instance = _pool.Get();
            instance.Initialize(score.ToString(), position, _bonusColor);
        }

        public void ShowComboText(Vector3 position, int multiplier)
        {
            var instance = _pool.Get();
            instance.Initialize($"COMBO x{multiplier}!", position + Vector3.up * 0.8f, _comboColor);
        }
    }
}
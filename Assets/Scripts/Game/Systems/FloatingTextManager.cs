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

        /// <summary>
        /// Initializes the object pool for floating text instances.
        /// </summary>
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

        /// <summary>
        /// Displays the score for a match at the specified position.
        /// </summary>
        /// <param name="position">The position to show the text at.</param>
        /// <param name="score">The score value.</param>
        public void ShowMatchScore(Vector3 position, int score)
        {
            var instance = _pool.Get();
            instance.Initialize(score.ToString(), position, _matchColor);
        }

        /// <summary>
        /// Displays the score for a bonus (powerup) at the specified position.
        /// </summary>
        /// <param name="position">The position to show the text at.</param>
        /// <param name="score">The score value.</param>
        public void ShowBonusScore(Vector3 position, int score)
        {
            var instance = _pool.Get();
            instance.Initialize(score.ToString(), position, _bonusColor);
        }

        /// <summary>
        /// Displays combo text for consecutive matches.
        /// </summary>
        /// <param name="position">The position to show the text at.</param>
        /// <param name="multiplier">The combo multiplier value.</param>
        public void ShowComboText(Vector3 position, int multiplier)
        {
            var instance = _pool.Get();
            instance.Initialize($"COMBO x{multiplier}!", position + Vector3.up * 0.8f, _comboColor);
        }
    }
}
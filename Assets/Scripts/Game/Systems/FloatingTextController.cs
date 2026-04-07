using UnityEngine;
using TMPro;
using DG.Tweening;
using UnityEngine.Pool;

namespace Assets.Scripts.Game.Systems
{
    public class FloatingTextController : MonoBehaviour
    {
        [SerializeField] private TextMeshPro _textComponent;
        [SerializeField] private float _floatDistance = 1.5f;
        [SerializeField] private float _duration = 0.8f;
        [SerializeField] private Ease _easeType = Ease.OutQuad;

        private IObjectPool<FloatingTextController> _pool;

        /// <summary>
        /// Sets the object pool reference for this controller.
        /// </summary>
        /// <param name="pool">The object pool instance.</param>
        public void SetPool(IObjectPool<FloatingTextController> pool)
        {
            _pool = pool;
        }

        /// <summary>
        /// Initializes the floating text with content, position, and color, and starts the animation.
        /// </summary>
        /// <param name="text">The text to display.</param>
        /// <param name="position">The starting position.</param>
        /// <param name="color">The text color.</param>
        public void Initialize(string text, Vector3 position, Color color)
        {
            transform.position = position;
            transform.localScale = Vector3.one;
            _textComponent.text = text;
            _textComponent.color = color;
            _textComponent.alpha = 1f;

            AnimateAndRelease();
        }

        /// <summary>
        /// Animates the text upwards and fades it out, then releases it back to the pool.
        /// </summary>
        private void AnimateAndRelease()
        {
            transform.DOKill();
            _textComponent.DOKill();

            Sequence sequence = DOTween.Sequence();
            sequence.Append(transform.DOMoveY(transform.position.y + _floatDistance, _duration).SetEase(_easeType));
            sequence.Join(_textComponent.DOFade(0f, _duration * 0.5f).SetDelay(_duration * 0.5f));

            sequence.OnComplete(() => _pool.Release(this));
        }
    }
}
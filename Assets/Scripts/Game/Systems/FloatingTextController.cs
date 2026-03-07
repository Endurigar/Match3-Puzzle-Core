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

        public void SetPool(IObjectPool<FloatingTextController> pool)
        {
            _pool = pool;
        }

        public void Initialize(string text, Vector3 position, Color color)
        {
            transform.position = position;
            transform.localScale = Vector3.one;
            _textComponent.text = text;
            _textComponent.color = color;
            _textComponent.alpha = 1f;

            AnimateAndRelease();
        }

        private void AnimateAndRelease()
        {
            transform.DOKill();
            _textComponent.DOKill();

            Sequence sequence = DOTween.Sequence();
            // Move up
            sequence.Append(transform.DOMoveY(transform.position.y + _floatDistance, _duration).SetEase(_easeType));
            // Fade out
            sequence.Join(_textComponent.DOFade(0f, _duration * 0.5f).SetDelay(_duration * 0.5f));

            sequence.OnComplete(() => _pool.Release(this));
        }
    }
}
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Scripts.Game.UI.Effects
{
    public class UIButtonAnimator : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
    {
        [Header("Settings")]
        [SerializeField] private float _hoverScale = 1.1f;
        [SerializeField] private float _pressScale = 0.95f;
        [SerializeField] private float _duration = 0.15f;

        private Vector3 _defaultScale;
        private Tween _currentTween;

        private void Awake()
        {
            _defaultScale = transform.localScale;
        }

        private void OnDisable()
        {
            _currentTween?.Kill();
            transform.localScale = _defaultScale;
        }

        private void AnimateTo(Vector3 targetScale, Ease ease)
        {
            _currentTween?.Kill();
            _currentTween = transform.DOScale(targetScale, _duration)
                .SetEase(ease)
                .SetUpdate(true);
        }

        public void OnPointerEnter(PointerEventData eventData) => AnimateTo(_defaultScale * _hoverScale, Ease.OutBack);
        public void OnPointerExit(PointerEventData eventData) => AnimateTo(_defaultScale, Ease.OutBack);
        public void OnPointerDown(PointerEventData eventData) => AnimateTo(_defaultScale * _pressScale, Ease.InOutSine);
        public void OnPointerUp(PointerEventData eventData) => AnimateTo(_defaultScale * _hoverScale, Ease.OutBack);
    }
}
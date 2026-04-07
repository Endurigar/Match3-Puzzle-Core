using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Scripts.Game.UI.Effects
{
    /// <summary>
    /// Provides simple scale-based animations for UI buttons on hover, click, and exit.
    /// Uses DOTween for smooth transitions.
    /// </summary>
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

        /// <summary>
        /// Animates the object's scale to the targets scale.
        /// </summary>
        /// <param name="targetScale">The scale to animate to.</param>
        /// <param name="ease">The easing function to use.</param>
        private void AnimateTo(Vector3 targetScale, Ease ease)
        {
            _currentTween?.Kill();
            _currentTween = transform.DOScale(targetScale, _duration)
                .SetEase(ease)
                .SetUpdate(true);
        }

        /// <summary>
        /// Triggered when the pointer enters the button's area.
        /// </summary>
        public void OnPointerEnter(PointerEventData eventData) => AnimateTo(_defaultScale * _hoverScale, Ease.OutBack);
        
        /// <summary>
        /// Triggered when the pointer exits the button's area.
        /// </summary>
        public void OnPointerExit(PointerEventData eventData) => AnimateTo(_defaultScale, Ease.OutBack);
        
        /// <summary>
        /// Triggered when the pointer is pressed down over the button.
        /// </summary>
        public void OnPointerDown(PointerEventData eventData) => AnimateTo(_defaultScale * _pressScale, Ease.InOutSine);
        
        /// <summary>
        /// Triggered when the pointer is released over the button.
        /// </summary>
        public void OnPointerUp(PointerEventData eventData) => AnimateTo(_defaultScale * _hoverScale, Ease.OutBack);
    }
}
 village
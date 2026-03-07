using DG.Tweening;
using TMPro;
using UnityEngine;

namespace Assets.Scripts.Game.UI.Base
{
    [RequireComponent(typeof(CanvasGroup))]
    public abstract class BaseMenu : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private GameObject _menuUI;
        [SerializeField] private TextMeshProUGUI _statusText;

        [Header("Animation Settings")]
        [SerializeField] protected float _fadeDuration = 0.5f;

        private CanvasGroup _canvasGroup;
        private bool _isActive;
        protected bool _isTransitioning;

        protected virtual void Awake()
        {
            if (_menuUI != null && !_menuUI.TryGetComponent(out _canvasGroup))
            {
                _canvasGroup = GetComponent<CanvasGroup>();
            }
            else if (_canvasGroup == null)
            {
                _canvasGroup = GetComponent<CanvasGroup>();
            }

            HideMenuImmediate();
        }

        protected void ShowMenu(string statusMessage = null, bool pauseTime = true)
        {
            if (_isTransitioning) return;

            _isTransitioning = true;
            _isActive = true;

            if (_menuUI != null) _menuUI.SetActive(true);
            UpdateStatusText(statusMessage, true);

            if (pauseTime) Time.timeScale = 0f;

            if (_canvasGroup != null)
            {
                _canvasGroup.DOKill();
                _canvasGroup.interactable = true;
                _canvasGroup.blocksRaycasts = true;
                _canvasGroup.alpha = 0f;

                _canvasGroup.DOFade(1f, _fadeDuration)
                    .SetEase(Ease.OutSine)
                    .SetUpdate(true)
                    .OnComplete(() => _isTransitioning = false);
            }
            else
            {
                _isTransitioning = false;
            }

            OnMenuShown();
        }

        protected void HideMenu(bool resumeTime = true)
        {
            if (_isTransitioning) return;

            _isTransitioning = true;
            _isActive = false;

            if (_canvasGroup != null)
            {
                _canvasGroup.DOKill();
                _canvasGroup.interactable = false;
                _canvasGroup.blocksRaycasts = true;

                _canvasGroup.DOFade(0f, _fadeDuration)
                    .SetEase(Ease.InSine)
                    .SetUpdate(true)
                    .OnComplete(() =>
                    {
                        _canvasGroup.blocksRaycasts = false;
                        DisableUI(resumeTime);
                    });
            }
            else
            {
                DisableUI(resumeTime);
            }

            OnMenuHidden();
        }

        protected void HideMenuImmediate()
        {
            _isTransitioning = false;
            _isActive = false;

            if (_canvasGroup != null)
            {
                _canvasGroup.DOKill();
                _canvasGroup.alpha = 0f;
                _canvasGroup.interactable = false;
                _canvasGroup.blocksRaycasts = false;
            }

            if (_menuUI != null) _menuUI.SetActive(false);
            if (_statusText != null) _statusText.gameObject.SetActive(false);
        }

        private void DisableUI(bool resumeTime)
        {
            if (_menuUI != null) _menuUI.SetActive(false);
            if (_statusText != null) _statusText.gameObject.SetActive(false);
            if (resumeTime) Time.timeScale = 1f;

            _isTransitioning = false;
        }

        private void UpdateStatusText(string message, bool isVisible)
        {
            if (_statusText == null) return;

            _statusText.gameObject.SetActive(isVisible && !string.IsNullOrEmpty(message));
            if (!string.IsNullOrEmpty(message))
            {
                _statusText.text = message;
            }
        }

        protected bool IsActive() => _isActive || _isTransitioning;

        protected virtual void OnMenuShown() { }
        protected virtual void OnMenuHidden() { }
    }
}
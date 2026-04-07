using DG.Tweening;
using UnityEngine;

namespace Assets.Scripts.Game.Systems
{
    public class CameraEffectManager : MonoBehaviour
    {
        [SerializeField] private Camera _mainCamera;

        [Header("Color Bomb Shake Settings")]
        [SerializeField] private float _shakeDuration = 0.5f;
        [SerializeField] private float _shakeStrength = 0.5f;
        [SerializeField] private int _vibrato = 10;
        [SerializeField] private float _randomness = 90f;

        private Vector3 _originalPosition;

        /// <summary>
        /// Initializes the camera reference and stores its original position.
        /// </summary>
        private void Start()
        {
            if (_mainCamera == null) _mainCamera = Camera.main;
            _originalPosition = _mainCamera.transform.position;
        }

        /// <summary>
        /// Shakes the camera using the default settings configured in the inspector.
        /// </summary>
        public void ShakeCamera()
        {
            _mainCamera.transform.DOKill(complete: true);

            _mainCamera.transform.DOShakePosition(_shakeDuration, _shakeStrength, _vibrato, _randomness);
        }

        /// <summary>
        /// Shakes the camera with the specified duration and strength.
        /// </summary>
        /// <param name="duration">Duration of the shake in seconds.</param>
        /// <param name="strength">Strength of the shake.</param>
        public void ShakeCamera(float duration, float strength)
        {
            _mainCamera.transform.DOKill(complete: true);
            _mainCamera.transform.DOShakePosition(duration, strength, _vibrato, _randomness);
        }
    }
}
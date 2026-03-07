using Assets.Scripts.Game.Systems;
using System.Collections;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.Game.Board
{
    public class BoardAutoFitter : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Camera _cam;
        [SerializeField] private Transform _boardRoot;

        [Header("Settings")]
        [SerializeField] private float _cellSize = 1f;
        [SerializeField] private float _cellSpacing = 0f;
        [SerializeField] private Vector2 _paddingInCells = new Vector2(0.5f, 0.5f);
        [SerializeField] private bool _recalcOnResolutionChange = true;
        [SerializeField] private float _recalcInterval = 0.25f;

        private LevelData _levelData;
        private int _lastWidth, _lastHeight;

        [Inject]
        public void Construct(LevelData levelData)
        {
            _levelData = levelData;
        }

        private void Reset() => _cam = Camera.main;

        private void Start()
        {
            FitNow();
            if (_recalcOnResolutionChange) StartCoroutine(WatchResolution());
        }

        public void FitNow()
        {
            if (_cam == null || _levelData == null) return;

            if (!_cam.orthographic) _cam.orthographic = true;

            int columns = _levelData.Width;
            int rows = _levelData.Height;

            float step = _cellSize + _cellSpacing;
            float boardWidth = columns * step;
            float boardHeight = rows * step;

            Vector2 padWorld = _paddingInCells * step;
            float halfW = boardWidth * 0.5f + padWorld.x;
            float halfH = boardHeight * 0.5f + padWorld.y;

            float aspect = (float)Screen.width / Screen.height;
            _cam.orthographicSize = Mathf.Max(halfH, halfW / aspect);

            Vector3 origin = _boardRoot ? _boardRoot.position : Vector3.zero;
            Vector3 center = origin + new Vector3((columns - 1) * step * 0.5f, (rows - 1) * step * 0.5f, 0f);

            _cam.transform.position = new Vector3(center.x, center.y, _cam.transform.position.z);

            _lastWidth = Screen.width;
            _lastHeight = Screen.height;
        }

        private IEnumerator WatchResolution()
        {
            while (true)
            {
                if (Screen.width != _lastWidth || Screen.height != _lastHeight)
                    FitNow();

                yield return new WaitForSeconds(_recalcInterval);
            }
        }
    }
}
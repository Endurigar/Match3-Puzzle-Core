using Assets.Scripts.Game.Board;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.Game.Systems
{
    public class InputController : MonoBehaviour
    {
        [SerializeField] private float _swipeThreshold = 50f;

        private Vector2 _startTouchPosition;
        private BoardEntity _selectedGem;
        private SwapHandler _swapHandler;

        [Inject]
        public void Construct(SwapHandler swapHandler)
        {
            _swapHandler = swapHandler;
        }

        private void Update()
        {
            if (!GameFlow.IsGameActive) return;

#if UNITY_EDITOR || UNITY_STANDALONE
            HandleMouseInput();
#else
            HandleTouchInput();
#endif
        }

        private void HandleMouseInput()
        {
            // 1. Press: store gem and position
            if (Input.GetMouseButtonDown(0))
            {
                var hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
                if (hit.collider != null)
                {
                    _selectedGem = hit.collider.GetComponent<BoardEntity>();
                    _startTouchPosition = Input.mousePosition;
                }
            }
            // 2. Hold: check swipe
            else if (Input.GetMouseButton(0) && _selectedGem != null)
            {
                CheckSwipe(Input.mousePosition);
            }
            // 3. Release: reset
            else if (Input.GetMouseButtonUp(0))
            {
                _selectedGem = null;
            }
        }

        private void HandleTouchInput()
        {
            if (Input.touchCount == 0) return;

            Touch touch = Input.GetTouch(0);

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    var hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(touch.position), Vector2.zero);
                    if (hit.collider != null)
                    {
                        _selectedGem = hit.collider.GetComponent<BoardEntity>();
                        _startTouchPosition = touch.position;
                    }
                    break;

                case TouchPhase.Moved:
                    if (_selectedGem != null)
                    {
                        CheckSwipe(touch.position);
                    }
                    break;

                case TouchPhase.Ended:
                case TouchPhase.Canceled:
                    _selectedGem = null;
                    break;
            }
        }

        private void CheckSwipe(Vector2 currentPosition)
        {
            float distance = Vector2.Distance(_startTouchPosition, currentPosition);

            if (distance < _swipeThreshold) return;

            Vector2 dir = (currentPosition - _startTouchPosition).normalized;
            Vector2 direction = Mathf.Abs(dir.x) > Mathf.Abs(dir.y)
                ? dir.x > 0 ? Vector2.right : Vector2.left
                : dir.y > 0 ? Vector2.up : Vector2.down;

            _swapHandler?.TrySwap(_selectedGem, direction);

            // Prevent multiple swaps from the same gesture
            _selectedGem = null;
        }
    }
}
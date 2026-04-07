using Assets.Scripts.Game.Board;
using Assets.Scripts.Game.Entities;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.Game.Systems
{
    public class HintSystem : ITickable
    {
        private PossibleMoveChecker _moveChecker;
        private VFXManager _vfxManager;

        private const float HINT_DELAY = 5f;
        private const float REPEAT_DELAY = 4f;

        private float _idleTimer;
        private float _repeatTimer;

        private ParticleSystem _activeHintParticles;
        private BoardEntity _hintedGem;

        public HintSystem() { }

        /// <summary>
        /// Initializes the hint system with necessary dependencies.
        /// </summary>
        /// <param name="moveChecker">The move checker used to find potential moves.</param>
        /// <param name="vfxManager">The VFX manager used to play hint effects.</param>
        public void Initialize(PossibleMoveChecker moveChecker, VFXManager vfxManager)
        {
            _moveChecker = moveChecker;
            _vfxManager = vfxManager;
        }

        /// <summary>
        /// Called every frame by Zenject. Updates the idle timer and triggers hints if needed.
        /// </summary>
        public void Tick()
        {
            if (_moveChecker == null || _vfxManager == null || !GameFlow.IsGameActive)
            {
                ResetTimer();
                return;
            }

            if (Input.anyKey || Input.touchCount > 0 || Input.GetMouseButton(0))
            {
                ResetTimer();
            }
            else
            {
                _idleTimer += Time.deltaTime;

                if (_idleTimer >= HINT_DELAY)
                {
                    if (_activeHintParticles == null)
                    {
                        ShowHint();
                    }
                    else
                    {
                        _repeatTimer += Time.deltaTime;

                        if (_repeatTimer >= REPEAT_DELAY)
                        {
                            RefreshHint();
                            _repeatTimer = 0;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Calculates and displays a hint if a move is found.
        /// </summary>
        private void ShowHint()
        {
            var move = _moveChecker.GetFirstPossibleMove();

            if (move.HasValue)
            {
                _hintedGem = move.Value.Gem1;
                if (_hintedGem == null) return;

                _activeHintParticles = _vfxManager.PlayHintLoop(_hintedGem.transform.position);
            }
        }

        /// <summary>
        /// Refreshes the currently active hint or finds a new one if the current one is lost.
        /// </summary>
        private void RefreshHint()
        {
            if (_activeHintParticles != null && _hintedGem != null)
            {
                _vfxManager.RestartHintLoop(_activeHintParticles);
            }
            else
            {
                StopHint();
                ShowHint();
            }
        }

        /// <summary>
        /// Resets the idle and repeat timers and stops any active hint.
        /// </summary>
        public void ResetTimer()
        {
            _idleTimer = 0;
            _repeatTimer = 0;
            StopHint();
        }

        /// <summary>
        /// Stops the currently active hint loop and clears particle effects.
        /// </summary>
        private void StopHint()
        {
            if (_activeHintParticles != null)
            {
                _vfxManager.StopHintLoop(_activeHintParticles);
                _activeHintParticles = null;
            }

            _hintedGem = null;
        }
    }
}
 village
 village
 village
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

        public void Initialize(PossibleMoveChecker moveChecker, VFXManager vfxManager)
        {
            _moveChecker = moveChecker;
            _vfxManager = vfxManager;
        }

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

        public void ResetTimer()
        {
            _idleTimer = 0;
            _repeatTimer = 0;
            StopHint();
        }

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
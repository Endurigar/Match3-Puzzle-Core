using Assets.Scripts.Game.Entities;
using UnityEngine;

namespace Assets.Scripts.Game.Board
{
    public class ObstacleController : BoardEntity
    {
        [SerializeField] private ObstacleType _obstacleType;
        [SerializeField] private int _health = 1;
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private Sprite[] _healthStates; // 0: Full health, 1: Damaged, etc.

        public ObstacleType ObstacleType => _obstacleType;

        public override bool IsMovable() => false;
        public override bool IsDestroyable() => _obstacleType != ObstacleType.Permanent && _health > 0;

        public void TakeDamage(int amount = 1)
        {
            if (_obstacleType == ObstacleType.Permanent) return;

            _health = Mathf.Max(0, _health - amount);
            UpdateVisuals();

            if (_health <= 0)
            {
                Destroy(gameObject);
            }
        }

        private void UpdateVisuals()
        {
            if (_spriteRenderer == null || _healthStates == null || _healthStates.Length == 0) return;

            int index = Mathf.Clamp(_healthStates.Length - _health, 0, _healthStates.Length - 1);
            _spriteRenderer.sprite = _healthStates[index];
        }
    }
}
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

        /// <summary>
        /// Gets the type of the obstacle.
        /// </summary>
        public ObstacleType ObstacleType => _obstacleType;

        /// <summary>
        /// Obstacles are not movable.
        /// </summary>
        /// <returns>Always false.</returns>
        public override bool IsMovable() => false;

        /// <summary>
        /// Permanent obstacles are not destroyable. Others are destroyable if health is above 0.
        /// </summary>
        /// <returns>True if destroyable.</returns>
        public override bool IsDestroyable() => _obstacleType != ObstacleType.Permanent && _health > 0;

        /// <summary>
        /// Damages the obstacle and updates its visuals or destroys it.
        /// </summary>
        /// <param name="amount">Amount of damage to take.</param>
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

        /// <summary>
        /// Updates the visual representation of the obstacle based on its current health.
        /// </summary>
        private void UpdateVisuals()
        {
            if (_spriteRenderer == null || _healthStates == null || _healthStates.Length == 0) return;

            int index = Mathf.Clamp(_healthStates.Length - _health, 0, _healthStates.Length - 1);
            _spriteRenderer.sprite = _healthStates[index];
        }
    }
}
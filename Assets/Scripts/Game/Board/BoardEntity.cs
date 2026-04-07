using UnityEngine;

namespace Assets.Scripts.Game.Board
{
    public abstract class BoardEntity : MonoBehaviour
    {
        /// <summary>
        /// Gets or sets the grid position of the entity.
        /// </summary>
        public Vector2 GridPosition { get; set; }

        /// <summary>
        /// Determines if the entity can be destroyed.
        /// </summary>
        /// <returns>True if destroyable, false otherwise.</returns>
        public virtual bool IsDestroyable() => true;

        /// <summary>
        /// Determines if the entity can be moved.
        /// </summary>
        /// <returns>True if movable, false otherwise.</returns>
        public virtual bool IsMovable() => true;
    }
}
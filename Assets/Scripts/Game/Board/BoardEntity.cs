using UnityEngine;

namespace Assets.Scripts.Game.Board
{
    public abstract class BoardEntity : MonoBehaviour
    {
        public Vector2 GridPosition { get; set; }

        public virtual bool IsDestroyable() => true;
        public virtual bool IsMovable() => true;
    }
}
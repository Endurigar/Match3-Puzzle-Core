using Assets.Scripts.Game.Entities;
using UnityEngine;

namespace Assets.Scripts.Game.Board
{
    public class BombController : BoardEntity
    {
        [SerializeField] private BombType _bombType;
        /// <summary>
        /// Gets the type of the bomb.
        /// </summary>
        public BombType BombType => _bombType;
    }
}
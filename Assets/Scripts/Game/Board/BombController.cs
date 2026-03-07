using Assets.Scripts.Game.Entities;
using UnityEngine;

namespace Assets.Scripts.Game.Board
{
    public class BombController : BoardEntity
    {
        [SerializeField] private BombType _bombType;
        public BombType BombType => _bombType;
    }
}
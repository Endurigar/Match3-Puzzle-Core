using Assets.Scripts.Game.Board;
using UnityEngine;

namespace Assets.Scripts.Game.Entities
{
    public class GemController : BoardEntity
    {
        [SerializeField] private GemType gemType;

        public GemType GemType => gemType;

    }
}
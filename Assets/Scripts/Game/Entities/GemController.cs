using Assets.Scripts.Game.Board;
using UnityEngine;

namespace Assets.Scripts.Game.Entities
{
    public class GemController : BoardEntity
    {
        [SerializeField] private GemType gemType;

        /// <summary>
        /// Gets the type of the gem.
        /// </summary>
        public GemType GemType => gemType;


    }
}
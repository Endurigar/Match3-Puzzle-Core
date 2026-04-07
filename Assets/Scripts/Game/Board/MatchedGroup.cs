using System.Collections.Generic;
using Assets.Scripts.Game.Entities;
using UnityEngine;

namespace Assets.Scripts.Game.Board
{
    public class MatchedGroup
    {
        /// <summary>
        /// Gets the list of gem controllers in the matched group.
        /// </summary>
        public List<GemController> Gems { get; }

        /// <summary>
        /// Gets the direction of the match.
        /// </summary>
        public MatchDirection Direction { get; }

        /// <summary>
        /// Gets the key position of the match (e.g., center for cross or powerup generation location).
        /// </summary>
        public Vector2Int? KeyPosition { get; }

        /// <summary>
        /// Initializes a new instance of the MatchedGroup class.
        /// </summary>
        /// <param name="gems">The list of matched gems.</param>
        /// <param name="direction">The direction of the match.</param>
        /// <param name="keyPosition">Optional key position for special match handling.</param>
        public MatchedGroup(List<GemController> gems, MatchDirection direction, Vector2Int? keyPosition = null)
        {
            Gems = gems;
            Direction = direction;
            KeyPosition = keyPosition;
        }
    }
}
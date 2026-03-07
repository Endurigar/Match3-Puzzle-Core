using System.Collections.Generic;
using Assets.Scripts.Game.Entities;
using UnityEngine;

namespace Assets.Scripts.Game.Board
{
    public class MatchedGroup
    {
        public List<GemController> Gems { get; }
        public MatchDirection Direction { get; }
        public Vector2Int? KeyPosition { get; }

        public MatchedGroup(List<GemController> gems, MatchDirection direction, Vector2Int? keyPosition = null)
        {
            Gems = gems;
            Direction = direction;
            KeyPosition = keyPosition;
        }
    }
}
using Assets.Scripts.Game.Entities;
using UnityEngine;

namespace Assets.Scripts.Game.Board
{
    public class PossibleMoveChecker
    {
        private readonly BoardState _board;
        private readonly MatchFinder _matchFinder;
        private static readonly Vector2Int[] Directions = { Vector2Int.right, Vector2Int.up };

        public PossibleMoveChecker(BoardState board, MatchFinder matchFinder)
        {
            _board = board;
            _matchFinder = matchFinder;
        }

        public bool HasPossibleMoves() => GetFirstPossibleMove().HasValue;

        public (BoardEntity Gem1, BoardEntity Gem2)? GetFirstPossibleMove()
        {
            for (int x = 0; x < _board.Width; x++)
            {
                for (int y = 0; y < _board.Height; y++)
                {
                    var currentGem = _board.GetGem(x, y);
                    if (currentGem == null || !currentGem.IsMovable()) continue;

                    Vector2Int pos = new Vector2Int(x, y);

                    foreach (var dir in Directions)
                    {
                        Vector2Int neighborPos = pos + dir;
                        if (!_board.IsInside(neighborPos)) continue;

                        var neighborGem = _board.GetGem(neighborPos.x, neighborPos.y);
                        if (neighborGem == null || !neighborGem.IsMovable()) continue;

                        _board.SetGem(x, y, neighborGem);
                        _board.SetGem(neighborPos.x, neighborPos.y, currentGem);

                        bool matchFound = false;
                        if (currentGem is BombController || neighborGem is BombController)
                        {
                            matchFound = true;
                        }
                        else
                        {
                            matchFound = _matchFinder.HasMatchAt(pos) || _matchFinder.HasMatchAt(neighborPos);
                        }

                        _board.SetGem(x, y, currentGem);
                        _board.SetGem(neighborPos.x, neighborPos.y, neighborGem);

                        if (matchFound) return (currentGem, neighborGem);
                    }
                }
            }
            return null;
        }
    }
}
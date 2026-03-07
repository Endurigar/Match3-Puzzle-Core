using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Game.Entities;
using UnityEngine;

namespace Assets.Scripts.Game.Board
{
    public class MatchFinder
    {
        private readonly BoardState _board;

        public MatchFinder(BoardState board)
        {
            _board = board;
        }

        public bool HasAnyMatches()
        {
            for (int x = 0; x < _board.Width; x++)
            {
                for (int y = 0; y < _board.Height; y++)
                {
                    if (HasMatchAt(new Vector2Int(x, y))) return true;
                }
            }
            return false;
        }

        public List<MatchedGroup> GetMatchesAt(params Vector2Int[] positions)
        {
            List<MatchedGroup> groups = new();
            HashSet<Vector2Int> visited = new();

            foreach (var pos in positions)
            {
                if (!_board.IsInside(pos) || visited.Contains(pos)) continue;

                var gem = _board.GetGem(pos.x, pos.y) as GemController;
                if (gem == null) continue;

                var horz = GetFullMatch(pos, Vector2Int.left, Vector2Int.right, gem.GemType);
                var vert = GetFullMatch(pos, Vector2Int.down, Vector2Int.up, gem.GemType);

                if (horz.Count >= 3 && vert.Count >= 3)
                {
                    var allGems = horz.Concat(vert).Distinct().ToList();
                    foreach (var g in allGems) visited.Add(Vector2Int.RoundToInt(g.GridPosition));
                    groups.Add(new MatchedGroup(allGems, MatchDirection.Cross, pos));
                }
            }

            foreach (var pos in positions)
            {
                if (!_board.IsInside(pos) || visited.Contains(pos)) continue;

                var gem = _board.GetGem(pos.x, pos.y) as GemController;
                if (gem == null) continue;

                var horz = GetFullMatch(pos, Vector2Int.left, Vector2Int.right, gem.GemType);
                if (horz.Count >= 3)
                {
                    foreach (var g in horz) visited.Add(Vector2Int.RoundToInt(g.GridPosition));
                    groups.Add(new MatchedGroup(horz, MatchDirection.Horizontal));
                    continue;
                }

                var vert = GetFullMatch(pos, Vector2Int.down, Vector2Int.up, gem.GemType);
                if (vert.Count >= 3)
                {
                    foreach (var g in vert) visited.Add(Vector2Int.RoundToInt(g.GridPosition));
                    groups.Add(new MatchedGroup(vert, MatchDirection.Vertical));
                }
            }

            return groups;
        }

        public bool HasMatchAt(Vector2Int pos)
        {
            var gem = _board.GetGem(pos.x, pos.y) as GemController;
            if (gem == null) return false;

            GemType type = gem.GemType;
            int h = 1 + CountSame(pos, Vector2Int.left, type) + CountSame(pos, Vector2Int.right, type);
            int v = 1 + CountSame(pos, Vector2Int.down, type) + CountSame(pos, Vector2Int.up, type);

            return h >= 3 || v >= 3;
        }

        private List<GemController> GetFullMatch(Vector2Int pos, Vector2Int dir1, Vector2Int dir2, GemType type)
        {
            var gem = _board.GetGem(pos.x, pos.y) as GemController;
            return GetMatchLine(pos, dir1, type)
                   .Concat(new[] { gem })
                   .Concat(GetMatchLine(pos, dir2, type))
                   .ToList();
        }

        private List<GemController> GetMatchLine(Vector2Int start, Vector2Int dir, GemType type)
        {
            List<GemController> matched = new();
            Vector2Int current = start + dir;
            while (_board.IsInside(current))
            {
                var gem = _board.GetGem(current.x, current.y) as GemController;
                if (gem == null || gem.GemType != type) break;
                matched.Add(gem);
                current += dir;
            }
            return matched;
        }

        private int CountSame(Vector2Int pos, Vector2Int dir, GemType type)
        {
            int count = 0;
            Vector2Int current = pos + dir;
            while (_board.IsInside(current))
            {
                var gem = _board.GetGem(current.x, current.y) as GemController;
                if (gem == null || gem.GemType != type) break;
                count++;
                current += dir;
            }
            return count;
        }
    }
}
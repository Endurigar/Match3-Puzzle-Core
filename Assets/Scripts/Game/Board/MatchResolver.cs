using Assets.Scripts.Game.Entities;
using Assets.Scripts.Game.Systems;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using DG.Tweening;

namespace Assets.Scripts.Game.Board
{
    public class MatchResolver
    {
        private readonly BoardState _board;
        private readonly MatchFinder _finder;
        private readonly GravityController _gravity;
        private readonly MatchAnimator _animator;
        private readonly GemsScriptableObject _gemsData;
        private readonly ScoreManager _scoreManager;
        private readonly VFXManager _vfxManager;
        private readonly AudioManager _audioManager;
        private readonly FloatingTextManager _floatingTextManager;
        private readonly GemPoolManager _poolManager;

        public MatchResolver(BoardState board, GravityController gravity, MatchAnimator animator,
                             GemsScriptableObject gemsData, ScoreManager scoreManager,
                             VFXManager vfxManager, AudioManager audioManager,
                             FloatingTextManager floatingTextManager,
                             GemPoolManager poolManager)
        {
            _board = board;
            _gravity = gravity;
            _animator = animator;
            _gemsData = gemsData;
            _scoreManager = scoreManager;
            _vfxManager = vfxManager;
            _audioManager = audioManager;
            _floatingTextManager = floatingTextManager;
            _poolManager = poolManager;
            _finder = new MatchFinder(board);
        }

        public bool HasMatches() => _finder.HasAnyMatches();

        public async Task ResolveMatches()
        {
            if (!GameFlow.IsGameActive) return;

            await _gravity.DropAndRefill();

            bool matchesFound = true;
            int comboMultiplier = 1;

            while (matchesFound)
            {
                if (!GameFlow.IsGameActive) break;

                var positions = GetAllBoardPositions();
                var groups = _finder.GetMatchesAt(positions.ToArray());

                if (groups.Count == 0)
                {
                    matchesFound = false;
                    break;
                }

                await AnimateAndRemoveGroups(groups, comboMultiplier);

                if (!GameFlow.IsGameActive) break;

                await _gravity.DropAndRefill();
                comboMultiplier++;
            }
        }

        private async Task AnimateAndRemoveGroups(List<MatchedGroup> groups, int multiplier)
        {
            if (groups.Count > 0) _audioManager.PlaySFX(SFXType.Match);

            HashSet<BoardEntity> gemsToDestroy = new();
            List<Task> animations = new();
            HashSet<Vector2Int> matchPositions = new();

            foreach (var group in groups)
            {
                int score = group.Gems.Count * 100;
                _scoreManager.AddScore(score, multiplier);

                Vector3 centerPos = GetGroupCenter(group.Gems);
                _floatingTextManager.ShowMatchScore(centerPos, score * multiplier);

                if (multiplier > 1)
                    _floatingTextManager.ShowComboText(centerPos, multiplier);

                foreach (var gem in group.Gems)
                    matchPositions.Add(Vector2Int.RoundToInt(gem.GridPosition));
            }

            // Damage Obstacles
            foreach (var gem in _board.GetAllGems())
            {
                if (gem is ObstacleController obstacle && IsAdjacentToMatch(obstacle.GridPosition, matchPositions))
                {
                    obstacle.TakeDamage();
                }
            }

            // Create Bonuses & Collect Destroyables
            foreach (var group in groups)
            {
                if (group.Gems.Count > 3 || group.Direction == MatchDirection.Cross)
                {
                    BombController bonus = CreateBonusFromGroup(group);
                    foreach (var gem in group.Gems)
                    {
                        if (gem.GridPosition != bonus.GridPosition && gem.IsDestroyable())
                            gemsToDestroy.Add(gem);
                    }
                }
                else
                {
                    foreach (var gem in group.Gems)
                        if (gem.IsDestroyable()) gemsToDestroy.Add(gem);
                }
            }

            // Execute Destruction
            foreach (var gem in gemsToDestroy)
            {
                if (_board.GetGem((int)gem.GridPosition.x, (int)gem.GridPosition.y) == gem)
                {
                    _board.SetGem((int)gem.GridPosition.x, (int)gem.GridPosition.y, null);
                    _vfxManager?.PlayGemDestroy(gem.transform.position);
                    animations.Add(_animator.AnimateDestroyGem(gem));
                }
            }

            await Task.WhenAll(animations);
        }

        private BombController CreateBonusFromGroup(MatchedGroup group)
        {
            BoardEntity baseGem = null;
            if (group.KeyPosition.HasValue)
                baseGem = _board.GetGem(group.KeyPosition.Value.x, group.KeyPosition.Value.y);

            if (baseGem == null)
                baseGem = group.Gems[group.Gems.Count / 2];

            Vector2Int pos = Vector2Int.RoundToInt(baseGem.GridPosition);

            if (baseGem != null && baseGem.gameObject != null)
            {
                _poolManager.Release(baseGem);
            }

            BombType type = DetermineBombType(group);
            var bonus = _poolManager.GetBomb(type);

            bonus.transform.SetParent(baseGem != null ? baseGem.transform.parent : null);
            bonus.transform.localPosition = new Vector3(pos.x, pos.y, 0f);

            var prefab = _gemsData.GetBombByType(type);
            Vector3 targetScale = prefab != null ? prefab.transform.localScale : Vector3.one;

            bonus.transform.localScale = Vector3.zero;
            bonus.transform.DOScale(targetScale, 0.4f).SetEase(Ease.OutBack);
            bonus.GridPosition = pos;

            _board.SetGem(pos.x, pos.y, bonus);
            return bonus;
        }

        private BombType DetermineBombType(MatchedGroup group)
        {
            if (group.Gems.Count >= 5 && group.Direction != MatchDirection.Cross)
                return BombType.ColorBomb;

            if (group.Direction == MatchDirection.Cross)
                return Random.value > 0.5f ? BombType.HorizontalBomb : BombType.VerticalBomb;

            return group.Direction == MatchDirection.Horizontal ? BombType.HorizontalBomb : BombType.VerticalBomb;
        }

        private bool IsAdjacentToMatch(Vector2 pos, HashSet<Vector2Int> matched)
        {
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    if (x == 0 && y == 0) continue;
                    if (matched.Contains(new Vector2Int((int)pos.x + x, (int)pos.y + y))) return true;
                }
            }
            return false;
        }

        private List<Vector2Int> GetAllBoardPositions()
        {
            List<Vector2Int> positions = new();
            for (int x = 0; x < _board.Width; x++)
                for (int y = 0; y < _board.Height; y++)
                    positions.Add(new Vector2Int(x, y));
            return positions;
        }

        private Vector3 GetGroupCenter(List<GemController> gems)
        {
            if (gems == null || gems.Count == 0) return Vector3.zero;
            Vector3 sum = Vector3.zero;
            foreach (var gem in gems) sum += gem.transform.position;
            return sum / gems.Count;
        }
    }
}
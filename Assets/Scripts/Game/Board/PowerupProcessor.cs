using System.Collections.Generic;
using System.Threading.Tasks;
using Assets.Scripts.Game.Entities;
using Assets.Scripts.Game.Systems;
using UnityEngine;

namespace Assets.Scripts.Game.Board
{
    public class PowerupProcessor
    {
        private readonly BoardState _board;
        private readonly MatchAnimator _matchAnimator;
        private readonly ScoreManager _scoreManager;
        private readonly VFXManager _vfxManager;
        private readonly AudioManager _audioManager;
        private readonly FloatingTextManager _floatingTextManager;
        private readonly CameraEffectManager _cameraEffectManager;
        private readonly GemPoolManager _poolManager;

        private const int GemScore = 50;
        private const int ObstacleScore = 150;
        private const int PowerupScore = 200;

        public PowerupProcessor(BoardState board, MatchAnimator animator, ScoreManager scoreManager,
                                VFXManager vfxManager, AudioManager audioManager,
                                FloatingTextManager floatingTextManager,
                                CameraEffectManager cameraEffectManager,
                                GemPoolManager poolManager)
        {
            _board = board;
            _matchAnimator = animator;
            _scoreManager = scoreManager;
            _vfxManager = vfxManager;
            _audioManager = audioManager;
            _floatingTextManager = floatingTextManager;
            _cameraEffectManager = cameraEffectManager;
            _poolManager = poolManager;
        }

        public async Task TriggerPowerup(BombController bomb, BoardEntity targetGem = null)
        {
            if (!GameFlow.IsGameActive || bomb == null) return;

            // Score & UI
            _scoreManager.AddScore(PowerupScore);
            _floatingTextManager.ShowBonusScore(bomb.transform.position, PowerupScore);

            if (bomb.BombType == BombType.ColorBomb)
            {
                _cameraEffectManager.ShakeCamera();
            }

            // Audio & VFX
            Vector2 bombGridPos = bomb.GridPosition;
            SFXType sfxType = bomb.BombType == BombType.ColorBomb ? SFXType.ColorBomb : SFXType.BombExplode;
            _audioManager.PlaySFX(sfxType);

            _board.SetGem((int)bombGridPos.x, (int)bombGridPos.y, null);
            var vfxTask = _vfxManager?.PlayActivationEffectAsync(bomb.transform.position, bomb.BombType);

            _poolManager.Release(bomb);

            if (!GameFlow.IsGameActive) return;

            // Logic execution
            if (bomb.BombType == BombType.ColorBomb)
            {
                await TriggerColorBomb(bombGridPos, targetGem, vfxTask);
            }
            else
            {
                switch (bomb.BombType)
                {
                    case BombType.VerticalBomb:
                        await ClearLine(bombGridPos, true, bomb.BombType);
                        break;
                    case BombType.HorizontalBomb:
                        await ClearLine(bombGridPos, false, bomb.BombType);
                        break;
                    default:
                        await ClearArea(bombGridPos, bomb.BombType);
                        break;
                }
            }

            if (vfxTask != null) await vfxTask;
        }

        private async Task TriggerColorBomb(Vector2 bombPos, BoardEntity manualTarget, Task vfxTask)
        {
            if (vfxTask != null) await vfxTask;
            if (!GameFlow.IsGameActive) return;

            var tasks = new List<Task>();
            var targetType = manualTarget as GemController;
            if (targetType == null) return;

            for (int x = 0; x < _board.Width; x++)
            {
                for (int y = 0; y < _board.Height; y++)
                {
                    var target = _board.GetGem(x, y) as GemController;
                    if (target != null && target.GemType == targetType.GemType)
                    {
                        tasks.Add(DestroyGem(target, GemScore, BombType.ColorBomb));
                    }
                }
            }
            await Task.WhenAll(tasks);
        }

        private async Task ClearLine(Vector2 pos, bool isVertical, BombType bombType)
        {
            var tasks = new List<Task>();
            int max = isVertical ? _board.Height : _board.Width;

            for (int i = 0; i < max; i++)
            {
                int x = isVertical ? (int)pos.x : i;
                int y = isVertical ? i : (int)pos.y;
                ProcessTarget(_board.GetGem(x, y), bombType, tasks);
            }
            if (tasks.Count > 0) await Task.WhenAll(tasks);
        }

        private async Task ClearArea(Vector2 pos, BombType bombType)
        {
            var tasks = new List<Task>();
            for (int dx = -1; dx <= 1; dx++)
            {
                for (int dy = -1; dy <= 1; dy++)
                {
                    int x = (int)pos.x + dx;
                    int y = (int)pos.y + dy;
                    if (_board.IsInside(new Vector2(x, y)))
                    {
                        ProcessTarget(_board.GetGem(x, y), bombType, tasks);
                    }
                }
            }
            if (tasks.Count > 0) await Task.WhenAll(tasks);
        }

        private void ProcessTarget(BoardEntity target, BombType bombType, List<Task> tasks)
        {
            if (target == null || !target.IsDestroyable()) return;

            if (target is BombController otherBomb)
            {
                tasks.Add(TriggerPowerup(otherBomb));
            }
            else
            {
                int score = (target is ObstacleController) ? ObstacleScore : GemScore;
                tasks.Add(DestroyGem(target, score, bombType));
            }
        }

        private async Task DestroyGem(BoardEntity gem, int scoreValue, BombType? sourceType = null)
        {
            if (!GameFlow.IsGameActive || gem == null || gem.gameObject == null) return;

            if (sourceType.HasValue)
                _vfxManager?.PlayBonusDestroy(gem.transform.position, sourceType.Value);
            else
                _vfxManager?.PlayGemDestroy(gem.transform.position);

            _board.SetGem(gem.GridPosition.x, gem.GridPosition.y, null);
            _scoreManager.AddScore(scoreValue);
            _floatingTextManager.ShowBonusScore(gem.transform.position, scoreValue);

            await _matchAnimator.AnimateDestroyGem(gem);
        }
    }
}
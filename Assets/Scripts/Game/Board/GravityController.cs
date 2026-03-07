using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using Assets.Scripts.Game.Systems;

namespace Assets.Scripts.Game.Board
{
    public class GravityController
    {
        private readonly BoardState _board;
        private readonly BoardGenerator _generator;
        private bool _isActive = true;

        public GravityController(BoardState board, BoardGenerator generator)
        {
            _board = board;
            _generator = generator;
        }

        public void Stop()
        {
            _isActive = false;
            DOTween.KillAll();
        }

        public async Task DropAndRefill()
        {
            if (!_isActive || !GameFlow.IsGameActive) return;

            var activeTweens = new List<Task>();

            for (int x = 0; x < _board.Width; x++)
            {
                for (int y = 0; y < _board.Height; y++)
                {
                    if (!_isActive || !GameFlow.IsGameActive) return;

                    if (_board.GetGem(x, y) == null)
                    {
                        // 1. Pull down existing gems
                        for (int k = y + 1; k < _board.Height; k++)
                        {
                            var above = _board.GetGem(x, k);
                            if (above != null && above.IsMovable())
                            {
                                MoveGemTo(above, x, y, activeTweens);
                                _board.SetGem(x, k, null);
                                break;
                            }
                        }

                        // 2. Spawn new if nothing above
                        if (_board.GetGem(x, y) == null)
                        {
                            var newGem = _generator.SpawnGem(x, y, true);
                            if (newGem != null)
                            {
                                newGem.transform.localPosition = new Vector3(x, _board.Height + 1, 0);
                                MoveGemTo(newGem, x, y, activeTweens);
                            }
                        }
                    }
                }
            }

            if (activeTweens.Count > 0)
            {
                try { await Task.WhenAll(activeTweens); } catch { /* Ignore cancellation */ }
            }
        }

        private void MoveGemTo(BoardEntity gem, int x, int y, List<Task> tasks)
        {
            _board.SetGem(x, y, gem);
            tasks.Add(gem.transform.DOLocalMove(new Vector3(x, y, 0), 0.25f)
                .SetEase(Ease.InQuad)
                .AsyncWaitForCompletion());
        }
    }
}
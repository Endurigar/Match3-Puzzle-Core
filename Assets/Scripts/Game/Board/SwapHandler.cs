using UnityEngine;
using DG.Tweening;
using System.Threading.Tasks;
using Assets.Scripts.Game.Entities;
using Assets.Scripts.Game.Systems;

namespace Assets.Scripts.Game.Board
{
    public class SwapHandler
    {
        private readonly BoardState _board;
        private readonly PowerupProcessor _powerupProcessor;
        private readonly AudioManager _audioManager;

        private bool _isProcessing;

        public SwapHandler(BoardState board, PowerupProcessor powerupProcessor, AudioManager audioManager)
        {
            _board = board;
            _powerupProcessor = powerupProcessor;
            _audioManager = audioManager;
        }

        public async void TrySwap(BoardEntity gem, Vector2 direction)
        {
            if (_isProcessing) return;
            _isProcessing = true;

            Vector2 pos1 = gem.GridPosition;
            Vector2 pos2 = pos1 + direction;

            if (!_board.IsInside(pos2))
            {
                _isProcessing = false;
                return;
            }

            var gem1 = _board.GetGem((int)pos1.x, (int)pos1.y);
            var gem2 = _board.GetGem((int)pos2.x, (int)pos2.y);

            if (gem1 == null || gem2 == null || !gem1.IsMovable() || !gem2.IsMovable())
            {
                _isProcessing = false;
                return;
            }

            _audioManager.PlaySFX(SFXType.Swap);

            BombController bonusGem = gem1 as BombController ?? gem2 as BombController;
            BoardEntity targetGem = (bonusGem == gem1) ? gem2 : gem1;

            await PerformSwapAnim(gem1, gem2);

            if (bonusGem != null)
            {
                await _powerupProcessor.TriggerPowerup(bonusGem, targetGem);
            }
            else if (!_board.HasMatches())
            {
                // No match, revert
                await PerformSwapAnim(gem1, gem2);
                _isProcessing = false;
                return;
            }

            await _board.CheckMatches();
            _isProcessing = false;
        }

        private async Task PerformSwapAnim(BoardEntity gem1, BoardEntity gem2)
        {
            Vector2 pos1 = gem1.GridPosition;
            Vector2 pos2 = gem2.GridPosition;

            _board.SetGem(pos1.x, pos1.y, gem2);
            _board.SetGem(pos2.x, pos2.y, gem1);

            gem1.GridPosition = pos2;
            gem2.GridPosition = pos1;

            Sequence sequence = DOTween.Sequence();
            sequence.Append(gem1.transform.DOMove(gem2.transform.position, 0.25f).SetEase(Ease.InOutQuad));
            sequence.Join(gem2.transform.DOMove(gem1.transform.position, 0.25f).SetEase(Ease.InOutQuad));

            await sequence.AsyncWaitForCompletion();
        }
    }
}
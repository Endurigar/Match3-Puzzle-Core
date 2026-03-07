using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using Assets.Scripts.Game.Systems;
using Zenject;

namespace Assets.Scripts.Game.Board
{
    public class MatchAnimator
    {
        [Inject] private readonly GemPoolManager _poolManager;

        public async Task AnimateDestroyGem(BoardEntity gem)
        {
            if (gem == null || gem.gameObject == null || GameFlow.Token.IsCancellationRequested) return;

            try
            {
                await gem.transform.DOScale(Vector3.zero, 0.25f)
                    .SetEase(Ease.InBack)
                    .AsyncWaitForCompletion();
            }
            catch
            {
                return;
            }

            if (gem != null && gem.gameObject != null && !GameFlow.Token.IsCancellationRequested)
            {
                _poolManager.Release(gem);
            }
        }
    }
}
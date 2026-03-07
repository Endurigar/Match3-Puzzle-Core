using System.Threading;
using DG.Tweening;

namespace Assets.Scripts.Game.Systems
{
    public static class GameFlow
    {
        public static bool IsGameActive = true;

        private static CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        public static CancellationToken Token => _cancellationTokenSource.Token;

        public static void StopGame()
        {
            IsGameActive = false;
            _cancellationTokenSource?.Cancel();
            DOTween.KillAll();
        }

        public static void StartGame()
        {
            IsGameActive = true;
            _cancellationTokenSource?.Dispose();
            _cancellationTokenSource = new CancellationTokenSource();
        }
    }
}
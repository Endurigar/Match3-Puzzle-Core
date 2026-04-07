using System.Threading;
using DG.Tweening;

namespace Assets.Scripts.Game.Systems
{
    public static class GameFlow
    {
        public static bool IsGameActive = true;

        private static CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        /// <summary>
        /// Gets the cancellation token for the current game session.
        /// </summary>
        public static CancellationToken Token => _cancellationTokenSource.Token;

        /// <summary>
        /// Stops the current game session and cancels all pending tasks and animations.
        /// </summary>
        public static void StopGame()
        {
            IsGameActive = false;
            _cancellationTokenSource?.Cancel();
            DOTween.KillAll();
        }

        /// <summary>
        /// Starts a new game session and resets the cancellation token.
        /// </summary>
        public static void StartGame()
        {
            IsGameActive = true;
            _cancellationTokenSource?.Dispose();
            _cancellationTokenSource = new CancellationTokenSource();
        }
    }
}
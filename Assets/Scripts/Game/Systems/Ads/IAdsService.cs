namespace Assets.Scripts.Game.Systems.Ads
{
    /// <summary>
    /// Interface for an advertisement service.
    /// </summary>
    public interface IAdsService
    {
        /// <summary>
        /// Initializes the advertisement service.
        /// </summary>
        void Initialize();

        /// <summary>
        /// Shows or hides the advertisement banner.
        /// </summary>
        /// <param name="show">True to show the banner, false to hide it.</param>
        void ShowBanner(bool show);
    }
}
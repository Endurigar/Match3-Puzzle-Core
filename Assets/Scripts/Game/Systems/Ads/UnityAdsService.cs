using UnityEngine;
using UnityEngine.Advertisements;
using Zenject;

namespace Assets.Scripts.Game.Systems.Ads
{
    public class UnityAdsService : IAdsService, IUnityAdsInitializationListener, IInitializable
    {
        private readonly string _androidGameId = "YOUR_ANDROID_GAME_ID";
        private readonly string _iosGameId = "YOUR_IOS_GAME_ID";

        private readonly string _bannerIdAndroid = "Banner_Android";
        private readonly string _bannerIdIOS = "Banner_iOS";

        private readonly bool _testMode = true;

        private string _gameId;
        private string _bannerId;

        public void Initialize()
        {
#if UNITY_IOS
            _gameId = _iosGameId;
            _bannerId = _bannerIdIOS;
#else
            _gameId = _androidGameId;
            _bannerId = _bannerIdAndroid;
#endif

            if (!Advertisement.isInitialized && Advertisement.isSupported)
            {
                Advertisement.Initialize(_gameId, _testMode, this);
            }

            Advertisement.Banner.SetPosition(BannerPosition.BOTTOM_CENTER);
        }

        public void ShowBanner(bool show)
        {
            if (!Advertisement.isInitialized) return;

            if (show)
            {
                BannerLoadOptions options = new BannerLoadOptions
                {
                    loadCallback = () => Advertisement.Banner.Show(_bannerId),
                    errorCallback = (message) => Debug.LogError($"Banner Load Error: {message}")
                };
                Advertisement.Banner.Load(_bannerId, options);
            }
            else
            {
                Advertisement.Banner.Hide();
            }
        }

        public void OnInitializationComplete() { }

        public void OnInitializationFailed(UnityAdsInitializationError error, string message)
        {
            Debug.LogError($"Unity Ads Init Failed: {error} - {message}");
        }
    }
}
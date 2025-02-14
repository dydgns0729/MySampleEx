using GoogleMobileAds.Api;
using System;
using UnityEngine;

namespace MySampleEx
{
    /// <summary>
    /// 광고를 관리하는 클래스
    /// 배너광고, 전면광고, 리워드(동영상) 광고 
    /// </summary>
    public class AdManager : PersistentSingleton<AdManager>
    {
#if AD_MODE
        #region Variables
        BannerView bannerView;
        InterstitialAd interstitialAd;
        private RewardedAd rewardedAd;
        #endregion

        private void Start()
        {
            // Initialize the Google Mobile Ads SDK.
            MobileAds.Initialize(initStatus => { });

            //광고 요청
            RequestBanner();
            RequestInterstitialAd();
            RequestRewardAd();
        }

        //배너 광고
        #region Banner
        public void RequestBanner()
        {
            string _adUnitId;
#if UNITY_ANDROID
              _adUnitId = "ca-app-pub-3940256099942544/6300978111";
#elif UNITY_IPHONE
              _adUnitId = "ca-app-pub-3940256099942544/2934735716";
#else
              _adUnitId = "unused";
#endif

            // If we already have a banner, destroy the old one.
            if (bannerView != null)
            {
                DestroyAd();
            }

            // Create a 320x50 banner at top of the screen
            //bannerView = new BannerView(_adUnitId, AdSize.Banner, AdPosition.Top);
            AdSize adAdaptiveSize = AdSize.GetCurrentOrientationAnchoredAdaptiveBannerAdSizeWithWidth(AdSize.FullWidth);
            bannerView = new BannerView(_adUnitId, adAdaptiveSize, AdPosition.Top);

            // create our request used to load the ad.
            var adRequest = new AdRequest();

            // send the request to load the ad.
            Debug.Log("Loading banner ad.");
            bannerView.LoadAd(adRequest);
        }

        public void ShowBanner()
        {
            if (bannerView != null)
            {
                bannerView.Show();
            }
        }

        public void HideBanner()
        {
            if (bannerView != null)
            {
                bannerView.Hide();
            }
        }

        public void DestroyAd()
        {
            if (bannerView != null)
            {
                Debug.Log("Destroying banner view.");
                bannerView.Destroy();
                bannerView = null;
            }
        }
        #endregion

        //전면광고
        #region InterstitialAd
        public void RequestInterstitialAd()
        {
            string _adUnitId;
#if UNITY_ANDROID
                _adUnitId = "ca-app-pub-3940256099942544/1033173712";
#elif UNITY_IPHONE
                _adUnitId = "ca-app-pub-3940256099942544/4411468910";
#else
                _adUnitId = "unused";
#endif

            // Clean up the old ad before loading a new one.
            if (interstitialAd != null)
            {
                interstitialAd.Destroy();
                interstitialAd = null;
            }

            Debug.Log("Loading the interstitial ad.");

            // create our request used to load the ad.
            var adRequest = new AdRequest();

            // send the request to load the ad.
            InterstitialAd.Load(_adUnitId, adRequest,
                (InterstitialAd ad, LoadAdError error) =>
                {
                    // if error is not null, the load request failed.
                    if (error != null || ad == null)
                    {
                        Debug.LogError("interstitial ad failed to load an ad " +
                                       "with error : " + error);
                        return;
                    }

                    interstitialAd = ad;

                    //interstitialAd 이벤트 일어날때 호출되는 함수 등록
                    interstitialAd.OnAdFullScreenContentOpened += HandleOnInterstitialAdOpen;
                    interstitialAd.OnAdFullScreenContentClosed += HandleOnInterstitialAdClose;
                });
        }

        public void ShowInterstitialAd()
        {
            if (interstitialAd != null && interstitialAd.CanShowAd())
            {
                Debug.Log("Showing interstitial ad.");
                interstitialAd.Show();
            }
            else
            {
                Debug.LogError("Interstitial ad is not ready yet.");
            }
        }

        void HandleOnInterstitialAdOpen()
        {

        }

        void HandleOnInterstitialAdClose()
        {
            RequestInterstitialAd();
        }
        #endregion

        //보상 동영상 광고
        #region RewardAd
        public void RequestRewardAd()
        {
            string _adUnitId;
#if UNITY_ANDROID
            _adUnitId = "ca-app-pub-3940256099942544/5224354917";
#elif UNITY_IPHONE
            _adUnitId = "ca-app-pub-3940256099942544/1712485313";
#else
            _adUnitId = "unused";
#endif

            // Clean up the old ad before loading a new one.
            if (rewardedAd != null)
            {
                rewardedAd.Destroy();
                rewardedAd = null;
            }

            Debug.Log("Loading the rewarded ad.");

            // create our request used to load the ad.
            var adRequest = new AdRequest();

            // send the request to load the ad.
            RewardedAd.Load(_adUnitId, adRequest,
                (RewardedAd ad, LoadAdError error) =>
                {
                    // if error is not null, the load request failed.
                    if (error != null || ad == null)
                    {
                        Debug.LogError("Rewarded ad failed to load an ad " +
                                       "with error : " + error);
                        return;
                    }

                    Debug.Log("Rewarded ad loaded with response : "
                              + ad.GetResponseInfo());

                    rewardedAd = ad;

                    //rewardedAd 이벤트 일어날때 호출되는 함수 등록
                    rewardedAd.OnAdFullScreenContentClosed += HandleOnRewardAdClosed;
                });
        }

        public void ShowRewardAd()
        {
            const string rewardMsg =
                        "Rewarded ad rewarded the user. Type: {0}, amount: {1}.";

            if (rewardedAd != null && rewardedAd.CanShowAd())
            {
                rewardedAd.Show((Reward reward) =>
                {
                    // TODO: Reward the user. 동영상을 끝까지 보았을 경우 보상을 진행(1000골드 지급)
                    //Debug.Log(String.Format(rewardMsg, reward.Type, reward.Amount));
                    UIManager.Instance.AddGold(1000);
                });
            }
        }

        void HandleOnRewardAdClosed()
        {
            RequestRewardAd();
        }
        #endregion
#endif
    }
}

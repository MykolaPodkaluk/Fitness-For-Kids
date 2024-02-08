#if UNITY_ANDROID || UNITY_EDITOR_WIN
using UnityEngine;
using GoogleMobileAds.Api;
using System;
using Cysharp.Threading.Tasks;

namespace FitnessForKids.Services
{
    public interface IGoogleAdsProvider : IAdsProvider
    {

    }

    public class GoogleAdsProvider : IAdsProvider
    {
        public event Action ON_INTERSTITIAL_FAILED;
        public event Action ON_INTERSTITIAL_CLOSED;
        public event Action ON_REWARDED_FAILED;
        public event Action ON_REWARDED_CLOSED;

        private const string kBannerId = "ca-app-pub-9340983276950968/7743235138";
        private const string kTestBannerId = "ca-app-pub-3940256099942544/6300978111";

        private const string kInterstitialId = "ca-app-pub-9340983276950968/7918688478";
        private const string kTestInterstitialId = "ca-app-pub-3940256099942544/1033173712";

        private const string kRewardedId = "ca-app-pub-9340983276950968/3664103918";
        private const string kTestRewardedId = "ca-app-pub-3940256099942544/5224354917";

        private readonly IDataService _dataService;
        private InterstitialAd interstitial;
        private RewardedAd rewarded;

        public bool IsProviderInited { get; private set; }
        public bool IsInterstitialLoaded => interstitial != null && interstitial.CanShowAd();
        public bool IsRewardedLoaded => rewarded != null && rewarded.CanShowAd();


        public void Init()
        {
            //Debug.LogFormat("INIT GOOGLE ADS CALL!");
            MobileAds.Initialize(initStatus =>
            {
                IsProviderInited = true;
                LoadGoogleInterstitialAds();
                //Debug.LogFormat("GOOGLE ADS INITED!");
                MobileAds.RaiseAdEventsOnUnityMainThread = true;
            });
        }

        public void PrepareInterstitial()
        {
            LoadGoogleInterstitialAds();
        }

        public void PrepareRewarded()
        {
            LoadGoogleRewardedAds();
        }

        public void ShowInterstitial()
        {
            interstitial?.Show();
        }

        public void ShowRewarded()
        {
            rewarded?.Show(OnEarnedReward);
        }

        private void LoadGoogleInterstitialAds()
        {
            if (interstitial != null)
            {
                interstitial.OnAdFullScreenContentClosed -= Interstitial_OnAdClosed;
                interstitial.OnAdFullScreenContentFailed -= Interstitial_OnLoadingFailed;

                interstitial.Destroy();
            }

            string adUnitId = GetInterstitialKey();
            AdRequest request = new AdRequest();
            request.Keywords.Add("unity-admob-sample");
            InterstitialAd.Load(adUnitId, request, (InterstitialAd ad, LoadAdError loadError) =>
            {
                if (loadError != null)
                {
                    Interstitial_OnLoadingFailed(loadError);
                    return;
                }
                if (ad == null)
                {
                    Interstitial_OnLoadingFailed();
                    return;
                }
                interstitial = ad;
                interstitial.OnAdFullScreenContentClosed += Interstitial_OnAdClosed;
                interstitial.OnAdFullScreenContentFailed += Interstitial_OnLoadingFailed;
            });
        }

        private void LoadGoogleRewardedAds()
        {
            if (rewarded != null)
            {
                rewarded.OnAdFullScreenContentFailed -= Rewarded_OnAdFailed;
                rewarded.OnAdFullScreenContentClosed -= Rewarded_OnAdClosed;
                rewarded.OnAdPaid -= Rewarded_OnUserEarnedReward;
                rewarded.Destroy();
            }

            var key = GetRewardedKey();
            AdRequest request = new AdRequest();
            request.Keywords.Add("unity-admob-sample");
            RewardedAd.Load(key, request, (RewardedAd ad, LoadAdError loadError) =>
            {
                if (loadError != null)
                {
                    Interstitial_OnLoadingFailed(loadError);
                    return;
                }
                else if (ad == null)
                {
                    Interstitial_OnLoadingFailed();
                    return;
                }
                rewarded = ad;
                rewarded.OnAdFullScreenContentFailed += Rewarded_OnAdFailed;
                rewarded.OnAdFullScreenContentClosed += Rewarded_OnAdClosed;
                rewarded.OnAdPaid += Rewarded_OnUserEarnedReward;
            });
        }


        private void Interstitial_OnLoadingFailed(AdError loadError)
        {
            var message = loadError.GetMessage();
            Debug.LogWarningFormat("Interstitial is failed to load! {0}", message);
            ON_INTERSTITIAL_FAILED?.Invoke();
        }

        private void Interstitial_OnLoadingFailed()
        {
            Debug.LogWarningFormat("Interstitial is failed to load!");
            ON_INTERSTITIAL_FAILED?.Invoke();
        }

        private void Interstitial_OnAdClosed()
        {
            interstitial?.Destroy();
            LoadInterstetialWithDelay(2);
            ON_INTERSTITIAL_CLOSED?.Invoke();
        }

        private void Rewarded_OnAdFailed(AdError error)
        {
            Debug.LogWarningFormat("Rewarded is failed to load!");
        }

        private void Rewarded_OnAdFailedToShow(object sender, AdErrorEventArgs args)
        {
            Debug.LogWarningFormat("Rewarded is failed to show!");
            ON_REWARDED_FAILED?.Invoke();
        }

        private void Rewarded_OnAdClosed()
        {
            rewarded?.Destroy();
            LoadRewardedWithDelay(2);
        }

        private void Rewarded_OnUserEarnedReward(AdValue adValue)
        {
            Debug.Log("Here user should get reward for RewardedAds! It's NULL for now");
        }

        private void OnEarnedReward(Reward reward)
        {
            Debug.Log("Here user should get reward for RewardedAds! It's NULL for now");
        }

        private async void LoadRewardedWithDelay(int delay = 5)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(delay));
            LoadGoogleRewardedAds();
        }

        private async void LoadInterstetialWithDelay(int delay = 5)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(delay));
            LoadGoogleInterstitialAds();
        }

        private string GetInterstitialKey()
        {
#if UNITY_EDITOR
            return kTestInterstitialId;
#else
            return kInterstitialId;
#endif
        }

        private string GetRewardedKey()
        {
#if UNITY_EDITOR
            return kTestRewardedId;
#else
            return kRewardedId;
#endif
        }
    }
}
#endif



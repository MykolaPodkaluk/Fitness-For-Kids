using System;

namespace FitnessForKids.Services
{
    public interface IAdsProvider
    {
        event Action ON_INTERSTITIAL_FAILED;
        event Action ON_INTERSTITIAL_CLOSED;
        event Action ON_REWARDED_FAILED;
        event Action ON_REWARDED_CLOSED;

        bool IsProviderInited { get; }
        bool IsInterstitialLoaded { get; }
        bool IsRewardedLoaded { get; }
        void Init();
        void ShowInterstitial();
        void ShowRewarded();
        void PrepareInterstitial();
        void PrepareRewarded();
    }

}
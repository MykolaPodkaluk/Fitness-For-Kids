#if UNITY_IOS
using System;

namespace FitnessForKids.Services
{
    public class AdsProviderStub : IAdsProvider
    {
        public bool IsProviderInited => false;
        public bool IsInterstitialLoaded => false;
        public bool IsRewardedLoaded => false;

        public event Action ON_INTERSTITIAL_FAILED;
        public event Action ON_INTERSTITIAL_CLOSED;
        public event Action ON_REWARDED_FAILED;
        public event Action ON_REWARDED_CLOSED;

        public void Init()
        {
            
        }

        public void PrepareInterstitial()
        {
            
        }

        public void PrepareRewarded()
        {
            
        }

        public void ShowInterstitial()
        {
            
        }

        public void ShowRewarded()
        {
            
        }
    }
}
#endif
using Cysharp.Threading.Tasks;
using Random = System.Random;
using UnityEngine.Purchasing;
using UnityEngine;
using System;

namespace FitnessForKids.Services
{
    public interface IAdsService
    {
        event Action ON_ADS_REMOVED;
        bool IsAdsRemoved { get; }
        void Init();
        bool TryShowInterstitialAds(int probability = 100, Action onSuccess = null, Action onFail = null);
        void ShowRewardedAds(Action onSuccess = null, Action onFail = null);
        void EnableAds(bool isEnable);
        void RemoveAds(Action<bool> callback = null);
    }


    public class AdsService : IAdsService
    {
        public event Action ON_ADS_REMOVED;

        private readonly IDataService _dataService;
        private readonly IIAPService _iapService;
        private readonly IAdsProvider _adsProvider;
        private Random _random;
        private bool _isAdsRemoved;

        public bool IsAdsRemoved => _isAdsRemoved;
        private bool _isInited => _adsProvider.IsProviderInited;
        private bool _isInterstitialReady => _adsProvider.IsInterstitialLoaded;
        private bool _isRewardedReady => _adsProvider.IsRewardedLoaded;


        public AdsService(IAdsProvider adsProvider
            , IDataService dataService
            , IIAPService iAPService)
        {
            _adsProvider = adsProvider;
            _dataService = dataService;
            _iapService = iAPService;
        }

        public void Init()
        {
            _random = new Random();
            TryInitInternal();
            _isAdsRemoved = CheckIfAdsRemoved();
        }

        public bool TryShowInterstitialAds(int probability = 100, Action onSuccess = null, Action onFail = null)
        {
            if (_isAdsRemoved)
            {
                return false;
            }

            if (_adsProvider != null && !_isInited)
            {
                TryInitInternal();
                return false;
            }

            var canShow = CanShow(probability);
            if (!canShow)
            {
                return false;
            }

            if (_isInterstitialReady)
            {
                _adsProvider.ON_INTERSTITIAL_FAILED += OnFail;
                _adsProvider.ON_INTERSTITIAL_CLOSED += OnSuccess;
                _adsProvider.ShowInterstitial();
                return true;

                void OnSuccess()
                {
                    _adsProvider.ON_INTERSTITIAL_CLOSED -= OnSuccess;
                    _adsProvider.PrepareInterstitial();
                    onSuccess?.Invoke();
                }

                void OnFail()
                {
                    _adsProvider.ON_INTERSTITIAL_FAILED -= OnFail;
                    _adsProvider.PrepareInterstitial();
                    onFail?.Invoke();
                }
            }
            else
            {
                _adsProvider.PrepareInterstitial();
                return false;
            }
        }

        public void ShowRewardedAds(Action onSuccess = null, Action onFail = null)
        {
            if (_adsProvider != null && !_isInited)
            {
                TryInitInternal();
                return;
            }

            if (_isRewardedReady)
            {
                _adsProvider.ON_REWARDED_FAILED += OnFail;
                _adsProvider.ON_REWARDED_CLOSED += OnSuccess;
                _adsProvider.ShowRewarded();

                void OnSuccess()
                {
                    _adsProvider.ON_REWARDED_CLOSED -= OnSuccess;
                    _adsProvider.PrepareInterstitial();
                    onSuccess?.Invoke();
                }

                void OnFail()
                {
                    _adsProvider.ON_REWARDED_FAILED -= OnFail;
                    _adsProvider.PrepareInterstitial();
                    onFail?.Invoke();
                }
            }
        }

        public void EnableAds(bool isEnable)
        {
            _isAdsRemoved = !isEnable;
            _dataService.UserProfileController.RemoveAds(_isAdsRemoved);
            //Debug.LogFormat("ADS enabled: {0}", isEnable);
            if (!isEnable)
            {
                ON_ADS_REMOVED?.Invoke();
            }
        }

        /// <summary>
        /// Obsolete method for NoADS Product buying.
        /// For now we just removed ADS localy as addition feature for subscription.
        /// </summary>
        /// <param name="callback">Removing progress callback</param>
        public void RemoveAds(Action<bool> callback = null)
        {
            if (!_isAdsRemoved)
            {
                _iapService.BuyProduct(PurchaseProductType.NoAds);
                _iapService.ON_PURCHASE_CALLBACK += OnTryPurchase;
            }

            void OnTryPurchase(bool isPurchased, Product product)
            {
                _iapService.ON_PURCHASE_CALLBACK -= OnTryPurchase;
                if (isPurchased)
                {
                    _isAdsRemoved = true;
                    _dataService.UserProfileController.RemoveAds(_isAdsRemoved);
                    ON_ADS_REMOVED?.Invoke();
                }
                callback?.Invoke(isPurchased);
                Debug.LogWarningFormat("Purchasing of {0} completed with result: {1}", product.metadata.localizedTitle, isPurchased);
            }
        }

        private bool CanShow(int probability)
        {
            float rnd = _random.Next(1, 101);

            if (rnd <= probability)
            {
                return true;
            }
            return false;
        }

        private void TryInitInternal()
        {
            if (_adsProvider != null && !_isInited)
            {
                _adsProvider.Init();
            }
        }

        private bool CheckIfAdsRemoved()
        {
            return _dataService.UserProfileController.IsAdsRemoved();
        }



        //Obsolete code, that check NoADS purchase from Product.
        //For now NoADS is additional feature on subscription, not separate Product.
        //private bool CheckIfAdsRemoved()
        //{
        //    var key = KeyValueIntegerKeys.ADSRemoved.ToString();
        //    if (_iapService.IsInited)
        //    {
        //        var product = _iapService.GetProduct(AppProductType.NoAds);
        //        if (product != null && product.hasReceipt)
        //        {
        //            _dataService.KeyValueStorage.SaveIntValue(key, 1);
        //            return true;
        //        }
        //        _dataService.KeyValueStorage.SaveIntValue(key, 0);
        //        return false;
        //    }
        //    else
        //    {
        //        var savedValue = _dataService.KeyValueStorage.GetIntValue(key);
        //        return savedValue != 0;
        //    }
        //}
    }
}



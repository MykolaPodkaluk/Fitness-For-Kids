using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine.Purchasing;
using System;

namespace FitnessForKids.Services.Subscription
{
    public class SubscriptionOnlineSource : BaseSubscriptionSourceAdapter
    {
        private readonly IIAPService _iapService;
        private SubscriptionManager _subscription;
        private List<Product> _products;

        public SubscriptionOnlineSource(IIAPService iapService
            , ISubscriptionDataAdapter dataServiceAdapter
            , ISubscriptionService subscriptionService) 
            : base(subscriptionService, dataServiceAdapter)
        {
            _iapService = iapService;
            InitSubscriptionManagers();
        }

        public override bool HasFreeTrial()
        {
            bool result = true;
    #if UNITY_ANDROID
            result = false;
    #endif
            if (_subscription != null)
            {
                var info = _subscription.getSubscriptionInfo();
                var isFree = info.isFreeTrial();
                if (!isFree.Equals(Result.True))
                {
                    result = false;
                }
            }
            return result;
        }

        public override bool IsSubscribed()
        {
            bool result = false;
            if (_subscription != null)
            {
                try
                {
                    var info = _subscription.getSubscriptionInfo();
                    var purchaseResult = info.isSubscribed();
                    if (purchaseResult.Equals(Result.True))
                    {
                        var expireDate = info.getExpireDate();
                        var difference = expireDate - DateTime.UtcNow;
                        var daysLeft = (int)difference.TotalDays;
                        _dataAdapter.SaveSubscriptionInfo(daysLeft, true);
                        result = true;
                    }
                }
                catch (StoreSubscriptionInfoNotSupportedException)
                {
                    var expireDate = DateTime.UtcNow.AddDays(1);
                    var difference = expireDate - DateTime.UtcNow;
                    var daysLeft = (int)difference.TotalDays;
                    _dataAdapter.SaveSubscriptionInfo(daysLeft, true);
                    result = true;
                }
            }            
            return result;
        }

        private void InitSubscriptionManagers()
        {
            _products = new List<Product>();
            Dictionary<string, string> intro = null;

            var yearSub = _iapService.GetProduct(PurchaseProductType.SubscriptionYear);
            if (yearSub != null)
            {
                _products.Add(yearSub);
            }
            var seasonSub = _iapService.GetProduct(PurchaseProductType.SubscriptionSeason);
            if (seasonSub != null)
            {
                _products.Add(seasonSub);
            }

#if UNITY_IOS
            intro = _iapService.GetAppleIntroductoryPriceDictionary();
#endif
            for (int i = 0, j = _products.Count; i < j; i++)
            {
                var product = _products[i];

                if (product.receipt != null)
                {
                    string intro_json = (intro == null || !intro.ContainsKey(product.definition.storeSpecificId))
                        ? null
                        : intro[product.definition.storeSpecificId];
                    var subManager = new SubscriptionManager(product, intro_json);
                    _subscription = subManager;
                }
            }
        }
    }

}
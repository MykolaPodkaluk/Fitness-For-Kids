using FitnessForKids.Services.Subscription;
using Cysharp.Threading.Tasks;
using UnityEngine.Purchasing;
using UnityEngine;
using System;

namespace FitnessForKids.Services
{
    /// <summary>
    /// Interface for the subscription service.
    /// </summary>
    public interface ISubscriptionService
    {
        event Action<bool> ON_PURCHASE;

        /// <summary>
        /// Initializes the subscription service.
        /// </summary>
        void Init();

        /// <summary>
        /// Checks if the user is subscribed.
        /// </summary>
        UniTask<bool> IsSubscribed();

        /// <summary>
        /// Checks if the subscription has a free trial.
        /// </summary>
        /// <returns>True if a free trial is available; otherwise, false.</returns>
        bool HasFreeTrial();

        /// <summary>
        /// Activates the season subscription.
        /// </summary>
        void ActivateSeasonSubscription();

        /// <summary>
        /// Activates the yearly subscription.
        /// </summary>
        void ActivateYearSubscription();
    }


    /// <summary>
    /// Implementation of the subscription service.
    /// </summary>
    public class SubscriptionService : ISubscriptionService
    {
        public event Action<bool> ON_PURCHASE;

        private readonly IIAPService _iapService;
        private readonly ISubscriptionDataAdapter _dataAdapter;
        private BaseSubscriptionSourceAdapter _data;
        private UniTaskCompletionSource _tcs;
        private bool _isInited;

        public SubscriptionService(IIAPService iapService, ISubscriptionDataAdapter dataServiceAdapter)
        {
            _iapService = iapService;
            _dataAdapter = dataServiceAdapter;
        }

        /// <summary>
        /// Initializes the subscription service.
        /// </summary>
        public void Init()
        {
            _dataAdapter.ON_RESET -= OnDataReset;
            _iapService.ON_PURCHASE_CALLBACK -= OnPurchase;

            _dataAdapter.ON_RESET += OnDataReset;
            _iapService.ON_PURCHASE_CALLBACK += OnPurchase;

            if (_iapService.IsInited)
            {
                _data = new SubscriptionOnlineSource(_iapService, _dataAdapter, this);
            }
            else
            {
                _data = new SubscriptionOfflineSource(_dataAdapter, this);
            }

            _isInited = true;
        }

        /// <summary>
        /// Checks the subscription status and waits until the service is initialized.
        /// </summary>
        /// <returns>A UniTask representing the completion of the check.</returns>
        public async UniTask CheckSubscription()
        {
            if (await IsSubscribed())
            {
                Debug.Log("Account Subscribed!");
                await UniTask.CompletedTask;
                return;
            }

            await _tcs.Task;
        }

        /// <summary>
        /// Checks if the subscription has a free trial.
        /// </summary>
        /// <returns>True if a free trial is available; otherwise, false.</returns>
        public bool HasFreeTrial()
        {
            return _data.HasFreeTrial();
        }

        /// <summary>
        /// Activates the season subscription.
        /// </summary>
        public void ActivateSeasonSubscription()
        {
            _iapService.BuyProduct(PurchaseProductType.SubscriptionSeason);
        }

        /// <summary>
        /// Activates the yearly subscription.
        /// </summary>
        public void ActivateYearSubscription()
        {
            _iapService.BuyProduct(PurchaseProductType.SubscriptionYear);
        }

        /// <summary>
        /// Checks if the user is subscribed.
        /// </summary>
        public async UniTask<bool> IsSubscribed()
        {
            await UniTask.WaitUntil(() => _isInited);
            return _data.IsSubscribed();
        }

        private void OnPurchase(bool success, Product product)
        {
            ON_PURCHASE?.Invoke(success);
        }

        private void OnDataReset()
        {
            _dataAdapter.ON_RESET -= OnDataReset;
            _isInited = false;
            Init();
        }
    }
}
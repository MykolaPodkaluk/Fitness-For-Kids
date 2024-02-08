using System;

namespace FitnessForKids.Services.Subscription
{
    /// <summary>
    /// Interface for the subscription data adapter.
    /// </summary>
    public interface ISubscriptionDataAdapter
    {
        /// <summary>
        /// Event raised when data needs to be reset.
        /// </summary>
        event Action ON_RESET;

        /// <summary>
        /// Checks if the user is subscribed.
        /// </summary>
        bool IsSubscribed();

        /// <summary>
        /// Saves the subscription information
        /// </summary>
        /// <param name="subscriptionDaysLeft">The number of subscription days left.</param>
        /// <param name="isSubscribed">True if the user is subscribed; otherwise, false.</param>
        void SaveSubscriptionInfo(int subscriptionDaysLeft, bool isSubscribed);
    }


    public class SubscriptionDataAdapter : ISubscriptionDataAdapter
    {
        public event Action ON_RESET;
        private readonly IDataService _dataService;

        public SubscriptionDataAdapter(IDataService dataService)
        {
            _dataService = dataService;
            dataService.ON_RESET += ON_RESET;
        }

        public bool IsSubscribed()
        {
            return _dataService.UserProfileController.IsSubscribed();
        }

        public void SaveSubscriptionInfo(int subscriptionDaysLeft, bool isSubscribed)
        {
            _dataService.UserProfileController.UpdateSubscription(subscriptionDaysLeft, isSubscribed);
        }
    }
}
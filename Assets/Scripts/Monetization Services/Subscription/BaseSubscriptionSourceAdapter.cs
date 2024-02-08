using Cysharp.Threading.Tasks;

namespace FitnessForKids.Services.Subscription
{
    /// <summary>
    /// Interface for the subscription source adapter.
    /// </summary>
    public interface ISubscriptionSourceAdapter
    {
        /// <summary>
        /// Checks if the user is subscribed.
        /// </summary>
        bool IsSubscribed();

        /// <summary>
        /// Checks if the subscription has a free trial.
        /// </summary>
        /// <returns>True if a free trial is available; otherwise, false.</returns>
        bool HasFreeTrial();
    }


    /// <summary>
    /// Abstract base class for the subscription source adapter.
    /// </summary>
    public abstract class BaseSubscriptionSourceAdapter : ISubscriptionSourceAdapter
    {
        protected readonly ISubscriptionService _subscriptionService;
        protected readonly ISubscriptionDataAdapter _dataAdapter;

        public BaseSubscriptionSourceAdapter(ISubscriptionService subscriptionService, ISubscriptionDataAdapter dataServiceAdapter)
        {
            _subscriptionService = subscriptionService;
            _dataAdapter = dataServiceAdapter;
        }

        /// <summary>
        /// Checks if the user is subscribed.
        /// </summary>
        public abstract bool IsSubscribed();

        /// <summary>
        /// Checks if the subscription has a free trial.
        /// </summary>
        /// <returns>True if a free trial is available; otherwise, false.</returns>
        public abstract bool HasFreeTrial();
    }
}
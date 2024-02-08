namespace FitnessForKids.Services.Subscription
{
    public class SubscriptionOfflineSource : BaseSubscriptionSourceAdapter
    {
        public SubscriptionOfflineSource(ISubscriptionDataAdapter dataServiceAdapter
            , ISubscriptionService subscriptionService) 
            : base(subscriptionService, dataServiceAdapter)
        {
        }

        public override bool HasFreeTrial()
        {
            return false;
        }

        public override bool IsSubscribed()
        {
            return _dataAdapter.IsSubscribed();
        }
    }
}
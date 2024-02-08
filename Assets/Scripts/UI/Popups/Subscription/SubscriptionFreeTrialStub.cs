using Cysharp.Threading.Tasks;

namespace FitnessForKids.UI.Subscription
{
    public class SubscriptionFreeTrialStub : IFreeTrialTracker
    {
        public int GetFreeDaysLeft()
        {
            int result = -1;
            return result;
        }

        public bool IsFreeNow()
        {
            bool result = false;
            return result;
        }
    }
}
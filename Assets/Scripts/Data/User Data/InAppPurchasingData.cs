using System;

namespace FitnessForKids.Data
{
    [Serializable]
    public class InAppPurchasingData
    {
        public DateTime SubscriptionDate { get; set; }
        public int SubscriptionDays { get; set; }
        public DateTime? FreeTrialEndDate { get; set; }
        public bool IsFreeTrialActivated { get; set; }
        public bool IsAdsRemoved { get; set; }

    }
}
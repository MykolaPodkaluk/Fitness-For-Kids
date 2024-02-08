using System.Collections.Generic;

namespace FitnessForKids.UI.Subscription
{
    public class SubscriptionOfferModel : IModel
    {
        public string LocalizedTitle { get; set; }
        public string PricePerMonthText { get; set; }
        public string ButtonText { get; set; }
        public string BestValueLabelText { get; set; }
        public string SaleLabelText { get; set; }
        public bool IsBestValueLabelEnable { get; set; }
        public bool IsSaleLabelEnable { get; set; }
        public List<string> OfferEntries { get; set; } = new();
    }
}
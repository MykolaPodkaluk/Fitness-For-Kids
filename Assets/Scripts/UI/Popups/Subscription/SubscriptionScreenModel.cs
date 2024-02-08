using System.Collections.Generic;

namespace FitnessForKids.UI.Subscription
{
    public class SubscriptionScreenModel : IModel
    {
        public List<string> BannersEntries { get; set; } = new();
        public SubscriptionOfferModel[] OfferModels { get; set; }
        public string TermsAndPolicy { get; set; }
        public string LocalisedSkipText { get; set; }
        public bool AllowSkip { get; set; }
        public int FreeDaysLeft { get; set; }
    }
}
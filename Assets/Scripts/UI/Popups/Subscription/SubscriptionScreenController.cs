using Cysharp.Threading.Tasks;
using FitnessForKids.Services;
using UnityEngine.Purchasing;
using UnityEngine;
using Zenject;
using System;

namespace FitnessForKids.UI.Subscription
{
    public interface ISubscriptionScreenInputHandler
    {
        public void ClickOnBuySubscription(int id);
        public void ClickOnSkip();
        public void ClickOnTextLinkWithID(string linkID);
    }

    public interface ISubscriptionScreenController : IBaseMediatedController
    {
        void SetMediator(ISubscriptionScreenMediator mediator);
    }

    public class SubscriptionScreenController
        : BaseMediatedController<ISubscriptionScreenView, SubscriptionScreenModel>
        , ISubscriptionScreenController
        , ISubscriptionScreenInputHandler
    {
        private const string kTermsLink = "terms";
        private const string kPolicyLink = "policy";
        private const string kRestoreLink = "restore";

        private const string kTextColor = "#3C83D4";

        private const string kTermsWeb = "https://www.apple.com/legal/internet-services/itunes/dev/stdeula/";
        private const string kPolicyWeb = "https://www.fivesysdev.com/Policy";
#if UNITY_IOS
        private const string kPrivacyPolicyFormat = "<color={8}>{0}</color> <u><link={1}>{2}</link></u>" +
            " <color={8}>{3}</color> <u><link={4}>{5}</link></u>" +
            " <color={8}>•</color> <u><link={6}>{7}</link></u>";
#else
        private const string kPrivacyPolicyFormat = "<color={6}>{0}</color> <u><link={1}>{2}</link></u>" +
            " <color={6}>{3}</color> <u><link={4}>{5}</link></u>";
#endif
        private const string kSubscriptionTable = "SubscriptionScreen";
        private const string kRestorePurchase = "RestorePurchase";
        private const string kBannerKeyFormat = "BannerKey_{0}";
        private const string kOurKey = "OurKey";
        private const string kAndKey = "AndKey";
        private const string kTermsKey = "TermsKey";
        private const string kPrivacyKey = "PrivacyKey";
        private const string kMonthsKey = "MonthKey";
        private const string kYearKey = "YearKey";
        private const string kPerMonthKeyFormat = "ValuePerMonthKey";
        private const string kTasksKey = "TasksKey";
        private const string kParentalControlKey = "ParentControlKey";
        private const string kProgramsKey = "TrainingProgramsKey";
        private const string kNoInternetKey = "WithoutInternetKey";
        private const string kFreeTrialKey = "FreeTrialKey";
        private const string kBuyKey = "BuyKey";
        private const string kSaleKey = "SaleKey";
        private const string kSkipKey = "SkipKey";
        private const string kBestValueKey = "BestValueKey";
        private const string kPriceFormat = "{0} {1}";
        private const int kBannersCount = 7;
        private const int kSeasonOfferId = 0;
        private const int kYearOfferId = 1;

        private readonly ISubscriptionService _subscriptionService;
        private readonly IIAPService _iapService;
        private readonly IAdsService _adsService;
        private readonly IFreeTrialTracker _freeTrialTracker;
        private ISubscriptionScreenMediator _mediator;


        public SubscriptionScreenController(DiContainer container
            , IIAPService iAPService
            , IAdsService adsService
            , IFreeTrialTracker freeTrialTracker)
        {
            _subscriptionService = container.Resolve<ISubscriptionService>();
            _iapService = iAPService;
            _adsService = adsService;
            _freeTrialTracker = freeTrialTracker;
        }

        public void SetMediator(ISubscriptionScreenMediator mediator)
        {
            _mediator = mediator;
        }

        public override void Show(Action onShow = null)
        {
            _view.Show(onShow);
        }

        public override void Hide(Action onHide)
        {
            _view.Hide(onHide);
        }

        public override void Release()
        {
            _view.Release();
        }

        public void ClickOnBuySubscription(int id)
        {
            _subscriptionService.ON_PURCHASE += OnSubscribe;

            switch (id)
            {
                case kSeasonOfferId:
                    _subscriptionService.ActivateSeasonSubscription();
                    Debug.Log("Click on SeasonSubscription");
                    break;
                case kYearOfferId:
                    _subscriptionService.ActivateYearSubscription();
                    Debug.Log("Click on YearSubscription");
                    break;

                default:
                    throw new ArgumentOutOfRangeException(
                        string.Format("Offer view ID: {0} - is out of range", id)
                        );
            }

            _view.DisallowClicksOnSubscription();
        }


        public void ClickOnSkip()
        {
            _mediator.ClosePopup();
        }

        public void ClickOnTextLinkWithID(string linkID)
        {
            switch (linkID)
            {
                case kTermsLink:
                    Application.OpenURL(kTermsWeb);
                    break;
                case kPolicyLink:
                    Application.OpenURL(kPolicyWeb);
                    break;
                case kRestoreLink:
                    _view.DisallowClicksOnSubscription();
                    _iapService.RestorePurchases(OnPurchaseRestored);

                    break;
                default:
                    break;
            }
        }

        protected override async UniTask<SubscriptionScreenModel> BuildModel()
        {
            var model = new SubscriptionScreenModel();

            var our = LocalizationController.GetLocalizedString(kSubscriptionTable, kOurKey);
            var terms = LocalizationController.GetLocalizedString(kSubscriptionTable, kTermsKey);
            var and = LocalizationController.GetLocalizedString(kSubscriptionTable, kAndKey);
            var privacy = LocalizationController.GetLocalizedString(kSubscriptionTable, kPrivacyKey);
            var restore = LocalizationController.GetLocalizedString(kSubscriptionTable, kRestorePurchase);
#if UNITY_IOS
            var privacyPolicyText = string.Format(kPrivacyPolicyFormat, our, kTermsLink, terms, and, kPolicyLink, privacy, kRestoreLink, restore, kTextColor);
#else
            var privacyPolicyText = string.Format(kPrivacyPolicyFormat, our, kTermsLink, terms, and, kPolicyLink, privacy, kTextColor);
#endif
            model.TermsAndPolicy = privacyPolicyText;
            model.LocalisedSkipText = LocalizationController.GetLocalizedString(kSubscriptionTable, kSkipKey);
            
            for (int i = 0; i < kBannersCount; i++)
            {
                var key = string.Format(kBannerKeyFormat, i);
                var bannerText = LocalizationController.GetLocalizedString(kSubscriptionTable, key);
                model.BannersEntries.Add(bannerText);
            }

            model.AllowSkip = _freeTrialTracker.IsFreeNow();
            model.FreeDaysLeft = _freeTrialTracker.GetFreeDaysLeft();

            var isSubscriptionHasFreeTrial = _subscriptionService.HasFreeTrial();
            var buttonTextKey = isSubscriptionHasFreeTrial
                ? kFreeTrialKey
                : kBuyKey;
            var buttonText = LocalizationController.GetLocalizedString(kSubscriptionTable, buttonTextKey);

            var tasksEntry = LocalizationController.GetLocalizedString(kSubscriptionTable, kTasksKey);
            var parentalEntry = LocalizationController.GetLocalizedString(kSubscriptionTable, kParentalControlKey);
            var booksEntry = LocalizationController.GetLocalizedString(kSubscriptionTable, kProgramsKey);
            var noInternetEntry = LocalizationController.GetLocalizedString(kSubscriptionTable, kNoInternetKey);
            var sale = LocalizationController.GetLocalizedString(kSubscriptionTable, kSaleKey);
            var bestValue = LocalizationController.GetLocalizedString(kSubscriptionTable, kBestValueKey);
            var perMonthFormat = LocalizationController.GetLocalizedString(kSubscriptionTable, kPerMonthKeyFormat);

            var offerViews = _view.OfferViews;
            var offerViewsLength = offerViews.Length;
            model.OfferModels = new SubscriptionOfferModel[offerViewsLength];

            for (int i = 0; i < offerViewsLength; i++)
            {
                var offerModel = new SubscriptionOfferModel();
                var titleKey = GetOfferTitleById(i);
                offerModel.LocalizedTitle = LocalizationController.GetLocalizedString(kSubscriptionTable, titleKey);

                var product = GetProductByOfferID(i);
                var offerCurrency = product.metadata.isoCurrencyCode;
                var offerPrice = product.metadata.localizedPrice;
                var priceEntry = string.Format(kPriceFormat, offerCurrency, offerPrice);

                offerModel.ButtonText = buttonText;
                offerModel.BestValueLabelText = bestValue;
                offerModel.SaleLabelText = sale;
                offerModel.IsBestValueLabelEnable = i == kYearOfferId ? true : false;
                offerModel.IsSaleLabelEnable = i == kYearOfferId ? true : false;

                var pricePerMonth = GetPricePerMonth(offerPrice, i);
                var perMonthEntry = string.Format(perMonthFormat, offerCurrency, pricePerMonth);
                offerModel.PricePerMonthText = perMonthEntry;

                offerModel.OfferEntries = new()
                {
                    priceEntry,
                    tasksEntry,
                    parentalEntry,
                    booksEntry,
                    noInternetEntry
                };
                model.OfferModels[i] = offerModel;
            }

            return await UniTask.FromResult(model);
        }

        protected override UniTask DoOnInit(ISubscriptionScreenView view)
        {
            view.SetInputHandler(this);

            var banners = _model.BannersEntries;
            var bannersCount = banners.Count;
            for (int i = 0, j = bannersCount; i < j; i++)
            {
                view.AddBannerEntry(banners[i]);
            }
            view.IndicatorsController.Init(bannersCount);

            var offerViews = view.OfferViews;
            var offerModels = _model.OfferModels;

            for (int i = 0, j = offerModels.Length; i < j; i++)
            {
                var offerView = offerViews[i];
                var offerModel = offerModels[i];
                offerView.SetTitle(offerModel.LocalizedTitle);
                offerView.SetPricePerMonthText(offerModel.PricePerMonthText);
                offerView.SetButtonText(offerModel.ButtonText);
                offerView.ShowDetailedLabel(offerModel.IsBestValueLabelEnable, offerModel.BestValueLabelText);
                offerView.ShowSaleLabel(offerModel.IsSaleLabelEnable, offerModel.SaleLabelText);

                var offerEntries = offerModel.OfferEntries;
                for (int l = 0, m = offerEntries.Count; l < m; l++)
                {
                    offerView.AddEntry(offerEntries[l]);
                }
                offerView.SetInputHandler(this);
                offerView.Init(i);
            }

            view.AllowSkip(_model.AllowSkip, _model.FreeDaysLeft);
            view.SetSkipText(_model.LocalisedSkipText);
            view.SetTermsAndPrivacyText(_model.TermsAndPolicy);

            return UniTask.CompletedTask;
        }

        private Product GetProductByOfferID(int id)
        {
            switch (id)
            {
                case kSeasonOfferId: return _iapService.GetProduct(PurchaseProductType.SubscriptionSeason);
                case kYearOfferId: return _iapService.GetProduct(PurchaseProductType.SubscriptionYear);
                default:
                    throw new ArgumentOutOfRangeException(
                        string.Format("Offer view ID: {0} - is out of range", id)
                        );
            }
        }

        private string GetOfferTitleById(int id)
        {
            string result = "";
            switch (id)
            {
                case kSeasonOfferId:
                    result = kMonthsKey; 
                    break;
                case kYearOfferId:
                    result = kYearKey;
                    break;
            }
            return result;
        }

        private string GetPricePerMonth(decimal price, int offerID)
        {
            decimal pricePerMonth = 0;
            switch (offerID)
            {
                case kSeasonOfferId:
                    pricePerMonth = price / 3;
                    break;
                case kYearOfferId:
                    pricePerMonth = price / 12;
                    break;
            }
            string result = pricePerMonth.ToString("F2");
            return result;
        }

        private async void OnPurchaseRestored(bool isRestored)
        {
            if (isRestored)
            {
                _view.DisallowClicksOnSubscription();
                var isSubscribed = await _subscriptionService.IsSubscribed();
                if (isSubscribed)
                {
                    _subscriptionService.Init();
                    _mediator.ClosePopup();
                }
                else
                {
                    _view.AllowClicksOnSubscription();
                }
            }
            else
            {
                _view.AllowClicksOnSubscription();
            }
        }

        private void OnSubscribe(bool success)
        {
            Debug.Log("Purchasing of subscription complete with result: " + success);
            _subscriptionService.ON_PURCHASE -= OnSubscribe;
            _view.AllowClicksOnSubscription();
            if (success)
            {
                _subscriptionService.Init();
                _adsService.EnableAds(false);
                _mediator.ClosePopup();
            }
        }
    }
}



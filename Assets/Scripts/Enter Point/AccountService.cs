using FitnessForKids.UI.Subscription;
using Cysharp.Threading.Tasks;
using FitnessForKids.UI;


namespace FitnessForKids.Services
{
    public interface IAccountService
    {
        UniTask CheckAllAsync();
        UniTask CheckSubscriptionAsync();
    }

    public class AccountService : IAccountService
    {
        private const string kCheckSkillPlan = "SkillPlanControl";
        private const string kCheckNameKey = "UserNameControl";

        private readonly IDataService _dataService;
        private readonly ISubscriptionScreenMediator _subscriptionMediator;
        private readonly ISubscriptionService _subscriptionService;
        private readonly IFreeTrialTracker _subscriptionFreeTrial;
        private readonly IAdsService _adsService;
        private readonly IProfileRegistrationMediator _profileRegistrationScreenMediator;

        public AccountService(IDataService dataService
            , ISubscriptionService subscriptionService
            , ISubscriptionScreenMediator subscriptionMediator
            , IFreeTrialTracker subscriptionFreeTrial
            , IAdsService adsService
            , IProfileRegistrationMediator profileRegistrationScreenMediator)
        {
            _dataService = dataService;
            _subscriptionService = subscriptionService;
            _subscriptionMediator = subscriptionMediator;
            _subscriptionFreeTrial = subscriptionFreeTrial;
            _adsService = adsService;
            _profileRegistrationScreenMediator = profileRegistrationScreenMediator;
        }


        public async UniTask CheckAllAsync()
        {
            var isSubscribed = await _subscriptionService.IsSubscribed();
            if (!isSubscribed)
            {
                await CheckSubscriptionAsync();
                isSubscribed = await _subscriptionService.IsSubscribed();
            }
            _adsService.EnableAds(!isSubscribed);
            CheckProfileRegistrationAsync();
        }

        public async UniTask CheckSubscriptionAsync()
        {
            var tcs = new UniTaskCompletionSource();
#if UNITY_EDITOR
            _subscriptionMediator.CreatePopup();
            _subscriptionMediator.ON_CLOSE += OnClose;

            void OnClose()
            {
                _subscriptionMediator.ON_CLOSE -= OnClose;
                tcs.TrySetResult();
            }

            await tcs.Task;
#else
            bool isFreePeriodNow = _subscriptionFreeTrial.IsFreeNow();
            if (!isFreePeriodNow)
            {
#if UNITY_IOS
                //_parentGateMediator.CreatePopup();
#endif
                _subscriptionMediator.CreatePopup();
                _subscriptionMediator.ON_CLOSE += OnClose;

                void OnClose()
                {
                    _subscriptionMediator.ON_CLOSE -= OnClose;
                    tcs.TrySetResult();
                }

                await tcs.Task;
            }
            else
            {
                await UniTask.CompletedTask;
            }
#endif
        }

        public void CheckProfileRegistrationAsync()
        {
            //UnityEngine.Debug.Log("Entered to " + nameof(CheckProfileRegistrationAsync));
            var hasActiveProfile = _dataService.UserProfileController.HasActiveProfile;
            if (!hasActiveProfile)
            {
                _profileRegistrationScreenMediator.CreatePopup();
                _profileRegistrationScreenMediator.ON_CLOSE += OnClose;
            }

            void OnClose()
            {
                _profileRegistrationScreenMediator.ON_CLOSE -= OnClose;
            }

            //UnityEngine.Debug.Log("Exit from " + nameof(CheckProfileRegistrationAsync));
        }
    }
}
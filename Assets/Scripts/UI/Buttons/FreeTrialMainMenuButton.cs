using FitnessForKids.UI.Subscription;
using FitnessForKids.Services;
using Cysharp.Threading.Tasks;
using UnityEngine.UI;
using UnityEngine;
using Zenject;
using TMPro;

namespace FitnessForKids.UI.Helpers
{
    public class FreeTrialMainMenuButton : MonoBehaviour
    {
        private const string kTable = "SubscriptionScreen";
        private const string kFreeTrialKey = "FreeTrialMenuButtonKey";

        [Inject] private ISubscriptionService _subscriptionService;
        [Inject] private IFreeTrialTracker _freeTrialTracker;
        [Inject] private ISubscriptionScreenMediator _subscriptionMediator;
        [SerializeField] private Button _button;
        [SerializeField] private TMP_Text _text;

        private async void Start()
        {
            await UniTask.Delay(500);
            var isSubscribed = await CheckSubscription();
            if (!isSubscribed)
            {
                UpdateText();
                _button.onClick.AddListener(OnButtonClick);
                _subscriptionService.ON_PURCHASE += DoOnSubscriptionPurchase;
                LocalizationController.OnLanguageChanged.AddListener(UpdateText);
            }
            gameObject.SetActive(!isSubscribed);
        }

        private void OnDestroy()
        {
            _button.onClick.RemoveListener(OnButtonClick);
            _subscriptionService.ON_PURCHASE -= DoOnSubscriptionPurchase;
            LocalizationController.OnLanguageChanged.RemoveListener(UpdateText);
        }

        private async void CheckFreeTrialButton()
        {
            var isSubscribed = await CheckSubscription();
            if (isSubscribed)
            {
                _button.onClick.RemoveListener(OnButtonClick);
                _subscriptionService.ON_PURCHASE -= DoOnSubscriptionPurchase;
                LocalizationController.OnLanguageChanged.RemoveListener(UpdateText);
            }
            gameObject.SetActive(!isSubscribed);
        }

        private void DoOnSubscriptionPurchase(bool isPurchased)
        {
            CheckFreeTrialButton();
        }

        private void OnButtonClick()
        {
            _subscriptionMediator.CreatePopup();
        }

        private void UpdateText()
        {
            var freeDaysLeft = GetDaysLeft();
            var textFormat = LocalizationController.GetLocalizedString(kTable, kFreeTrialKey);
            _text.text = string.Format(textFormat, freeDaysLeft);
        }

        private async UniTask<bool> CheckSubscription()
        {
            var isSubscribed = await _subscriptionService.IsSubscribed();
            return isSubscribed;
        }

        private int GetDaysLeft()
        {
            var daysLeft = _freeTrialTracker.GetFreeDaysLeft();
            return daysLeft;
        }
    }
}
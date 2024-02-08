using FitnessForKids.UI.Animation;
using UnityEngine.UI;
using UnityEngine;
using System.Linq;
using System;
using TMPro;

namespace FitnessForKids.UI.Subscription
{
    public interface ISubscriptionScreenView : IView
    {
        IIndicatorsController IndicatorsController { get; }
        ISubscriptionOfferView[] OfferViews { get; }
        void AddBannerEntry(string text);
        void Init();
        void Init(Camera camera, int layer);
        void SetInputHandler(ISubscriptionScreenInputHandler inputHandler);
        void AllowSkip(bool allow, int daysLeft);
        void AllowClicksOnSubscription();
        void DisallowClicksOnSubscription();
        void SetTermsAndPrivacyText(string text);
        void SetSkipText(string text);
    }

    public class SubscriptionScreenView : MonoBehaviour, ISubscriptionScreenView, IClickableTextListener
    {
        [SerializeField] private IndicatorsController _indicatorsController;
        [SerializeField] private SubscriptionEntry[] _infoEntries;
        [SerializeField] private SubscriptionOfferView[] _offerViews;
        [SerializeField] private Button _skipButton;
        [SerializeField] private TMP_Text _skipText;
        [SerializeField] private TMP_Text _termsAndPolicyText;
        [SerializeField] private BaseViewAnimator _animator;
        [SerializeField] private Canvas _canvas;
        [SerializeField] private GameObject[] _decors;
        [SerializeField] private ClickableText _clickableText;
        private ISubscriptionScreenInputHandler _inputHandler;

        public IIndicatorsController IndicatorsController => _indicatorsController;
        public ISubscriptionOfferView[] OfferViews => _offerViews;

        public void Init(Camera camera, int layer)
        {
            _canvas.worldCamera = camera;
            _canvas.sortingOrder = layer;

            _clickableText.Init(this, camera);
        }

        public void Init()
        {
            _clickableText.Init(this, null);
        }

        public void SetInputHandler(ISubscriptionScreenInputHandler inputHandler)
        {
            _inputHandler = inputHandler;
        }

        public void AddBannerEntry(string text)
        {
            var freeEntry = _infoEntries.FirstOrDefault(x => !x.IsInited);
            if (freeEntry == null)
            {
                Debug.LogWarning("There is no empty entry available!");
                return;
            }

            freeEntry.Init(text);
        }

        public void Show(Action onShow)
        {
            _animator.AnimateShowing(() =>
            {
                ShowOffers();
                _skipButton.onClick.AddListener(OnSkipClick);
                onShow?.Invoke();
            });
        }

        public void AllowSkip(bool allow, int daysLeft)
        {
            _skipButton.gameObject.SetActive(allow);
            //we have ability to show days count of free period left 
        }

        public void Hide(Action onHide)
        {
            _skipButton.onClick.RemoveListener(OnSkipClick);
            HideOffers();
            _animator.AnimateHiding(() =>
            {
                onHide?.Invoke();
            });
        }

        public void OnTextClick(string linkID)
        {
            _inputHandler.ClickOnTextLinkWithID(linkID);
        }

        public void AllowClicksOnSubscription()
        {
            ShowOffers();
        }

        public void DisallowClicksOnSubscription()
        {
            HideOffers();
        }

        public void Release()
        {
            Destroy(gameObject);
        }

        private void OnSkipClick()
        {
            _inputHandler.ClickOnSkip();
        }

        public void SetTermsAndPrivacyText(string text)
        {
            _termsAndPolicyText.text = text;
        }

        public void SetSkipText(string text)
        {
            _skipText.text = text;
        }

        private void ShowOffers()
        {
            for (int i = 0, j = _offerViews.Length; i < j; i++)
            {
                _offerViews[i].Show(null);
            }
        }

        private void HideOffers()
        {
            for (int i = 0, j = _offerViews.Length; i < j; i++)
            {
                _offerViews[i].Hide(null);
            }
        }
    }
}



using UnityEngine.UI;
using UnityEngine;
using System.Linq;
using System;
using TMPro;

namespace FitnessForKids.UI.Subscription
{
    public interface ISubscriptionOfferView : IView
    {
        void Init(int id);
        void SetInputHandler(ISubscriptionScreenInputHandler inputHandler);
        void SetTitle(string title);
        void SetPricePerMonthText(string text);
        void AddEntry(string text);
        void SetButtonText(string text);
        void ShowDetailedLabel(bool isShow, string labelText = "");
        void ShowSaleLabel(bool isShow, string labelText = "");
    }

    public class SubscriptionOfferView : MonoBehaviour, ISubscriptionOfferView
    {
        [SerializeField] private TMP_Text _titleText;
        [SerializeField] private TMP_Text _pricePerMonthText;
        [SerializeField] private TMP_Text _detailLabelText;
        [SerializeField] private TMP_Text _saleLabelText;
        [SerializeField] private TMP_Text _buttonText;
        [SerializeField] private GameObject _detailLabel;
        [SerializeField] private GameObject _saleLabel;
        [SerializeField] private Button _buyButton;
        [SerializeField] private SubscriptionEntry[] _entries;
        private ISubscriptionScreenInputHandler _inputHandler;
        private int _id;

        public void Init(int id)
        {
            _id = id;
        }

        public void AddEntry(string text)
        {
            var freeEntry = _entries.FirstOrDefault(x => !x.IsInited);
            if (freeEntry == null)
            {
                Debug.LogWarning("There is no empty entry available!");
                return;
            }

            freeEntry.Init(text);
        }

        public void Show(Action onShow)
        {
            _buyButton.onClick.AddListener(OnButtonClick);
            onShow?.Invoke();
        }

        public void Hide(Action onHide)
        {
            _buyButton.onClick.RemoveListener(OnButtonClick);
            onHide?.Invoke();
        }

        public void Release()
        {
            
        }

        public void SetInputHandler(ISubscriptionScreenInputHandler inputHandler)
        {
            _inputHandler = inputHandler;
        }

        public void SetTitle(string title)
        {
            _titleText.text = title;
        }

        public void SetPricePerMonthText(string text)
        {
            _pricePerMonthText.text = text;
        }

        public void ShowDetailedLabel(bool isShow, string labelText = "")
        {
            _detailLabel.SetActive(isShow);
            _detailLabelText.text = labelText;
        }

        public void ShowSaleLabel(bool isShow, string labelText = "")
        {
            _saleLabel.SetActive(isShow);
            _saleLabelText.text = labelText;
        }

        public void SetButtonText(string text)
        {
            _buttonText.text = text;
        }

        private void OnButtonClick()
        {
            _inputHandler.ClickOnBuySubscription(_id);
        }
    }
}
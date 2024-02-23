using UnityEngine.UI;
using UnityEngine;
using System;
using TMPro;
using FitnessForKids.Data;
using UnityEngine.Purchasing.MiniJSON;

namespace FitnessForKids.UI
{
    public interface ITrainingProgramPanelView : IView
    {
        void Init(ProgramType type, bool isActive, int id, string title, string description, Sprite icon, Sprite iconBase, Sprite panelBase, Color color);
        void SetInputHandler(ITrainingProgramsScreenInputHandler inputHandler);
        void ShowNewLabel(bool isShow);
    }

    public class TrainingProgramPanelView : MonoBehaviour, ITrainingProgramPanelView
    {
        #region FIELDS

        [Header("COMPONENTS:")]
        [SerializeField] private TMP_Text titleLabel;
        [SerializeField] private TMP_Text descriptionLabel;
        [SerializeField] Image iconImage;
        [SerializeField] private Image iconBaseImage;
        [SerializeField] private Image panelBaseImage;
        [SerializeField] private Button _moreButton;
        [SerializeField] private Button _startButton;
        [SerializeField] private Button unlockButton;
        [SerializeField] private GameObject _newLabel;
        [SerializeField] private Toggle _selectToggle;

        private ProgramType _type;
        private int _id;
        private ITrainingProgramsScreenInputHandler _inputHandler;

        #endregion

        public void Init(ProgramType type, bool isActive, int id, string title, string description, Sprite icon, Sprite iconBase, Sprite panelBase, Color color)
        {
            _type = type;
            _selectToggle.isOn = isActive;
            _id = id;
            titleLabel.text = title;
            titleLabel.color = color;
            descriptionLabel.text = description;
            descriptionLabel.fontSize = 30;
            iconImage.sprite = icon;
            iconBaseImage.sprite = iconBase;
            panelBaseImage.sprite = panelBase; 
        }

        public void Show(Action onShow)
        {
            _startButton.onClick.AddListener(OnButtonClick);
            _selectToggle.onValueChanged.AddListener(OnSelectToggleClick);
            _moreButton.onClick.AddListener(OnMoreButtonClick);
            onShow?.Invoke();
        }

        public void Hide(Action onHide)
        {
            _selectToggle.onValueChanged.RemoveListener(OnSelectToggleClick);
            _startButton.onClick.RemoveListener(OnButtonClick);
            _startButton.onClick.RemoveListener(OnMoreButtonClick);
            onHide?.Invoke();
        }

        public void Release()
        {

        }
        public void SetInputHandler(ITrainingProgramsScreenInputHandler inputHandler)
        {
            _inputHandler = inputHandler;
        }

        public void ShowNewLabel(bool isShow)
        {
            _newLabel.SetActive(isShow);
        }

        private void OnButtonClick()
        {
            _inputHandler.ClickOnStartTraining(_id);
        }

        private void OnSelectToggleClick(bool isOn)
        {
            _inputHandler.ClickOnSelectToggle(_type, isOn);
        }

        private void OnMoreButtonClick()
        {
            _inputHandler.ClickOnMoreDetails(_id);
        }
    }
}
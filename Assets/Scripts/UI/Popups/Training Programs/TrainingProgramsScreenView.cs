using DanielLochner.Assets.SimpleScrollSnap;
using FitnessForKids.UI.Animation;
using System.Collections.Generic;
using FitnessForKids.Data;
using UnityEngine.UI;
using UnityEngine;
using DG.Tweening;
using System;
using TMPro;

namespace FitnessForKids.UI
{
    public interface ITrainingProgramsScreenView : IView
    {
        IIndicatorsController IndicatorsController { get; }
        List<TrainingProgramPanelView> ProgramViews { get; }
        void AllowClicksOnPrograms();
        void DisallowClicksOnPrograms();
        void AddTrainingProgramView(ProgramType type, bool isActive, int id, string title, string description, Sprite icon, Sprite iconBase, Sprite panelBase, Color color);
        void SetInputHandler(ITrainingProgramsScreenInputHandler inputHandler);
        void ActivateScrollSnap();
        void ShowProgramDescription(string description);
    }

    public class TrainingProgramsScreenView : MonoBehaviour, ITrainingProgramsScreenView
    {
        [Header("COMPONENTS:")]
        [SerializeField] private Button _closeButton;
        [SerializeField] private Button _returnButton;
        [SerializeField] private Button _confirmButton;
        [SerializeField] private TMP_Text _programDescriptionLebel;
        [SerializeField] private CanvasGroup _descriptionPopUp;
        [SerializeField] private BaseViewAnimator _animator;
        [SerializeField] private IndicatorsController _indicatorsController;
        [SerializeField] private List<TrainingProgramPanelView> _programViews = new List<TrainingProgramPanelView>();
        [SerializeField] private SimpleScrollSnap _simpleScrollSnap;

        [SerializeField] private Transform _programsContainer;
        [SerializeField] private TrainingProgramPanelView _programViewPrefab;

        private ITrainingProgramsScreenInputHandler _inputHandler;
        public IIndicatorsController IndicatorsController => _indicatorsController;
        public List<TrainingProgramPanelView> ProgramViews => _programViews;

        public void SetInputHandler(ITrainingProgramsScreenInputHandler inputHandler)
        {
            _inputHandler = inputHandler;
        }

        public void ActivateScrollSnap()
        {
            _simpleScrollSnap.enabled = true;
        }

        public void AddTrainingProgramView(ProgramType type, bool isActive, int id, string title, string description, Sprite icon, Sprite iconBase, Sprite panelBase, Color color)
        {
            Debug.Log(Time.time);
            var programView = Instantiate(_programViewPrefab, _programsContainer);
            programView.SetInputHandler(_inputHandler);
            programView.Init(type, isActive, id, title, description, icon, iconBase, panelBase, color);
            _programViews.Add(programView);
        }

        public void AllowClicksOnPrograms()
        {
            ShowProgramPanels();
        }

        public void DisallowClicksOnPrograms()
        {
            HideProgramPanels();
        }

        public void Show(Action onShow)
        {
            _animator.AnimateShowing(() =>
            {
                ShowProgramPanels();
                _closeButton.onClick.AddListener(OnClosedClick);
                _confirmButton.onClick.AddListener(OnClosedClick);
                _returnButton.onClick.AddListener(OnReturnClick);
                onShow?.Invoke();
            });
        }

        public void Hide(Action onHide)
        {
            _closeButton.onClick.RemoveListener(OnClosedClick);
            _confirmButton.onClick.RemoveListener(OnClosedClick);
            _returnButton.onClick.RemoveListener(OnClosedClick);
            HideProgramPanels();
            _animator.AnimateHiding(() =>
            {
                onHide?.Invoke();
            });
        }

        private void OnClosedClick()
        {
            _inputHandler.ClickOnClose();
        }

        private void OnReturnClick()
        {
            _inputHandler.ClickOnReturn();
        }

        public void Release()
        {
            Destroy(gameObject);
        }

        private void ShowProgramPanels()
        {
            for (int i = 0, j = _programViews.Count; i < j; i++)
            {
                _programViews[i].Show(null);
            }
        }

        private void HideProgramPanels()
        {
            for (int i = 0, j = _programViews.Count; i < j; i++)
            {
                _programViews[i].Hide(null);
            }
        }

        public void ShowProgramDescription(string description)
        {
            _programDescriptionLebel.text = description;

            _descriptionPopUp.gameObject.SetActive(true);
            _descriptionPopUp.alpha = 0;
            _descriptionPopUp.DOFade(1, 0.5f);

            _returnButton.onClick.RemoveAllListeners();
            _returnButton.onClick.AddListener(CloseProgramDescription);
        }

        public void CloseProgramDescription()
        {
            _descriptionPopUp.DOFade(0, 0.5f).OnComplete(() => _descriptionPopUp.gameObject.SetActive(false));

            _returnButton.onClick.RemoveAllListeners();
            _returnButton.onClick.AddListener(OnReturnClick);
        }
    }
}
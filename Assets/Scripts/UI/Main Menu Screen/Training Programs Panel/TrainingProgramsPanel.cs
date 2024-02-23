using System.Collections.Generic;
using FitnessForKids.UI.Helpers;
using FitnessForKids.Services;
using FitnessForKids.Data;
using System.Collections;
using UnityEngine.UI;
using System.Linq;
using UnityEngine;
using Zenject;
using System;
using Cysharp.Threading.Tasks;

namespace FitnessForKids.UI
{
    public class TrainingProgramsPanel : MonoBehaviour
    {
        [SerializeField] private LinePointsController _linePointsController;
        [SerializeField] private StartTrainingButton _startTrainingButton;
        [SerializeField] private TrainingProgramsButton _addTrainingProgramButton;
        [SerializeField] private RectTransform _programButtonsContainer;
        [SerializeField] private RectTransform _buttonsBackground;
        [SerializeField] private List<TrainingProgramSelectableButton> _programButtons;
        [Inject] private ITrainingService _trainingService;
        [Inject] private IDataService _dataService;

        private List<TrainingProgramSettings> _programSettings = new List<TrainingProgramSettings>();
        private TrainingProgramSelectableButton _selectedButton;
        private const float kButtonSegment = 222f; // Distance between centers of the adjacent training program buttons
        private List<TrainingProgramSelectableButton> _activeProgramButtons;
        private int _activeButtonsCount;

        public event Action OnProgramSelected;
        private void Start()
        {
            RebuildButtonsLayout(); // Required to avoid display failures of layout group elements
            Subscribe();
        }

        private async void OnEnable()
        {
            if (_dataService.UserProfileController.HasActiveProfile)
            {
                UpdateProgramButtons();
                await UniTask.Delay(100);
                UpdateProgramButtons();
            }
        }

        private void RebuildButtonsLayout()
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(_programButtonsContainer);
            var backgroundSizeX = GetDistanceBetweenExtremeButtons();
            _buttonsBackground.sizeDelta = new Vector2(backgroundSizeX, _buttonsBackground.sizeDelta.y);
        }

        private void Subscribe()
        {
            //_addTrainingProgramButton.OnTrainingSettingsUpdated += UpdateProgramButtons;
            for (int i = 0; i < _programButtons.Count; i++)
            {
                var button = _programButtons[i];
                button.OnClick += () => OnTrainingButtonClicked(button);
            }
        }

        private void OnTrainingButtonClicked(TrainingProgramSelectableButton button)
        {
            _trainingService.SelectTrainingProgram(button.ProgramType);
            if (_selectedButton != null && button != _selectedButton)
            {
                _selectedButton.Deselect();
            }
            _selectedButton = button;
            _selectedButton.Select();
        }

        public void Initialize(List<TrainingProgramSettings> programSettings)
        {
            _programSettings = programSettings.Count > 0 ? programSettings : DefaultProgramSettings();
            UpdateProgramButtons();
        }

        private List<TrainingProgramSettings> DefaultProgramSettings()
        {
            var settings = new List<TrainingProgramSettings>();
            foreach (ProgramType programType in Enum.GetValues(typeof(ProgramType)))
            {
                if (programType != ProgramType.Custom)
                {
                    var programSettings = new TrainingProgramSettings(programType, true);
                    settings.Add(programSettings);
                }
            }
            return settings;
        }

        public void UpdateProgramButtons()
        {
            foreach (var button in _programButtons)
            {
                ProgramType buttonType = button.ProgramType;
                TrainingProgramSettings programSettings = _programSettings.FirstOrDefault(settings => settings.Type == buttonType);
                button.gameObject.SetActive(programSettings.IsActive);
            }
            RebuildButtonsLayout();
            StartCoroutine(SelectAvailableButton());
        }

        private IEnumerator SelectAvailableButton()
        {
            yield return new WaitUntil(() => _trainingService != null);
            if (_activeButtonsCount > 0)
            {
                var selectedButton = _activeProgramButtons[0];
                OnTrainingButtonClicked(selectedButton);
                _linePointsController.SetTargetPosition(selectedButton.RectTransform);
                _startTrainingButton.SetInteractable(true);
                _linePointsController.SetLinesActive(true);
            }
            else
            {
                _startTrainingButton.SetInteractable(false);
                _linePointsController.SetLinesActive(false);
            }
        }

        private float GetDistanceBetweenExtremeButtons()
        {
            _activeButtonsCount = 0;
            _activeProgramButtons = new List<TrainingProgramSelectableButton>();
            foreach (var button in _programButtons)
            {
                if (button.gameObject.activeInHierarchy)
                {
                    _activeButtonsCount++;
                    _activeProgramButtons.Add(button);
                }
            }
            float distance = kButtonSegment * _activeButtonsCount;
            if (_activeButtonsCount > 1) distance += kButtonSegment;
            return distance;
        }

        public void HideLineSelector()
        {
            _linePointsController.SetLinesActive(false);
        }
    }
}
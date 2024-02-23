using UnityEngine.UI;
using UnityEngine;
using Zenject;
using System;

namespace FitnessForKids.UI.Helpers
{
    public class TrainingProgramsButton : MonoBehaviour
    {
        [SerializeField] private Button _button;
        [SerializeField] private TrainingProgramsPanel _trainingProgramsPanel;
        [Inject] private ITrainingProgramsMediator _trainingProgramsMediator;
        [Inject] private IUIScreenController _uiScreenController;

        public event Action OnTrainingSettingsUpdated;

        private void Start()
        {
            _trainingProgramsMediator.OnTrainingSettingsUpdated += OnTrainingSettingsUpdatedHandler;
            _button.onClick.AddListener(OnButtonClick);
            //WarmupPopups();
        }

        private void OnDestroy()
        {
            _button.onClick.RemoveListener(OnButtonClick);
        }

        private void OnButtonClick()
        {
            _trainingProgramsMediator.CreatePopup(() =>
            { 
                _uiScreenController.SetActiveMainMenu(false);
                _trainingProgramsPanel.UpdateProgramButtons();
            });
        }

        private void WarmupPopups()
        {
            _trainingProgramsMediator.CreatePopup(() =>
            _trainingProgramsMediator.ClosePopup());
        }

        private void OnTrainingSettingsUpdatedHandler()
        {
            OnTrainingSettingsUpdated?.Invoke();
        }
    }
}
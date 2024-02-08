using FitnessForKids.Services;
using FitnessForKids.Data;
using UnityEngine;

namespace FitnessForKids.UI
{
    public interface IUIScreenController
    {
        void SetActiveMainMenu(bool isActive);
    }

    /// <summary>
    /// Shows and hides basic UI screens
    /// </summary>
    public class UIScreenController : IUIScreenController
    {
        private IDataService _dataService;
        private IMainMenuScreenController _mainMenuScreen;
        private ITrainingController _trainingScreen;
        private ITrainingService _trainingService;
        private IProfileRegistrationMediator _profileRegistrationMediator;

        public UIScreenController(IDataService dataService, 
            IMainMenuScreenController mainMenuScreen,
            ITrainingController trainingScreen,
            IProfileRegistrationMediator profileRegistrationMediator,
            ITrainingService trainingService)
        {
            _dataService = dataService;
            _mainMenuScreen = mainMenuScreen;
            _trainingScreen = trainingScreen;
            _profileRegistrationMediator = profileRegistrationMediator;
            _trainingService = trainingService;

            Init();
        }

        private void Init()
        {
            _dataService.UserProfileController.OnProfilesChanged += UpdateActiveProfileInfo;
            _dataService.UserProfileController.CheckActiveProfile();

            _trainingService.OnTrainingStarted += StartTraining;
            _trainingService.OnTrainingStoped += StopTraining;
        }

        public void OpenRegistrationScreen()
        {
            _profileRegistrationMediator.CreatePopup();
        }

        private void UpdateActiveProfileInfo(IUserProfileData activeProfile)
        {
            if (!_dataService.UserProfileController.HasActiveProfile) OpenRegistrationScreen();

            if (activeProfile != null)
            {
                if (!((MainMenuScreenView)_mainMenuScreen.View).gameObject.activeInHierarchy)
                {
                    _mainMenuScreen.View.Show(null);
                }
                _mainMenuScreen.View.SetPlayerProfileText(activeProfile.Name, activeProfile.Age);
                _mainMenuScreen.UpdateStatistics();
                _mainMenuScreen.UpdateProgramPanel();
            }
            else
            {
                _mainMenuScreen.View.Hide(null);
                _trainingScreen.View.Hide(null);
            }
        }

        private void StartTraining()
        {
            _trainingScreen.View.Show(null);
            _mainMenuScreen.View.Hide(null);
        }

        private void StopTraining()
        {
            _mainMenuScreen.View.Show(_mainMenuScreen.UpdateStatistics);
            _trainingScreen.StopTraining();
        }

        public void SetActiveMainMenu(bool isActive)
        {
            _mainMenuScreen.View.GameObject.SetActive(isActive);
        }
    }
}
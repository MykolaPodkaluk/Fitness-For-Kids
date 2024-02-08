using Cysharp.Threading.Tasks;
using FitnessForKids.Services;
using System.Collections;
using UnityEngine;
using Zenject;

namespace FitnessForKids.UI
{
    public interface IMainMenuScreenController
    {
        IMainMenuScreenView View { get; }
        IWeeklyStatisticsPanelController WeeklyStatisticsPanel { get; }
        void UpdateStatistics();
        void UpdateProgramPanel();
    }

    public class MainMenuScreenController : IMainMenuScreenController
    {
        private IMainMenuScreenView view;
        private IWeeklyStatisticsPanelController weeklyStatisticsPanel;
        public IMainMenuScreenView View => view;
        public IWeeklyStatisticsPanelController WeeklyStatisticsPanel => weeklyStatisticsPanel;
        [Inject] IDataService _dataService;
        private IProfileRegistrationMediator _profileRegistrationMediator;

        public MainMenuScreenController(IMainMenuScreenView view, 
            IWeeklyStatisticsPanelController weeklyStatisticsPanel,
            IProfileRegistrationMediator profileRegistrationMediator)
        {
            this.view = view;
            this.weeklyStatisticsPanel = weeklyStatisticsPanel;
            _profileRegistrationMediator = profileRegistrationMediator;

            view.OnShow += UpdateStatistics;
            _ = Initialize();
        }

        private async UniTask Initialize()
        {
            await UniTask.Delay(500);
            view.Initialize(
                _dataService.UserProfileController.CanCreateProfile,
                OpenRegistrationScreen
                );
        }

        private void OpenRegistrationScreen()
        {
            _profileRegistrationMediator.CreatePopup();
        }

        public void UpdateStatistics()
        {
            var weeklyStatistics = _dataService.UserProfileController.GetWeeklyStatistics();
            weeklyStatisticsPanel.ShowWeeklyStatistics(weeklyStatistics);
        }

        public void UpdateProgramPanel()
        {
            view.UpdateProgramPanel();
        }
    }
}
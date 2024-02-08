using FitnessForKids.Data.Addressables;
using FitnessForKids.Services.UI;
using System.Collections.Generic;
using FitnessForKids.Services;
using Cysharp.Threading.Tasks;
using FitnessForKids.Data;
using UnityEngine;
using System;
using Zenject;

namespace FitnessForKids.UI
{
    public interface ITrainingProgramsMediator : IPopupMediator
    {
        event Action OnTrainingSettingsUpdated;
        void UpdateSelectedPrograms(List<TrainingProgramSettings> programSettings);
    }

    public class TrainingProgramsMediator : ITrainingProgramsMediator, IPopupView
    {
        public event Action ON_CLOSE;
        public event Action OnTrainingSettingsUpdated;

        private ITrainingProgramsScreenController _controller;
        private readonly IAddressableRefsHolder _refsHolder;
        private readonly IDataService _dataService;
        private readonly IUIManager _uiManager;

        [Inject] private IUIScreenController _uiScreenController;

        public TrainingProgramsMediator(            
            ITrainingProgramsScreenController controller,
            IAddressableRefsHolder refsHolder,
            IDataService dataService,
            IUIManager uIManager)
        {
            _dataService = dataService;
            _controller = controller;
            _refsHolder = refsHolder;
            _uiManager = uIManager;
        }

        public void CreatePopup(Action onComplete = null)
        {
            _uiManager.OpenView(this, viewBehaviour: UIBehaviour.StayWithNew, onShow: onComplete);
        }

        public void ClosePopup(Action callback = null)
        {
            _uiManager.CloseView(this, () =>
            {
                callback?.Invoke();
                ON_CLOSE?.Invoke();
            });
        }

        public async UniTask InitPopup(Camera camera, Transform parent, int orderLayer = 0)
        {
            var view = await _refsHolder.Popups.Main.InstantiateFromReference<ITrainingProgramsScreenView>(Popups.TrainingProgramsScreen, parent);
            var programSettings = _dataService.UserProfileController.ActiveProgramSettings;
            _controller.SetMediator(this);
            _controller.SetProgramSettings(programSettings);
            await _controller.Init(view);
        }

        public void UpdateSelectedPrograms(List<TrainingProgramSettings> programSettings)
        {
            _dataService.SaveTrainingSettings(programSettings);
            OnTrainingSettingsUpdated?.Invoke();
        }

        public void Show(Action onShow)
        {
            _controller.Show(onShow);
        }

        public void Hide(Action onHide)
        {
            _uiScreenController.SetActiveMainMenu(true);
            _controller.Hide(onHide);
        }

        public void Release()
        {
            _controller.Release();
        }
    }
}
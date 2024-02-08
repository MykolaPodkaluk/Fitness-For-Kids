using FitnessForKids.Data.Addressables;
using FitnessForKids.Services.UI;
using Cysharp.Threading.Tasks;
using FitnessForKids.Services;
using FitnessForKids.Data;
using UnityEngine;
using System;

namespace FitnessForKids.UI
{
    public interface IProfileRegistrationMediator : IPopupMediator
    {

    }

    public class ProfileRegistrationMediator : IProfileRegistrationMediator, IPopupView
    {
        public event Action ON_CLOSE;

        private readonly IAddressableRefsHolder _refsHolder;
        private readonly IUIManager _uiManager;
        private readonly IDataService _dataService;

        private INewProfileScreenView _view;
        private INewProfileScreenController _controller;

        public ProfileRegistrationMediator(
            IAddressableRefsHolder refsHolder, 
            IUIManager uIManager, 
            IDataService dataService,
            INewProfileScreenController controller)
        {
            _refsHolder = refsHolder;
            _uiManager = uIManager;
            _dataService = dataService;
            _controller = controller;
        }

        public void CreatePopup(Action onComplete = null)
        {
            _uiManager.OpenView(this, viewBehaviour: UIBehaviour.StayWithNew, onShow: onComplete);
        }

        public void ClosePopup(Action onComplete = null)
        {
            _uiManager.CloseView(this, onComplete);
        }

        public async UniTask InitPopup(Camera camera, Transform parent, int orderLayer = 0)
        {
            _view = await _refsHolder.Popups.Main.InstantiateFromReference<INewProfileScreenView>(Popups.Registration, parent);
            _controller.Init(_view, !_dataService.UserProfileController.HasActiveProfile);
            _controller.Model.OnRegistrationCompleted += CompleteRegistration;
        }

        public void Show(Action onShow)
        {
            _view.Show(() =>
            {
                _view.ON_CLOSE_CLICK += DoOnCloseClick;
                onShow?.Invoke();
            });
        }

        public void Hide(Action onHide)
        {
            _view.Hide(onHide);
        }

        public void Release()
        {
            _view.Release();
        }

        private void DoOnCloseClick()
        {
            _view.ON_CLOSE_CLICK -= DoOnCloseClick;

            // Here we doing something when we wanna close Registration Screen

            ClosePopup(() =>
            {
                ON_CLOSE?.Invoke();
            });
        }

        private void CompleteRegistration(UserProfileData newProfileData)
        {
            _dataService.UserProfileController.CreateProfile(newProfileData);
            DoOnCloseClick();
        }
    }
}
using FitnessForKids.Data.Addressables;
using FitnessForKids.Services.UI;
using Cysharp.Threading.Tasks;
using UnityEngine;
using System;

namespace FitnessForKids.UI.Subscription
{
    public interface ISubscriptionScreenMediator : IPopupMediator
    {

    }

    public class SubscriptionScreenMediator: ISubscriptionScreenMediator, IPopupView
    {
        public event Action ON_CLOSE;

        private IAddressableRefsHolder _refsHolder;
        private ISubscriptionScreenController _controller;
        private IUIManager _uiManager;

        public SubscriptionScreenMediator(
            IAddressableRefsHolder refsHolder, 
            ISubscriptionScreenController controller, 
            IUIManager uIManager)
        {
            _refsHolder = refsHolder;
            _controller = controller;
            _uiManager = uIManager;
        }

        public void CreatePopup(Action onComplete = null)
        {
            _uiManager.OpenView(this, UIBehaviour.StayWithNew, onComplete);
        }

        public void Show(Action onShow)
        {
            _controller.Show(onShow);
        }

        public void Hide(Action onHide)
        {
            _controller.Hide(onHide);
        }

        public void ClosePopup(Action callback = null)
        {
            _uiManager.CloseView(this, ()=>
            {
                callback?.Invoke();
                ON_CLOSE?.Invoke();
            });
        }

        public async UniTask InitPopup(Camera camera, Transform parent, int orderLayer = 0)
        {
            var view = await _refsHolder.Popups.Main.InstantiateFromReference<ISubscriptionScreenView>(Popups.Subscription, parent);
            view.Init();
            _controller.SetMediator(this);
            await _controller.Init(view);
        }

        public void Release()
        {
            _controller.Release();
        }
    }
}
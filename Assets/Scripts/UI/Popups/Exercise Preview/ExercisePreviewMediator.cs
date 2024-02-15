using FitnessForKids.Data.Addressables;
using FitnessForKids.Services.UI;
using Cysharp.Threading.Tasks;
using UnityEngine;
using System;

namespace FitnessForKids.UI
{
    public interface IExercisePreviewMediator : IPopupMediator
    {
        void CreatePopup(string exerciseId, Action onComplete = null);
    }

    public class ExercisePreviewMediator : IExercisePreviewMediator, IPopupView
    {
        public event Action ON_CLOSE;

        private IExercisePreviewPanelController _controller;
        private readonly IAddressableRefsHolder _refsHolder;
        private readonly IUIManager _uiManager;

        public ExercisePreviewMediator(
            IExercisePreviewPanelController controller,
            IAddressableRefsHolder refsHolder,
            IUIManager uIManager)
        {
            _controller = controller;
            _refsHolder = refsHolder;
            _uiManager = uIManager;
        }

        public void CreatePopup(string exerciseId, Action onComplete = null)
        {
            _controller.InitModel(exerciseId);
            _uiManager.OpenView(this, viewBehaviour: UIBehaviour.StayWithNew, onShow: onComplete);
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
            var view = await _refsHolder.Popups.Main.InstantiateFromReference<IExercisePreviewPanelView>(Popups.ExercisePreviewPanel, parent);
            _controller.SetMediator(this);
            await _controller.Init(view);
        }

        public void Show(Action onShow)
        {
            _controller.Show(onShow);
        }

        public void Hide(Action onHide)
        {
            _controller.Hide(onHide);
        }

        public void Release()
        {
            _controller.Release();
        }
    }
}
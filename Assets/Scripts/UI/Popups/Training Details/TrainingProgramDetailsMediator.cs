using FitnessForKids.Data.Addressables;
using FitnessForKids.Services.UI;
using Cysharp.Threading.Tasks;
using FitnessForKids.Data;
using UnityEngine;
using System;

namespace FitnessForKids.UI
{
    public interface ITrainingProgramDetailsMediator : IPopupMediator
    {
        void CreatePopup(ProgramType type, Skill[] skills, int difficulty, 
            Sprite baseIcone, Sprite icone,  Action onComplete = null);
    }

    public class TrainingProgramDetailsMediator : ITrainingProgramDetailsMediator, IPopupView
    {
        public event Action ON_CLOSE;

        private ITrainingProgramDetailsController _controller;
        private readonly IAddressableRefsHolder _refsHolder;
        private readonly IUIManager _uiManager;

        public TrainingProgramDetailsMediator(
            ITrainingProgramDetailsController controller,
            IAddressableRefsHolder refsHolder,
            IUIManager uIManager)
        {
            _controller = controller;
            _refsHolder = refsHolder;
            _uiManager = uIManager;
        }

        public void CreatePopup(ProgramType type, Skill[] skills, int difficulty, 
            Sprite baseIcone, Sprite icone, Action onComplete = null)
        {
            _controller.InitModel(type, skills, difficulty, baseIcone, icone);
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
            var view = await _refsHolder.Popups.Main.InstantiateFromReference<ITrainingProgramDetailsView>(Popups.TrainingProgramDetails, parent);
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
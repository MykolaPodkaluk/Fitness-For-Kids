using Cysharp.Threading.Tasks;
using System;

namespace FitnessForKids.UI
{
    public interface IExercisePreviewInputHandler
    {
        public void ClickOnClose();
    }

    public interface IExercisePreviewPanelController : IBaseMediatedController
    {
        void SetMediator(IExercisePreviewMediator mediator);
        void InitModel(string exerciseId);
    }

    public class ExercisePreviewPanelController : BaseMediatedController<IExercisePreviewPanelView, ExercisePreviewPanelModel>,
        IExercisePreviewPanelController, IExercisePreviewInputHandler
    {
        private IExercisePreviewMediator _mediator;
        private string _exerciseId;

        public void SetMediator(IExercisePreviewMediator mediator)
        {
            _mediator = mediator;
        }

        public void InitModel(string exerciseId)
        {
            _exerciseId = exerciseId;
        }

        public override void Show(Action onShow = null)
        {
            _view.Show(onShow);
        }

        public override void Hide(Action onHide)
        {
            _view.Hide(onHide);
        }

        public override void Release()
        {
            _view.Release();
        }

        public void ClickOnClose()
        {
            _mediator.ClosePopup();
        }

        protected override async UniTask<ExercisePreviewPanelModel> BuildModel()
        {
            var model = new ExercisePreviewPanelModel(_exerciseId);
            return await UniTask.FromResult(model);
        }

        protected override UniTask DoOnInit(IExercisePreviewPanelView view)
        {
            view.SetInputHandler(this);
            string name = _model.Name;
            string description = _model.Description;
            view.UpdateView(name, description);
            return UniTask.CompletedTask;
        }
    }
}
using Cysharp.Threading.Tasks;
using FitnessForKids.Data;
using UnityEngine;
using System;

namespace FitnessForKids.UI
{
    public interface ITrainingProgramsDetailsInputHandler
    {
        public void ClickOnClose();
    }

    public interface ITrainingProgramDetailsController : IBaseMediatedController
    {
        void SetMediator(ITrainingProgramDetailsMediator mediator);
        void InitModel(ProgramType type, Skill[] skills, int difficulty,
            Sprite baseIcone, Sprite icone);
    }

    public class TrainingProgramDetailsController : BaseMediatedController<ITrainingProgramDetailsView, TrainingProgramDetailsModel>,
        ITrainingProgramDetailsController, ITrainingProgramsDetailsInputHandler
    {
        private ITrainingProgramDetailsMediator _mediator;

        private ProgramType _type;
        private Skill[] _skills;
        private int _difficulty;
        private Sprite _baseIcone;
        private Sprite _icone;

        public void SetMediator(ITrainingProgramDetailsMediator mediator)
        {
            _mediator = mediator;
        }

        public void InitModel(ProgramType type, Skill[] skills, int difficulty,
            Sprite baseIcone, Sprite icone)
        {
            _type = type;
            _skills = skills;
            _difficulty = difficulty;
            _baseIcone = baseIcone;
            _icone = icone;
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

        protected override async UniTask<TrainingProgramDetailsModel> BuildModel()
        {
            var model = new TrainingProgramDetailsModel(_type, _skills, _difficulty, _baseIcone, _icone);
            return await UniTask.FromResult(model);
        }

        protected override UniTask DoOnInit(ITrainingProgramDetailsView view)
        {
            view.SetInputHandler(this);
            string title = _model.Title;   
            string description = _model.Description;
            string exercises = _model.IncludingExercises;
            string time = _model.Time;
            Sprite baseIcone = _model.BaseIcon;
            Sprite icone = _model.Icone;
            int difficulty = _model.Difficulty;
            Skill[] skills = _model.Skills;
            view.UpdateView(title, description, exercises, time, baseIcone, icone, difficulty, skills);
            return UniTask.CompletedTask;
        }
    }
}
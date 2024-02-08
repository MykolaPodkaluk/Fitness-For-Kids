using FitnessForKids.Data.Addressables;
using System.Collections.Generic;
using FitnessForKids.UI.Styles;
using Cysharp.Threading.Tasks;
using FitnessForKids.Services;
using FitnessForKids.Training;
using FitnessForKids.Data;
using UnityEngine;
using System.Linq;
using Zenject;
using System;

namespace FitnessForKids.UI
{
    public interface ITrainingProgramsScreenInputHandler
    {
        public void ClickOnSelectToggle(ProgramType type, bool isActive);
        public void ClickOnStartTraining(int id);
        public void ClickOnMoreDetails(int id);
        public void ClickOnClose();
        public void ClickOnReturn();
    }

    public interface ITrainingProgramsScreenController : IBaseMediatedController
    {
        void SetMediator(ITrainingProgramsMediator mediator);
        void SetProgramSettings(List<TrainingProgramSettings> programSettings);
    }

    public class TrainingProgramsScreenController : 
        BaseMediatedController<ITrainingProgramsScreenView, TrainingProgramsScreenModel>, 
        ITrainingProgramsScreenController,
        ITrainingProgramsScreenInputHandler
    {
        private readonly IAddressableRefsHolder _refsHolder;
        private readonly ITrainingService _trainingService;
        private ITrainingProgramsMediator _mediator;
        private ITrainingProgramDetailsMediator _programDetailsMediator;

        List<TrainingProgramSettings> _programSettings;

        public TrainingProgramsScreenController(DiContainer container)
        {
            _refsHolder = container.Resolve<IAddressableRefsHolder>();
            _trainingService = container.Resolve<ITrainingService>();
            _programDetailsMediator = container.Resolve<ITrainingProgramDetailsMediator>();
        }

        public void SetMediator(ITrainingProgramsMediator mediator)
        {
            _mediator = mediator;
        }

        public void SetProgramSettings(List<TrainingProgramSettings> programSettings)
        {
            _programSettings = programSettings;
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

        public void ClickOnSelectToggle(ProgramType type, bool isActive)
        {
            _model.UpdateProgramSettings(type, isActive);
            UpdateSelectedPrograms();
        }

        private void UpdateSelectedPrograms()
        {
            _mediator.UpdateSelectedPrograms(_model.ProgramSettings);
        }

        public void ClickOnStartTraining(int id)
        {
            _trainingService.OnTrainingStarted += OnStartTraining;
            _trainingService.StartTraining(_model.TrainingPrograms[id]);

            _view.DisallowClicksOnPrograms();
        }

        public void ClickOnMoreDetails(int id)
        {
            ShowProgramDescription(id);
            _view.DisallowClicksOnPrograms();
        }

        public void ClickOnClose()
        {
            _mediator.ClosePopup();
        }

        public void ClickOnReturn()
        {
            _mediator.ClosePopup();
        }

        protected override async UniTask<TrainingProgramsScreenModel> BuildModel()
        {
            var model = new TrainingProgramsScreenModel();
            var programs = _refsHolder.TrainingPrograms;
            var trainingProgramsCount = programs.Length;

            model.ProgramSettings = _programSettings;
            model.TrainingPrograms = programs;
            model.ProgramIcons = new Sprite[trainingProgramsCount];
            model.ProgramIconBases = new Sprite[trainingProgramsCount];
            model.ProgramBases = new Sprite[trainingProgramsCount];
            model.ProgramColors = new Color[trainingProgramsCount];
            model.ProgramTitles = new string[trainingProgramsCount];
            model.ProgramDescriptions = new string[trainingProgramsCount];
            model.ProgramFullDescriptions = new string[trainingProgramsCount];

            async UniTask CreateProgram(int i)
            {
                var type = programs[i].Type;
                var viewStyle = await _refsHolder.Trainings.Views.GetData<TrainingProgramPanelViewStyleData>(type);
                var colorStyle = await _refsHolder.Trainings.Styles.GetData<TrainingProgramColorStyleData>(viewStyle.ColorScheme);
                var localization = await _refsHolder.Trainings.LocalizationKeys.GetData<TrainingProgramLocalizationData>(type);

                model.ProgramIcons[i] = viewStyle.IconSprite;
                model.ProgramIconBases[i] = colorStyle.IconBackgroundSprite;
                model.ProgramBases[i] = colorStyle.PanelBaseSprite;
                model.ProgramColors[i] = colorStyle.Color;

                model.ProgramTitles[i] = LocalizationController.GetLocalizedString(model.TableKey, localization.Title);
                model.ProgramDescriptions[i] = LocalizationController.GetLocalizedString(model.TableKey, localization.Description);
                model.ProgramFullDescriptions[i] = LocalizationController.GetLocalizedString(model.TableKey, localization.FullDescription);
            }
            for (int i = 0, j = trainingProgramsCount; i < j; i++)
            {
                await CreateProgram(i);
            }
            return await UniTask.FromResult(model);
        }

        protected override UniTask DoOnInit(ITrainingProgramsScreenView view)
        {
            view.SetInputHandler(this);
            Debug.Log(Time.time);

            var trainingPrograms = _model.TrainingPrograms;
            var trainingProgramsCount = trainingPrograms.Length;
            for (int i = 0, j = trainingProgramsCount; i < j; i++)
            {
                var type = trainingPrograms[i].Type;
                var programSettings = _programSettings.FirstOrDefault(settings => settings.Type == type);
                var isActive = programSettings.IsActive;
                var id = i;
                var title = _model.ProgramTitles[id];
                var description = _model.ProgramDescriptions[id];
                var icon = _model.ProgramIcons[id];
                var iconBase = _model.ProgramIconBases[id];
                var panelBase = _model.ProgramBases[id];
                var color = _model.ProgramColors[id];
                view.AddTrainingProgramView(type, isActive, id, title, description, icon, iconBase, panelBase, color);
            }
            view.IndicatorsController.Init(trainingProgramsCount);
            view.ActivateScrollSnap();
            return UniTask.CompletedTask;
        }

        private void OnStartTraining()
        {
            _trainingService.OnTrainingStarted -= OnStartTraining;
            _view.AllowClicksOnPrograms();
            _mediator.ClosePopup();
        }

        public void ShowProgramDescription(int id)
        {
            var program = _model.TrainingPrograms[id];
            var type = program.Type;
            var skills = program.Skills;
            var difficulty = program.Difficulty;
            var iconBase = _model.ProgramIconBases[id];
            var icon = _model.ProgramIcons[id];
            _programDetailsMediator.CreatePopup(type, skills, difficulty, iconBase, icon);
        }
    }
}
using FitnessForKids.Services;
using FitnessForKids.Training;
using FitnessForKids.UI;
using UnityEngine;
using Zenject;

public class MainSceneContextInstaller : MonoInstaller
{
    #region FIELDS

    [Header("BASIC CONTROLLERS:")]
    [SerializeField] private TrainingController trainingController;
    [SerializeField] private AnimationController animationController;

    [Header("BASIC VIEWS:")]
    [SerializeField] private MainMenuScreenView mainMenuScreenView;
    [SerializeField] private WeeklyStatisticsPanelView weeklyStatisticsPanelView;
    [SerializeField] private TrainingScreenView trainingScreenView;

    #endregion

    public override void InstallBindings()
    {
        Container.Bind<IUIScreenController>().To<UIScreenController>().AsSingle().NonLazy();
        BindMainMenuScreen();
        BindTrainingService();
        BindTrainingProgramsScreen();
    }

    private void BindMainMenuScreen()
    {
        Container.Bind<IMainMenuScreenView>().FromInstance(mainMenuScreenView).AsSingle();
        Container.Bind<IMainMenuScreenController>().To<MainMenuScreenController>().AsSingle();
        Container.Bind<IWeeklyStatisticsPanelView>().FromInstance(weeklyStatisticsPanelView).AsSingle();
        Container.Bind<IWeeklyStatisticsPanelController>().To<WeeklyStatisticsPanelController>().AsSingle();
    }

    private void BindTrainingService()
    {
        Container.Bind<ITrainingService>().To<TrainingService>().AsSingle();
        Container.Bind<ExerciceDataProvider>().To<ExerciceDataProvider>().AsSingle();
        Container.Bind<ITrainingController>().FromInstance(trainingController).AsSingle();
        Container.Bind<ITrainingScreenView>().FromInstance(trainingScreenView).AsSingle();
        Container.Bind<IAnimationController>().FromInstance(animationController).AsSingle();
        Container.Bind<IExercisePreviewMediator>().To<ExercisePreviewMediator>().AsTransient();
        Container.Bind<IExercisePreviewPanelController>().To<ExercisePreviewPanelController>().AsTransient();

        trainingController.Init(trainingScreenView, animationController);
    }

    private void BindTrainingProgramsScreen()
    {
        Container.Bind<ITrainingProgramsMediator>().To<TrainingProgramsMediator>().AsTransient();
        Container.Bind<ITrainingProgramsScreenController>().To<TrainingProgramsScreenController>().AsTransient();
        Container.Bind<ITrainingProgramDetailsMediator>().To<TrainingProgramDetailsMediator>().AsTransient();
        Container.Bind<ITrainingProgramDetailsController>().To<TrainingProgramDetailsController>().AsTransient();
    }
}
using FitnessForKids.Services;
using FitnessForKids.Training;
using FitnessForKids.UI;
using UnityEngine;
using Zenject;

public class TrainingSceneContextInstaller : MonoInstaller
{
    //[SerializeField] private AnimationController animationController;
    //[SerializeField] private TrainingController trainingController;
    //[SerializeField] private TrainingScreenView trainingScreenView;

    //public override void InstallBindings()
    //{
    //    //BindTrainingService();
    //}

    //private void BindTrainingService()
    //{
    //    Container.Bind<ITrainingService>().To<TrainingService>().AsSingle();
    //    Container.Bind<ExerciceDataProvider>().To<ExerciceDataProvider>().AsSingle();
    //    Container.Bind<ITrainingScreenView>().FromInstance(trainingScreenView).AsSingle().NonLazy();
    //    Container.Bind<ITrainingController>().FromInstance(trainingController).AsSingle().NonLazy();
    //    Container.Bind<IAnimationController>().FromInstance(animationController).AsSingle().NonLazy();
    //}
}

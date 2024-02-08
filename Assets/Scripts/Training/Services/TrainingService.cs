using FitnessForKids.Data.Addressables;
using FitnessForKids.Data;
using UnityEngine;
using System;

namespace FitnessForKids.Services
{
    public interface ITrainingService
    {
        void StartSelectedTraining();
        void SelectTrainingProgram(ProgramType type);
        void StartTraining(ITrainingParameters trainingParameters);
        void StopTraining();
        event Action OnTrainingStarted;
        event Action OnTrainingStoped;
    }

    public class TrainingService : ITrainingService
    {
        private readonly IAddressableRefsHolder _refsHolder;
        private ITrainingController _trainingController;
        public event Action OnTrainingStarted;
        public event Action OnTrainingStoped;

        private ITrainingParameters _selectedTrainingProgram;

        public TrainingService(ITrainingController trainingController,
            IAddressableRefsHolder refsHolder)
        {
            _refsHolder = refsHolder;
            _trainingController = trainingController;
            trainingController.OnProgramCompleted += StopTraining;
        }

        public void SelectTrainingProgram(ProgramType type)
        {
            _selectedTrainingProgram = _refsHolder.GetTrainingProgramByType(type);
        }

        public void StartSelectedTraining()
        {
            StartTraining();
        }

        public void StartTraining(ITrainingParameters trainingParameters)
        {
            _selectedTrainingProgram = trainingParameters;
            StartTraining();
        }

        private void StartTraining()
        {
            _trainingController.StartTraining(_selectedTrainingProgram);
            OnTrainingStarted?.Invoke();
        }

        public void StopTraining()
        {
            OnTrainingStoped?.Invoke();
        }
    }
}
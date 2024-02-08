using FitnessForKids.Data;
using System;
using System.Collections.Generic;

namespace FitnessForKids.Services
{
    public interface IDataService
    {
        event Action ON_RESET;
        IUserProfileController UserProfileController { get; }
        //IStatisticsController StatisticsController { get; }
        //ISettingsController SettingsController { get; }
        public bool IsInitiated { get; }
        void ResetProgress();
        void SaveProgramData(TrainingProgram completedProgram);
        void SaveTrainingSettings(List<TrainingProgramSettings> programSettings);
    }
}
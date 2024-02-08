using System.Collections.Generic;
using System;

namespace FitnessForKids.Data
{
    public interface ITrainingSettingsData
    {
        List<TrainingProgramSettings> ProgramSettings { get; set; }
    }

    [Serializable]
    public class TrainingSettingsData : ITrainingSettingsData
    {
        public List<TrainingProgramSettings> ProgramSettings { get; set; } = new List<TrainingProgramSettings>();

        public TrainingSettingsData()
        {
            ProgramSettings = new List<TrainingProgramSettings>();
        }

        public TrainingSettingsData(bool isDefault)
        {
            ProgramSettings = DefaultProgramSettings();
        }

        private List<TrainingProgramSettings> DefaultProgramSettings()
        {
            var settings = new List<TrainingProgramSettings>();
            foreach (ProgramType programType in Enum.GetValues(typeof(ProgramType)))
            {
                if (programType != ProgramType.Custom)
                {
                    var programSettings = new TrainingProgramSettings(programType, true);
                    settings.Add(programSettings);
                }
            }
            return settings;
        }
    }

    [Serializable]
    public class TrainingProgramSettings
    {
        public ProgramType Type { get; set; }
        public bool IsActive { get; set; }

        public TrainingProgramSettings(ProgramType type, bool isActive)
        {
            Type = type;
            IsActive = isActive;
        }
    }
}
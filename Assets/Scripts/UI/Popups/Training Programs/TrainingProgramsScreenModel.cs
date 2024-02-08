using System.Collections.Generic;
using FitnessForKids.Data;
using System.Linq;
using UnityEngine;

namespace FitnessForKids.UI
{
    public class TrainingProgramsScreenModel : IModel
    {
        private List<TrainingProgramSettings> _programSettings = new List<TrainingProgramSettings>();
        public List<TrainingProgramSettings> ProgramSettings
        {
            get => _programSettings;
            set
            {
                _programSettings = value;
            }
        }
        private TrainingProgramParameters[] _trainingPrograms;
        public TrainingProgramParameters[] TrainingPrograms
        {
            get => _trainingPrograms;
            set
            {
                _trainingPrograms = value;
            }
        }
        public string[] ProgramTitles;
        public string[] ProgramDescriptions;
        public string[] ProgramFullDescriptions;
        public Sprite[] ProgramIcons;
        public Sprite[] ProgramIconBases;
        public Sprite[] ProgramBases;
        public Color[] ProgramColors;
        public const string kTableKey = "Sports Programs";
        public string TableKey => kTableKey;

        public void UpdateProgramSettings(ProgramType type, bool isActive)
        {
            _programSettings.FirstOrDefault(settings => settings.Type == type).IsActive = isActive;
        }
    }
}
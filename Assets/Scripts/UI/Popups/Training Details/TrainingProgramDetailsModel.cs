using FitnessForKids.Data;
using UnityEngine;

namespace FitnessForKids.UI
{
    public class TrainingProgramDetailsModel : IModel
    {
        private ProgramType _programType;
        private Skill[] _skills;
        private int _difficulty;
        private Sprite _baseIcone;
        private Sprite _icone;
        public string Title => GetTitle();
        public string Time => GetTime();
        public string IncludingExercises => GetExercises();
        public string Description => GetDescription();
        public Sprite BaseIcon => _baseIcone;
        public Sprite Icone => _icone;
        public int Difficulty => _difficulty;
        public Skill[] Skills => _skills;

        public const string kTableKey = "Sports Programs";
        private readonly string[] _typesKeys = { "Warm-up", "Morning exercises", "No equipment", "Healthy back", "Full body", "Fitness strong" };
        private string _typeKey { get => _typesKeys[((int)_programType) - 1]; }

        public TrainingProgramDetailsModel(ProgramType type, Skill[] skills, int difficulty, 
            Sprite baseIcone, Sprite icone)
        {
            _programType = type;
            _skills = skills;
            _difficulty = difficulty;
            _baseIcone = baseIcone;
            _icone = icone;
        }

        private string GetTitle()
        {
            string title = LocalizationController.GetLocalizedString(kTableKey, _typeKey);
            return title;
        }

        private string GetTime()
        {
            var timeKey = $"{_typeKey} time";
            string time = LocalizationController.GetLocalizedString(kTableKey, timeKey);
            return time;
        }

        private string GetExercises()
        {
            var exerciseKey = $"{_typeKey} including exercises";
            string exercises = LocalizationController.GetLocalizedString(kTableKey, exerciseKey);
            return exercises;
        }

        private string GetDescription()
        {
            var descriptionKey = $"{_typeKey} full description";
            string fullDescription = LocalizationController.GetLocalizedString(kTableKey, descriptionKey);
            return fullDescription;
        }
    }
}
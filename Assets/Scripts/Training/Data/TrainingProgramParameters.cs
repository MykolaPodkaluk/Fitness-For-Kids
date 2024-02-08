using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;

namespace FitnessForKids.Data
{
    [CreateAssetMenu(fileName = "New ProgramParameters", menuName = "ScriptableObjects/New ProgramParameters")]
    public class TrainingProgramParameters : ScriptableObject, ITrainingParameters
    {
        public ProgramType type;
        public ProgramType Type => type;
        public List<ExerciseParametersData> exerciseParameters;
        public List<ExerciseParametersData> ExerciseParameters => exerciseParameters;
        public List<BodyPart> BodyParts => GetBodyParts();
        [SerializeField] private Skill[] _skills;
        [SerializeField] private int _difficulty;
        public Skill[] Skills => _skills;
        public int Difficulty => _difficulty;
        private List<BodyPart> GetBodyParts()
        {
            return exerciseParameters.Select(exercise => exercise.BodyParts.FirstOrDefault()).ToList();
        }
    }

    [Serializable]
    public class ExerciseParametersData
    {
        public List<string> IDs;
        public List<BodyPart> BodyParts;
        public List<Difficulty> DifficultyLevels;
        public int MinReps = 10;
        public int MaxReps = 15;
    }
}
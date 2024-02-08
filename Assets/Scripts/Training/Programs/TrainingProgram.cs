using FitnessForKids.Training;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FitnessForKids.Data
{
    [Serializable]
    public class TrainingProgram : ITrainingProgram
    {
        private ProgramType type;
        private List<ExerciseData> exercises;
        public ProgramType Type => type;
        public List<ExerciseData> Exercises => exercises;
        public List<ExerciseSaveData> ExerciseSaveDatas => GetAllExerciseSaveDatas();

        public TrainingProgram(ProgramType type, List<ExerciseData> exercises)
        {
            this.type = type;
            this.exercises = exercises;
        }

        public List<Skill> GetAllSkills()
        {
            List<Skill> allSkills = exercises
                .SelectMany(exercise => exercise.ActiveSkills)
                .Where(skill => skill != Skill.None)
                .ToList();

            return allSkills;
        }

        private List<ExerciseSaveData> GetAllExerciseSaveDatas()
        {
            var exerciseSaveDatas = new List<ExerciseSaveData>();
            foreach (var exerciseData in exercises)
            {
                var exerciseSaveData = new ExerciseSaveData(
                    exerciseData.ID,
                    exerciseData.BodyPart,
                    exerciseData.WorkingParts,
                    exerciseData.Difficulty,
                    exerciseData.Skills);
                exerciseSaveDatas.Add(exerciseSaveData);
            }
            return exerciseSaveDatas;
        }
    }

    [Serializable]
    public class TrainingProgramStatistics
    {
        private ProgramType type;
        private List<ExerciseSaveData> exercises;
        public ProgramType Type => type;
        public List<ExerciseSaveData> Exercises => exercises;

        public TrainingProgramStatistics(ProgramType type, List<ExerciseSaveData> exercises)
        {
            this.type = type;
            this.exercises = exercises;
        }
    }
}
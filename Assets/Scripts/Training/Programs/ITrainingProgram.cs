using System.Collections.Generic;

namespace FitnessForKids.Data
{
    public interface ITrainingProgram
    {
        public ProgramType Type { get; }
        public List<ExerciseData> Exercises {get;}
        public List<Skill> GetAllSkills();
    }

    public enum ProgramType
    {
        Custom = 0,
        WarmUp = 1,
        MorningExercise = 2,
        NoEquipment = 3,
        HealthyBack = 4,
        FullBody = 5,
        FitnessStrong = 6,
    }
}
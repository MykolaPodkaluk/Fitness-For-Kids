using System.Collections.Generic;

namespace FitnessForKids.Data
{
    public interface ITrainingParameters
    {
        ProgramType Type { get; }
        List<ExerciseParametersData> ExerciseParameters { get; }
        List<BodyPart> BodyParts { get; }
    }
}
using Cysharp.Threading.Tasks;
using FitnessForKids.Training;
using System;

namespace FitnessForKids.Data.Addressables
{
    public interface ITrainingProgramLocalizationRefsProvider : ILocalizationKeysRefsProvider
    {
        UniTask<TKeys> GetData<TKeys>(ProgramType type) where TKeys : ITrainingProgramLocalizationData;
    }

    [Serializable]
    public class TrainingProgramLocalizationRefsProvider : TrainingProgramTextRefsProvider<ProgramType>, ITrainingProgramLocalizationRefsProvider
    {

    }
}
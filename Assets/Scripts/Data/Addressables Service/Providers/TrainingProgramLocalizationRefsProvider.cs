using Cysharp.Threading.Tasks;
using FitnessForKids.Training;
using System;
using System.Collections.Generic;

namespace FitnessForKids.Data.Addressables
{
    public interface ITrainingProgramLocalizationRefsProvider : ILocalizationKeysRefsProvider
    {
        UniTask<TKeys> GetData<TKeys>(ProgramType type) where TKeys : ITrainingProgramLocalizationData;
        UniTask<List<TKeys>> GetAllData<TKeys>() where TKeys : ITrainingProgramLocalizationData;
    }

    [Serializable]
    public class TrainingProgramLocalizationRefsProvider : TrainingProgramTextRefsProvider<ProgramType>, ITrainingProgramLocalizationRefsProvider
    {

    }
}
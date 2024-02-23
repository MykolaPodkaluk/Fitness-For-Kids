using FitnessForKids.UI.Styles;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;

namespace FitnessForKids.Data.Addressables
{
    public interface ITrainingProgramViewRefsProvider : IViewStyleRefsProvider
    {
        UniTask<TView> GetData<TView>(ProgramType type) where TView : ITrainingProgramViewStyleData;
        UniTask<List<TView>> GetAllData<TView>() where TView : ITrainingProgramViewStyleData;
    }

    [Serializable]
    public class TrainingProgramViewRefsProvider : TrainingProgramViewStyleRefsProvider<ProgramType>, ITrainingProgramViewRefsProvider
    {
        
    }
}
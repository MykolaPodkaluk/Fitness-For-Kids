using FitnessForKids.UI.Styles;
using Cysharp.Threading.Tasks;
using System;

namespace FitnessForKids.Data.Addressables
{
    public interface ITrainingProgramViewRefsProvider : IViewStyleRefsProvider
    {
        UniTask<TView> GetData<TView>(ProgramType type) where TView : ITrainingProgramViewStyleData;
    }

    [Serializable]
    public class TrainingProgramViewRefsProvider : TrainingProgramViewStyleRefsProvider<ProgramType>, ITrainingProgramViewRefsProvider
    {
        
    }
}
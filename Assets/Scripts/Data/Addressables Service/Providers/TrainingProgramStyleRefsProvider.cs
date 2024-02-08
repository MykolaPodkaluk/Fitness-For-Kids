using FitnessForKids.UI.Styles;
using Cysharp.Threading.Tasks;
using System;

namespace FitnessForKids.Data.Addressables
{
    public interface ITrainingProgramStyleRefsProvider : IViewStyleRefsProvider
    {
        UniTask<TStyle> GetData<TStyle>(ColorScheme type) where TStyle : ITrainingProgramColorStyleData;
    }

    [Serializable]
    public class TrainingProgramStyleRefsProvider : TrainingProgramColorStyleRefsProvider<ColorScheme>, ITrainingProgramStyleRefsProvider
    {
        
    }
}
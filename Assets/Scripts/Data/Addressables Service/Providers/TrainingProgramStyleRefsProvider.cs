using FitnessForKids.UI.Styles;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;

namespace FitnessForKids.Data.Addressables
{
    public interface ITrainingProgramStyleRefsProvider : IViewStyleRefsProvider
    {
        UniTask<TStyle> GetData<TStyle>(ColorScheme type) where TStyle : ITrainingProgramColorStyleData;
        UniTask<List<TSyle>> GetAllData<TSyle>() where TSyle : ITrainingProgramColorStyleData;
    }

    [Serializable]
    public class TrainingProgramStyleRefsProvider : TrainingProgramColorStyleRefsProvider<ColorScheme>, ITrainingProgramStyleRefsProvider
    {
        
    }
}
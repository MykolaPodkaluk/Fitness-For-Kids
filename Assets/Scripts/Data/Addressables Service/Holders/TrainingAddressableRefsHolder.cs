using UnityEngine;

namespace FitnessForKids.Data.Addressables
{
    public interface ITrainingAddressableRefsHolder
    {
        ITrainingProgramStyleRefsProvider Styles { get; }
        ITrainingProgramViewRefsProvider Views { get; }
        ITrainingProgramLocalizationRefsProvider LocalizationKeys { get; }
    }

    [CreateAssetMenu(fileName = "TrainingAddressableRefsHolder", menuName = "ScriptableObjects/AddressableRefsHolders/Trainings")]
    public class TrainingAddressableRefsHolder : ScriptableObject, ITrainingAddressableRefsHolder
    {
        [SerializeField] private TrainingProgramStyleRefsProvider _styles;
        [SerializeField] private TrainingProgramViewRefsProvider _views;
        [SerializeField] private TrainingProgramLocalizationRefsProvider _localizationKeys;
        public ITrainingProgramStyleRefsProvider Styles => _styles;
        public ITrainingProgramViewRefsProvider Views => _views;
        public ITrainingProgramLocalizationRefsProvider LocalizationKeys => _localizationKeys;
    }
}
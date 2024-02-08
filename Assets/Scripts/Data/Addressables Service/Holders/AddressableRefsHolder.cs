using FitnessForKids.Data.Providers;
using FitnessForKids.Training;
using System.Linq;
using UnityEngine;

namespace FitnessForKids.Data.Addressables
{
    public interface IAddressableRefsHolder
    {
        ExerciceDataProvider ExerciceDataProvider { get; }
        IPopupAddressableRefsHolder Popups { get; }
        ITrainingAddressableRefsHolder Trainings { get; }
        IExerciseAssetsHolder Exercises { get; }
        ITrainingAvatarsRefsHolder TrainingsAvatars { get; }
        TrainingProgramParameters[] TrainingPrograms { get; }
        TrainingProgramParameters GetTrainingProgramByType(ProgramType type);
    }


    [CreateAssetMenu(fileName = "AddressableRefsHolder", menuName = "ScriptableObjects/AddressableRefsHolders/General")]
    public class AddressableRefsHolder : ScriptableObject, IAddressableRefsHolder
    {
        [SerializeField] private PopupAddressableRefsHolder _popupsHolder;
        [SerializeField] private TrainingAddressableRefsHolder _trainingsHolder;
        [SerializeField] private ExerciseAssetsHolder _exercicesHolder;
        [SerializeField] private TrainingAvatarsRefsHolder _avatarsHolder;

        [SerializeField] private TrainingProgramParameters[] _allTrainingPrograms;

        private ExerciceDataProvider exerciceDataProvider;
        public IPopupAddressableRefsHolder Popups => _popupsHolder;
        public ITrainingAddressableRefsHolder Trainings => _trainingsHolder;
        public IExerciseAssetsHolder Exercises => _exercicesHolder;
        public ITrainingAvatarsRefsHolder TrainingsAvatars => _avatarsHolder;
        public TrainingProgramParameters[] TrainingPrograms => _allTrainingPrograms;
        public ExerciceDataProvider ExerciceDataProvider => exerciceDataProvider;

        public TrainingProgramParameters GetTrainingProgramByType(ProgramType type)
        {
            var program = _allTrainingPrograms.FirstOrDefault(p => p.Type == type);
            return program;
        }
    }
}
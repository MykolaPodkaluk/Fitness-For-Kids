using UnityEngine;

namespace FitnessForKids.Training
{
    public interface ITrainingProgramLocalizationData
    {

    }

    [CreateAssetMenu(fileName = "TrainingProgramLocalizationData", menuName = "ScriptableObjects/TrainingProgramLocalizationData")]
    public class TrainingProgramLocalizationData : ScriptableObject, ITrainingProgramLocalizationData
    {
        [field: SerializeField] public string Title { get; private set; }
        [field: SerializeField] public string Description { get; private set; }
        [field: SerializeField] public string FullDescription { get; private set; }
    }
}
using UnityEngine;

namespace FitnessForKids.UI.Styles
{
    public interface ITrainingProgramViewStyleData
    {

    }

    [CreateAssetMenu(fileName = "Basic TrainingProgram View StyleData", menuName = "ScriptableObjects/Styles/Basic TrainingProgram View StyleData")]
    public class BasicTrainingProgramViewStyleData : ScriptableObject, ITrainingProgramViewStyleData
    {
        [field: SerializeField] public Sprite IconSprite { get; private set; }
    }
}
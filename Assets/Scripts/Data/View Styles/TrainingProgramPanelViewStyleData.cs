using UnityEngine;

namespace FitnessForKids.UI.Styles
{
    [CreateAssetMenu(fileName = "TrainingProgram Panel View StyleData", menuName = "ScriptableObjects/Styles/TrainingProgram Panel View StyleData")]
    public class TrainingProgramPanelViewStyleData : BasicTrainingProgramViewStyleData
    {
        [field: SerializeField] public ColorScheme ColorScheme { get; private set; }
    }
}
using UnityEngine.AddressableAssets;
using UnityEngine;

namespace FitnessForKids.UI.Styles
{
    public interface ITrainingProgramColorStyleData
    {

    }

    [CreateAssetMenu(fileName = "TrainingProgram ColorStyleData", menuName = "ScriptableObjects/Styles/TrainingProgram ColorStyleData")]
    public class TrainingProgramColorStyleData : ScriptableObject, ITrainingProgramColorStyleData
    {
        [field: SerializeField] public Color Color { get; private set; }
        [field: SerializeField] public Sprite IconBackgroundSprite { get; private set; }
        [field: SerializeField] public Sprite PanelBaseSprite { get; private set; }
    }
}
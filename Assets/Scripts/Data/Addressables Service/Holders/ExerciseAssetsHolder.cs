using UnityEngine;

namespace FitnessForKids.Data.Providers
{
    public interface IExerciseAssetsHolder
    {
        ExerciseAnimationsProvider AnimationsProvider { get; }
    }

    [CreateAssetMenu(fileName = "ExerciseAssetsHolder", menuName = "ScriptableObjects/Holders/Exercises")]
    public class ExerciseAssetsHolder : ScriptableObject, IExerciseAssetsHolder
    {
        [SerializeField] public ExerciseAnimationsProvider _exerciseAnimationsProvider;
        public ExerciseAnimationsProvider AnimationsProvider => _exerciseAnimationsProvider;
    }
}
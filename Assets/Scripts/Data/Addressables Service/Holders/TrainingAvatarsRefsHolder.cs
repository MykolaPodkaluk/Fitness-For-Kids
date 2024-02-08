using UnityEngine;

namespace FitnessForKids.Data.Addressables
{
    public interface ITrainingAvatarsRefsHolder
    {
        ITrainingAvatarsRefProvider Main { get; }
    }

    [CreateAssetMenu(fileName = "AvatarRefsHolder", menuName = "ScriptableObjects/AddressableRefsHolders/Avatars")]
    public class TrainingAvatarsRefsHolder : ScriptableObject, ITrainingAvatarsRefsHolder
    {
        [SerializeField] private TrainingAvatarsRef _avatarsProvider;
        public ITrainingAvatarsRefProvider Main => _avatarsProvider;
    }
}


using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace FitnessForKids.Data.Addressables
{
    public interface ITrainingAvatarsRefProvider : IAddressableRefsProvider
    {
        UniTask<T> InstantiateFromReference<T>(Gender gender, Transform parent); 
    }

    [Serializable]
    public class TrainingAvatarsRef : AddressableRefsProvider<Gender, AssetReferenceGameObject>,
            ITrainingAvatarsRefProvider
    {

    }
}

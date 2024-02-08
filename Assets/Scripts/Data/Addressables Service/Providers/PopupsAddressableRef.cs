using UnityEngine.AddressableAssets;
using Cysharp.Threading.Tasks;
using UnityEngine;
using System;

namespace FitnessForKids.Data.Addressables
{
    public interface IPopupsAddressableRefProvider : IAddressableRefsProvider
    {
        UniTask<T> InstantiateFromReference<T>(Popups type, Transform parent);
    }

    [Serializable]
    public class PopupsAddressableRef : AddressableRefsProvider<Popups, AssetReferenceGameObject>, 
        IPopupsAddressableRefProvider
    { 

    }
}
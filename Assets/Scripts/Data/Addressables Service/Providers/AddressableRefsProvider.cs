using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.AddressableAssets;
using System.Collections.Concurrent;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;
using System;

namespace FitnessForKids.Data.Addressables
{
    /// <summary>
    /// Interface for clearing the cache of addressable asset references.
    /// </summary>
    public interface IAddressableRefsProvider
    {
        /// <summary>
        /// Clears the cache of addressable asset references.
        /// </summary>
        void ClearCache();
    }

    public abstract class AddressableRefsProvider<TType, TRef> : IAddressableRefsProvider where TType : Enum where TRef : AssetReference
    {
        [SerializeField] protected RefPair[] references;
        protected DiContainer container;
        private ConcurrentDictionary<string, AsyncOperationHandle<GameObject>> cache = new();

        /// <summary>
        /// Instantiate a game object from an addressable asset reference asynchronously.
        /// </summary>
        /// <typeparam name="T">The type of the component attached to the instantiated game object.</typeparam>
        /// <param name="type">The type of the addressable asset reference.</param>
        /// <param name="parent">The parent transform of the instantiated game object.</param>
        /// <returns>The instantiated game object with the specified component.</returns>
        public async UniTask<T> InstantiateFromReference<T>(TType type, Transform parent)
        {
            TRef reference = GetReference(type);
            string key = reference.RuntimeKey.ToString();
            GameObject viewPrefab = null;

            if (cache.TryGetValue(key, out AsyncOperationHandle<GameObject> handle))
            {
                viewPrefab = handle.Result;
                cache.TryUpdate(key, handle, handle);
            }
            else
            {
                try
                {
                    AsyncOperationHandle<GameObject> handler = UnityEngine.AddressableAssets.Addressables.LoadAssetAsync<GameObject>(reference.RuntimeKey);
                    await handler;
                    int attempts = 0;

                    while (handler.Status != AsyncOperationStatus.Succeeded && attempts < 5)
                    {
                        handler = UnityEngine.AddressableAssets.Addressables.LoadAssetAsync<GameObject>(reference.RuntimeKey);
                        await handler;
                        attempts++;
                        Debug.LogFormat("Invoked attempt № {0} for {1}", attempts, type);
                    }

                    cache.TryAdd(key, handler);
                    viewPrefab = handler.Result;
                    //UnityEngine.Debug.LogFormat("Cache contains {0} references", cache.Count);
                }
                catch (Exception e)
                {
                    throw new ArgumentNullException(
                        string.Format("Can't instantiate game object by addressable reference for >>{0}<<" + e, type)
                    );
                }
            }

            if (container == null)
            {
                container = ProjectContext.Instance.Container;
            }

            var viewGO = container.InstantiatePrefab(viewPrefab, parent);
            var view = viewGO.GetComponent<T>();
            return view;
        }

        /// <summary>
        /// Clears the cache of addressable asset references.
        /// </summary>
        public void ClearCache()
        {
            foreach (var reference in cache.Values)
            {
                UnityEngine.AddressableAssets.Addressables.Release(reference);
            }
            cache.Clear();
        }

        /// <summary>
        /// Loads an asset asynchronously based on its addressable reference.
        /// </summary>
        /// <typeparam name="T">The type of the asset.</typeparam>
        /// <param name="type">The type of the addressable asset reference.</param>
        /// <returns>The loaded asset.</returns>
        protected async UniTask<T> LoadAsync<T>(TType type)
        {
            TRef reference = GetReference(type);

            try
            {
                AsyncOperationHandle<T> handler = UnityEngine.AddressableAssets.Addressables.LoadAssetAsync<T>(reference.RuntimeKey);
                await handler;
                int attempts = 0;

                while (handler.Status != AsyncOperationStatus.Succeeded && attempts < 5)
                {
                    handler = UnityEngine.AddressableAssets.Addressables.LoadAssetAsync<T>(reference.RuntimeKey);
                    await handler;
                    attempts++;
                    Debug.LogFormat("Invoked attempt № {0} for {1}", attempts, type);
                }

                return handler.Result;
            }
            catch (Exception)
            {
                throw new ArgumentNullException(
                    string.Format("Can't Load async by addressable reference for >>{0}<<", type)
                );
            }
        }

        /// <summary>
        /// Loads an asset asynchronously based on a given addressable reference.
        /// </summary>
        /// <typeparam name="T">The type of the asset.</typeparam>
        /// <param name="reference">The addressable asset reference.</param>
        /// <returns>The loaded asset.</returns>
        protected async UniTask<T> LoadByRefAsync<T>(TRef reference)
        {
            try
            {
                AsyncOperationHandle<T> handler = UnityEngine.AddressableAssets.Addressables.LoadAssetAsync<T>(reference.RuntimeKey);
                await handler;
                int attempts = 0;

                while (handler.Status != AsyncOperationStatus.Succeeded && attempts < 5)
                {
                    handler = UnityEngine.AddressableAssets.Addressables.LoadAssetAsync<T>(reference.RuntimeKey);
                    await handler;
                    attempts++;
                    Debug.LogFormat("Invoked attempt № {0} for {1}", attempts, typeof(T));
                }

                return handler.Result;
            }
            catch (Exception)
            {
                throw new ArgumentNullException(
                    string.Format("Can't LoadByRefAsync")
                );
            }
        }

        private TRef GetReference(TType type)
        {
            for (int i = 0, j = references.Length; i < j; i++)
            {
                if (references[i].Type.Equals(type))
                {
                    return references[i].Reference;
                }
            }
            throw new ArgumentException(
                string.Format("There is no addressable reference found for task >>{0}<<", type)
            );
        }

        /// <summary>
        /// Nested class - mapper for Type and Reference
        /// </summary>
        [Serializable]
        public class RefPair
        {
            [field: SerializeField] public TType Type { get; private set; }
            [field: SerializeField] public TRef Reference { get; private set; }
        }
    }
}
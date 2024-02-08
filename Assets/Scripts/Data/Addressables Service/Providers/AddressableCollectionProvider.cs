using UnityEngine;
using System;

namespace FitnessForKids.Data.Providers
{
    public abstract class AssetCollectionProvider <T> where T : UnityEngine.Object
    {
        [SerializeField] protected T[] assets;
        [SerializeField] protected T[] placeholders;

        public T TryGetAssetByKey(string key)
        {
            T asset;
            if (TryGetAssetByKey(key, out asset))
            {
                return asset;
            }
            else
            {
                Debug.LogWarningFormat("Assets doesn't contain Asset with key: {0}. Using a placeholder.", key);
                return GetRandomPlaceholder();
            }
        }

        public T[] GetAllAssets()
        {
            return assets;
        }

        private T GetRandomPlaceholder()
        {
            var length = placeholders.Length;

            if (placeholders != null && length > 0)
            {
                int randomIndex = UnityEngine.Random.Range(0, length);
                Debug.LogWarningFormat("Using a random placeholder for Asset");
                return placeholders[randomIndex];
            }
            else
            {
                Debug.LogErrorFormat("No asset placeholders available");
                return default; // Default value for type T
            }
        }

        private bool TryGetAssetByKey(string key, out T asset)
        {
            for (int i = 0, j = assets.Length; i < j; i++)
            {
                if (assets[i].name.Equals(key))
                {
                    asset = assets[i];
                    return true;
                }
            }
            asset = null;
            return false;
            throw new ArgumentException(
                string.Format("There is no AnimationClip found for >>{0}<<", key)
            );
        }
    }
}
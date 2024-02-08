using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.AddressableAssets;
using Cysharp.Threading.Tasks;
using FitnessForKids.UI.Styles;
using System;

namespace FitnessForKids.Data.Addressables
{
    public interface IViewStyleRefsProvider
    {
    }

    public abstract class TrainingProgramColorStyleRefsProvider<TType> : AddressableRefsProvider<TType, AssetReference> where TType : Enum
    {
        public async UniTask<TStyle> GetRandomData<TStyle>() where TStyle : ITrainingProgramColorStyleData
        {
            var random = new System.Random();
            var values = Enum.GetValues(typeof(ColorScheme));
            var style = (ColorScheme)values.GetValue(random.Next(values.Length));
            return await GetData<TStyle>(style);
        }

        public async UniTask<TSyle> GetData<TSyle>(ColorScheme color) where TSyle : ITrainingProgramColorStyleData
        {
            var referenceData = GetReference(color);

            try
            {
                AsyncOperationHandle<TSyle> handler = UnityEngine.AddressableAssets.Addressables.LoadAssetAsync<TSyle>(referenceData.Reference.RuntimeKey);
                await handler;
                return handler.Result;
            }
            catch (Exception)
            {
                throw new ArgumentNullException(
                    string.Format("Can't load ScriptableObject from addressable reference for >>{0}<<", color)
                    );
            }
        }

        private RefPair GetReference(ColorScheme color)
        {
            for (int i = 0, j = references.Length; i < j; i++)
            {
                if (references[i].Type.Equals(color))
                {
                    return references[i];
                }
            }
            throw new ArgumentException(
                string.Format("There is no addressable reference finded for task >>{0}<<", color)
                );
        }
    }
}
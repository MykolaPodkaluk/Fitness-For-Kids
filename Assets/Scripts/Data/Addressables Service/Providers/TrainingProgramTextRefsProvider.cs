using Cysharp.Threading.Tasks;
using FitnessForKids.Training;
using System;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace FitnessForKids.Data.Addressables
{
    public interface ILocalizationKeysRefsProvider
    {
    }

    public abstract class TrainingProgramTextRefsProvider<TType> : AddressableRefsProvider<TType, AssetReference> where TType : Enum
    {
        public async UniTask<TKeys> GetRandomData<TKeys>() where TKeys : ITrainingProgramLocalizationData
        {
            var random = new System.Random();
            var values = Enum.GetValues(typeof(ProgramType));
            var keys = (ProgramType)values.GetValue(random.Next(values.Length));
            return await GetData<TKeys>(keys);
        }

        public async UniTask<TKeys> GetData<TKeys>(ProgramType type) where TKeys : ITrainingProgramLocalizationData
        {
            var referenceData = GetReference(type);

            try
            {
                AsyncOperationHandle<TKeys> handler = UnityEngine.AddressableAssets.Addressables.LoadAssetAsync<TKeys>(referenceData.Reference.RuntimeKey);
                await handler;
                return handler.Result;
            }
            catch (Exception)
            {
                throw new ArgumentNullException(
                    string.Format("Can't load ScriptableObject from addressable reference for >>{0}<<", type)
                    );
            }
        }

        private RefPair GetReference(ProgramType type)
        {
            for (int i = 0, j = references.Length; i < j; i++)
            {
                if (references[i].Type.Equals(type))
                {
                    return references[i];
                }
            }
            throw new ArgumentException(
                string.Format("There is no addressable reference finded for task >>{0}<<", type)
                );
        }
    }
}
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.AddressableAssets;
using Cysharp.Threading.Tasks;
using FitnessForKids.UI.Styles;
using System;
using System.Collections.Generic;

namespace FitnessForKids.Data.Addressables
{
    public abstract class TrainingProgramViewStyleRefsProvider<TType> : AddressableRefsProvider<TType, AssetReference> where TType : Enum
    {
        public async UniTask<TView> GetRandomData<TView>() where TView : ITrainingProgramViewStyleData
        {
            var random = new System.Random();
            var values = Enum.GetValues(typeof(ProgramType));
            var view = (ProgramType)values.GetValue(random.Next(values.Length));
            return await GetData<TView>(view);
        }

        public async UniTask<TView> GetData<TView>(ProgramType type) where TView : ITrainingProgramViewStyleData
        {
            var referenceData = GetReference(type);

            try
            {
                AsyncOperationHandle<TView> handler = UnityEngine.AddressableAssets.Addressables.LoadAssetAsync<TView>(referenceData.Reference.RuntimeKey);
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

        public async UniTask<List<TView>> GetAllData<TView>() where TView : ITrainingProgramViewStyleData
        {
            List<RefPair> refPairs = new List<RefPair>();
            foreach (ProgramType item in Enum.GetValues(typeof(ProgramType)))
            {
                if ((int)item != 0)
                {
                    refPairs.Add(GetReference(item));
                }
            } 
             
            List<TView> views = new List<TView>(); 
            List<AsyncOperationHandle<TView>> handlers = new List<AsyncOperationHandle<TView>>();

            foreach (var item in refPairs)
            { 
                AsyncOperationHandle<TView> handler = UnityEngine.AddressableAssets.Addressables.LoadAssetAsync<TView>(item.Reference.RuntimeKey);
                await handler;
                handlers.Add(handler);
            }

            foreach (var item in handlers)
            {
                views.Add(item.Result);
            }

            return views; 
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
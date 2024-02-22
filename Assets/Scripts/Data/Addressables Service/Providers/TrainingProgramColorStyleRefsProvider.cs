using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.AddressableAssets;
using Cysharp.Threading.Tasks;
using FitnessForKids.UI.Styles;
using System;
using System.Collections.Generic;
using UnityEngine;

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

        public async UniTask<List<TSyle>> GetAllData<TSyle>() where TSyle : ITrainingProgramColorStyleData
        {
            List<RefPair> refPairs = new List<RefPair>();
            foreach (ColorScheme item in Enum.GetValues(typeof(ColorScheme)))
            {
                if ((int)item != 0)
                {
                    refPairs.Add(GetReference(item));
                }
            }

            List<TSyle> views = new List<TSyle>();
            List<AsyncOperationHandle<TSyle>> handlers = new List<AsyncOperationHandle<TSyle>>();

            foreach (var item in refPairs)
            { 
                AsyncOperationHandle<TSyle> handler = UnityEngine.AddressableAssets.Addressables.LoadAssetAsync<TSyle>(item.Reference.RuntimeKey);
                await handler;
                handlers.Add(handler);
            }

            foreach (var item in handlers)
            {
                views.Add(item.Result);
            }

            return views;
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
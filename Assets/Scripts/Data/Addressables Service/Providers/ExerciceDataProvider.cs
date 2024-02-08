using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.AddressableAssets;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using UnityEngine;
using FitnessForKids.Data;

namespace FitnessForKids.Training
{
    public class ExerciceDataProvider
    {
        public AsyncOperationHandle<IList<ExerciseData>> GetExerciceDatas(List<string> exerciseDataKeys)
        {
            return Addressables.LoadAssetsAsync<ExerciseData>(exerciseDataKeys, null);
        }

        public async Task<List<ExerciseData>> GetExercisesForProgram(ITrainingParameters trainingParameters)
        {
            List<ExerciseData> exercisesForProgram = new List<ExerciseData>();

            System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();

            // Get all locations of "ExerciseData" resources
            IList<IResourceLocation> exerciseDataLocations = await Addressables.LoadResourceLocationsAsync("ExerciseData").Task;

            // Load all "ExerciseData" objects using the obtained locations
            List<ExerciseData> allExercises = new List<ExerciseData>();
            AsyncOperationHandle<IList<ExerciseData>> loadOperation = Addressables.LoadAssetsAsync<ExerciseData>(exerciseDataLocations, allExercises.Add);
            await loadOperation.Task;

            foreach (ExerciseParametersData parameters in trainingParameters.ExerciseParameters)
            {
                List<ExerciseData> filteredExercises = allExercises
                    .Where(exercise =>
                        (parameters.BodyParts.Contains(exercise.BodyPart) || parameters.BodyParts.Count == 0)
                        && (parameters.DifficultyLevels.Contains(exercise.Difficulty) || parameters.DifficultyLevels.Count == 0)
                        && (parameters.IDs.Contains(exercise.ID) || parameters.IDs.Count == 0)
                    )
                    .ToList();

                if (filteredExercises.Count > 0)
                {
                    int randomIndex = Random.Range(0, filteredExercises.Count);
                    exercisesForProgram.Add(filteredExercises[randomIndex]);
                }
            }

            Addressables.Release(loadOperation);
            stopwatch.Stop();

            long elapsedTimeMs = stopwatch.ElapsedMilliseconds;
            Debug.Log("GetExercisesForProgram Execution time: " + elapsedTimeMs + " ms");

            return exercisesForProgram;
        }
    }
}
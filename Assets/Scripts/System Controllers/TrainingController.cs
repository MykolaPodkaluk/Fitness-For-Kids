using FitnessForKids.Data.Addressables;
using FitnessForKids.Data;
#if UNITY_EDITOR
using FitnessForKids.Data.Providers;
#endif
using System.Collections.Generic;
using FitnessForKids.UI;
using UnityEngine;
using System.Linq;
using Zenject;
using System;
using Cysharp.Threading.Tasks;
using UnityEditor;
using System.IO;  

namespace FitnessForKids.Services
{
    public interface ITrainingController
    {
        ITrainingScreenView View { get; }
        UniTask StartTraining(ITrainingParameters trainingParameters);
        UniTask StopTraining();
        void TogglePause();
        Action OnProgramCompleted { get; set; }
        void Init(ITrainingScreenView trainingScreenView, IAnimationController animator);
    }

    /// <summary>
    /// Controls the training process by managing exercises, sports programs, and the training view.
    /// </summary>
    public class TrainingController : MonoBehaviour, ITrainingController
    {
        private ITrainingScreenView _trainingScreenView;        
        private IAnimationController _animator;
        private ITrainingProgram currentProgram;
        private int currentExerciseIndex;
        private bool isPaused = false;
        private List<float> _currentTrainingDurations;
        [Inject] private IDataService _dataService;
        [Inject] private IAddressableRefsHolder _refsHolder;
        [Inject] private IAdsService _adsService;

#if UNITY_EDITOR
        [SerializeField] private ExerciseAssetsHolder animations;
#endif 
        [SerializeField] private string _scriptableExercisePath;
        [SerializeField] private List<ExerciseData> allExercises;
        public ITrainingScreenView View => _trainingScreenView;
        public Action OnProgramCompleted { get; set; }

        public void Init(ITrainingScreenView trainingScreenView, IAnimationController animator)
        {
            _trainingScreenView = trainingScreenView;
            _animator = animator;
        }

        private void Start()
        {
            SetScreenActive(false);
        }

        private async UniTask Subscribe()
        {
            await UniTask.Delay(200);
            _animator.AnimationEvent.OnExerciseCompleted += UpdateNextExerciseView;
            _animator.AnimationEvent.OnProgramCompleted += CompleteTraining;
        }

        private void Unubscribe()
        {
            _animator.AnimationEvent.OnExerciseCompleted -= UpdateNextExerciseView;
            _animator.AnimationEvent.OnProgramCompleted -= CompleteTraining;
        }

        private void CompleteTraining()
        {
            Unubscribe();
            SaveStatistics();
            OnProgramCompleted?.Invoke();
        }

        private void SaveStatistics()
        {
            Debug.Log("Data Saved!");
            _dataService.SaveProgramData((TrainingProgram)currentProgram);
        }

        public async UniTask StartTraining(ITrainingParameters trainingParameters)
        {
            SetScreenActive(true);
            await _animator.SetActive(true);
            await Subscribe();
            await StartTrainingProgram(trainingParameters);
        }

        private async UniTask StartTrainingProgram(ITrainingParameters trainingParameters)
        {
            currentProgram = new TrainingProgram(trainingParameters.Type, GetExercisesForProgram(trainingParameters));

            currentExerciseIndex = 0;
            var currentExercises = currentProgram.Exercises;
            List<AnimationClip> curentProgramAnimations = new List<AnimationClip>();
            List<int> curentProgramReps = new List<int>();
            List<string> curentProgramIds = new List<string>();

            for (int i = 0; i < currentExercises.Count; i++)
            {
                var exercise = currentExercises[i];
                var id = exercise.ID;
                var animation = _refsHolder.Exercises.AnimationsProvider.TryGetAssetByKey(id);
                curentProgramAnimations.Add(animation);
                curentProgramReps.Add(trainingParameters.ExerciseParameters[i].MinReps);
                curentProgramIds.Add(id);
            }

            GetTrainingDurations(curentProgramAnimations, curentProgramReps);
            var allAnimationsTime = GetAllTrainingDurations(curentProgramAnimations, curentProgramReps);
            UpdateCurrentExerciseView(); 
            await _animator.RunExerciseProgram(curentProgramAnimations, curentProgramReps, curentProgramIds, allAnimationsTime);
        } 

        public async UniTask StopTraining()
        {
            await _animator.StopAnimations();
            _trainingScreenView.Hide(() => SetScreenActive(false));
            _adsService.TryShowInterstitialAds(100);
        }

        private void UpdateNextExerciseView()
        {
            currentExerciseIndex++; 
            if (currentExerciseIndex < currentProgram.Exercises.Count)
            {
                UpdateCurrentExerciseView();
            }
        }

        private void UpdateCurrentExerciseView()
        {
            var currentExercise = currentProgram.Exercises[currentExerciseIndex];
            var exerciseDuration = _currentTrainingDurations[currentExerciseIndex];
            var exerciseName = LocalizationController.GetLocalizedString("Sports Exercises Names", currentExercise.ID);
            _trainingScreenView.UpdateView(
                currentExerciseIndex,
                exerciseName,
                exerciseDuration,
                currentExercise.BodyPart,
                currentExercise.Skills);
        }

        private void SetScreenActive(bool isActive)
        {
            _trainingScreenView.GameObject.SetActive(isActive);
        }

        private void GetTrainingDurations(List<AnimationClip> animations, List<int> reps)
        {
            _currentTrainingDurations = new List<float>();
            for (int i = 0, j = animations.Count; i < j; i++)
            {
                var animation = animations[i];
                var duration = animation.length * reps[i];
                _currentTrainingDurations.Add(duration);
            }
        }

        private List<float> GetAllTrainingDurations(List<AnimationClip> animations, List<int> reps)
        {
            var currentAllTrainingDurations = new List<float>();
            for (int i = 0, j = animations.Count; i < j; i++)
            {
                var animation = animations[i];
                for (int k = 0; k < reps[i]; k++)
                { 
                    var duration = animation.length;
                    currentAllTrainingDurations.Add(duration);
                }
            }
            return currentAllTrainingDurations;
        }

        public List<ExerciseData> GetExercisesForProgram(ITrainingParameters trainingParameters)
        {
            List<ExerciseData> exercisesForProgram = new List<ExerciseData>();

            //System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
            //stopwatch.Start();

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
                    int randomIndex = UnityEngine.Random.Range(0, filteredExercises.Count);
                    exercisesForProgram.Add(filteredExercises[randomIndex]);
                }
            }

            //stopwatch.Stop();

            //long elapsedTimeMs = stopwatch.ElapsedMilliseconds;
            //Debug.Log("GetExercisesForProgram Execution time: " + elapsedTimeMs + " ms");

            return exercisesForProgram;
        }

        public void TogglePause()
        {
            isPaused = !isPaused;

            if (isPaused)
            {
                Time.timeScale = 0f;
            }
            else
            {
                Time.timeScale = 1f;
            }
        }
#if UNITY_EDITOR
        [ContextMenu("Update All Tasks")]
        private void UpdateAllTasks()
        {
            if (!Directory.Exists(_scriptableExercisePath))
            {
                Debug.LogWarning(
                    string.Format("Path folder: {0} not exists. Can't load all exercises", _scriptableExercisePath));
                return;
            }
            allExercises.Clear();

            var exercisesFolders = Directory.GetDirectories(_scriptableExercisePath);
            var temp = animations.AnimationsProvider.GetAllAssets();

            string[] guids = AssetDatabase.FindAssets($@"t:{nameof(ExerciseData)}", exercisesFolders);

            foreach (string guid in guids)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                ExerciseData scriptableExercise = AssetDatabase.LoadAssetAtPath<ExerciseData>(assetPath);

                foreach (var animation in temp)
                {
                    if (animation.name == scriptableExercise.ID)
                    {
                        allExercises.Add(scriptableExercise);
                    } 
                }
            }
        }
#endif
    }
}
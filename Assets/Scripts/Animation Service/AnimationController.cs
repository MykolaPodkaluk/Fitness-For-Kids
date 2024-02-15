using System.Collections.Generic;
using System.Collections;
using System.Linq;
using UnityEngine;
using System;
using Zenject;
using FitnessForKids.UI;
using FitnessForKids.Data.Addressables;
using Cysharp.Threading.Tasks;
using FitnessForKids.Data;
using FitnessForKids.Services;
using FitnessForKids.UI.Subscription;
using System.Threading;

public interface IAnimationController
{
    UniTask RunExerciseProgram(List<AnimationClip> exerciseAnimations, List<int> exerciseReps, List<string> exerciseIds, List<float> currentTrainingDurations); 
    UniTask StopAnimations();
    UniTask SetActive(bool isActive); 
    AnimationEvent AnimationEvent { get; }
}

public class AnimationController : MonoBehaviour, IAnimationController
{
    private Animator _animator; 
    private List<int> animationsRepetitionCount;
    public int AnimationsRepetitionCount => animationsRepetitionCount[_currentAnimationIndex];
    private List<string> animationComplex = new List<string>();
    private List<string> _exerciseIds = new List<string>();
    private int _currentAnimationIndex;
    private int currentAnimationPlayedCount; 
    private IAvatarsView _avatarView;
    private AnimationEvent animationEvent;
    private CancellationTokenSource animationDelay;
    private List<float> currentTrainingDurations;
    private Action playNextAnimation;
    private Action playfirstPrewiew;

    AnimationEvent IAnimationController.AnimationEvent => animationEvent;

    [Inject] IExercisePreviewMediator _exercisePreviewMediator;
    [Inject] IAddressableRefsHolder _refsHolder;
    [Inject] IDataService _dataService;

    private void Start()
    {
    }

    private async UniTaskVoid SetupPreview()
    {
        _exercisePreviewMediator.ON_CLOSE -= playfirstPrewiew;
        await HandleAnimation();
        playNextAnimation = UniTask.Action(PlayNextAnimation);
        _exercisePreviewMediator.ON_CLOSE += playNextAnimation;
    }

    public async UniTask RunExerciseProgram(List<AnimationClip> exerciseAnimations, List<int> exerciseReps, List<string> exerciseIds, List<float> currentTrainingDurations)
    {
        RuntimeAnimatorProvider.SetRuntimeAnimatorController(_animator, exerciseAnimations);

        this.currentTrainingDurations = currentTrainingDurations;
        _exerciseIds = exerciseIds;
        animationComplex.Clear();
        animationComplex = RuntimeAnimatorProvider.AnimationKeys.Take(exerciseAnimations.Count).ToList();
        animationsRepetitionCount = exerciseReps;
        _currentAnimationIndex = 0;
        playfirstPrewiew = UniTask.Action(SetupPreview);
        var currentExerciseId = _exerciseIds[_currentAnimationIndex];
        _exercisePreviewMediator.ON_CLOSE += playfirstPrewiew;
        _exercisePreviewMediator.ON_CLOSE += (() => animationEvent.ExerciseStart());
        _exercisePreviewMediator.CreatePopup(currentExerciseId);
    } 

    private void HandleTrainingProgram()
    {
        if (_currentAnimationIndex < animationComplex.Count) 
        {
            OpenNextExercisePreview();
        }
        else
        {
            animationEvent.ProgramComplete();
        }
    }

    private void OpenNextExercisePreview()
    {
        _animator.enabled = false;
        animationDelay.Cancel();
        var currentExerciseId = _exerciseIds[_currentAnimationIndex];
        _exercisePreviewMediator.CreatePopup(currentExerciseId);
    }

    private async UniTaskVoid PlayNextAnimation()
    { 
        await UniTask.Delay(200);
        animationEvent.ExerciseComplete();
        await HandleAnimation();
    }

    public async UniTask SetActive(bool isActive)
    {
        if(isActive)
        {
            Gender _currentGender = _dataService.UserProfileController.CurrentGender;
            _avatarView = await _refsHolder.TrainingsAvatars.Main.InstantiateFromReference<IAvatarsView>(_currentGender, transform);
            _animator = _avatarView.GetAnimator();
            animationEvent = GetComponentInChildren<AnimationEvent>();
        }
        else
        {
            if(_avatarView != null)
            {
                _avatarView.Release();
            }
        }
        //gameObject.SetActive(isActive);
    }

    private async UniTask HandleAnimation()
    {
        animationDelay = new CancellationTokenSource();
        _animator.enabled = true;
        var state = Animator.StringToHash(animationComplex[_currentAnimationIndex]);

        for (int i = 0; i < animationsRepetitionCount[_currentAnimationIndex]; i++)
        {
            _animator.Play(state, 0);
            await UniTask.Delay(TimeSpan.FromSeconds(currentTrainingDurations[_currentAnimationIndex * animationsRepetitionCount[_currentAnimationIndex]]), cancellationToken: animationDelay.Token);
        }

        _currentAnimationIndex++;
        HandleTrainingProgram();
    }

    public async UniTask StopAnimations()
    {
        StopAllCoroutines();
        animationDelay.Cancel();
        _currentAnimationIndex = 0;
        currentAnimationPlayedCount = 0;
        animationComplex = new List<string>();
        _animator.Rebind();
        _animator.enabled = false;
        await SetActive(false); 
    }
}









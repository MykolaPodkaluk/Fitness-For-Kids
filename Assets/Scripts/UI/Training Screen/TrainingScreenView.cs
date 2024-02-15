using System.Collections.Generic;
using FitnessForKids.Services;
using FitnessForKids.Data;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine;
using Zenject;
using System;
using TMPro;

namespace FitnessForKids.UI
{
    public interface ITrainingScreenView : IView
    {
        void UpdateView(int exerciseIndex, string exerciseName, float exercisetime, BodyPart activeBodyPart, List<Skill> activeSkills);
        void UpdateTimer(float exercisetime);
        Action OnShow { get; set; }
        GameObject GameObject { get; }
    }

    public class TrainingScreenView : MonoBehaviour, ITrainingScreenView
    {
        [SerializeField] private CanvasGroup panelCanvasGroup;
        [SerializeField] private TMP_Text exerciseTitle;
        [SerializeField] private IndicatorCounter exerciseCounter;
        [SerializeField] private BodyPartsPreview bodyPartsPreview;
        [SerializeField] private SkillIndicatorsView skillIndicatorPanel;
        [SerializeField] private Button backwardButton;
        [SerializeField] private Button pauseButton;
        [SerializeField] private RadialTimerView _timerPanel;
        [Inject] private ITrainingController trainingController;
        [Inject] private ITrainingService trainingService;
        [Inject] private IDataService _dataService;

        [SerializeField] private TimeScaleController _timeScaleController;

        public GameObject GameObject => gameObject;
        public Action OnShow { get; set; }

        private void Start()
        {
            backwardButton.onClick.AddListener(trainingService.StopTraining);
            pauseButton.onClick.AddListener(trainingController.TogglePause);
        }

        public void UpdateView(int exerciseIndex, string exerciseName, float exercisetime, BodyPart activeBodyPart, List<Skill> activeSkills)
        {
            //Temp, need to update all ExerciseDatas
            if(activeBodyPart == BodyPart.None)
            {
                activeBodyPart = (BodyPart)UnityEngine.Random.Range(0, 11);
            }

            exerciseTitle.text = exerciseName;
            exerciseCounter.SelectIndicator(exerciseIndex);
            bodyPartsPreview.ShowActiveParts(activeBodyPart);
            skillIndicatorPanel.UpdateIndicators(activeSkills); 
        }

        public void UpdateTimer(float exercisetime)
        {
            _timerPanel.StartTimer(exercisetime);
        }

        public void Show(Action onShow)
        {
            panelCanvasGroup.gameObject.SetActive(true);
            panelCanvasGroup.DOFade(1, 0.5f);
            bodyPartsPreview.Init(_dataService.UserProfileController.CurrentGender);
            onShow?.Invoke();
        }

        public void Hide(Action onHide)
        {
            panelCanvasGroup.DOFade(0, 0.5f).OnComplete(() => panelCanvasGroup.gameObject.SetActive(false));
            _timerPanel.CancelTimer();
            _timeScaleController.Reset();
            if (_dataService != null)
            {
                bodyPartsPreview.Init(_dataService.UserProfileController.CurrentGender, false);
            }
            onHide?.Invoke();
        }

        public void Release()
        {
            Destroy(panelCanvasGroup.gameObject);
        }
    }
}
using FitnessForKids.UI.Animation;
using FitnessForKids.Data;
using UnityEngine.UI;
using UnityEngine;
using System.Linq;
using System;
using TMPro;

namespace FitnessForKids.UI
{
    public interface ITrainingProgramDetailsView : IView
    {
        void UpdateView(string title, string description, string exercises, string time, 
            Sprite baseIcone, Sprite icone, int difficulty, Skill[] skills);
        void SetInputHandler(ITrainingProgramsDetailsInputHandler inputHandler);
    }

    public class TrainingProgramDetailsView : MonoBehaviour, ITrainingProgramDetailsView
    {
        [Header("COMPONENTS:")]
        [SerializeField] private Button _confirmButton;
        [SerializeField] private TMP_Text _titleLebel;
        [SerializeField] private TMP_Text _descriptionLebel;
        [SerializeField] private TMP_Text _exercisesLebel;
        [SerializeField] private TMP_Text _timeLebel;
        [SerializeField] private Image _baseIcone;
        [SerializeField] private Image _icone;
        [SerializeField] private BaseViewAnimator _animator;
        [SerializeField] private DifficultyIndicatorsView _difficultyIndicatorsView;
        [SerializeField] private SkillIndicatorsToggleLikeView _skillIndicatorsView;

        private ITrainingProgramsDetailsInputHandler _inputHandler;

        public void SetInputHandler(ITrainingProgramsDetailsInputHandler inputHandler)
        {
            _inputHandler = inputHandler;
        }

        public void UpdateView(string title, string description, string exercises, string time, 
            Sprite baseIcone, Sprite icone, int difficulty, Skill[] skills)
        {
            SetText(title, description, exercises, time);
            SetIcone(baseIcone, icone);
            _difficultyIndicatorsView.SetDifficulty(difficulty);
            _skillIndicatorsView.UpdateIndicators(skills.ToList());
        }

        private void SetText(string title, string description, string exercises, string time)
        {
            _titleLebel.text = title;
            _descriptionLebel.text = description;
            _exercisesLebel.text = exercises;
            _timeLebel.text = time;
        }

        private void SetIcone(Sprite baseIcone, Sprite icone)
        {
            _baseIcone.sprite = baseIcone;
            _icone.sprite = icone;
        }

        public void Show(Action onShow)
        {
            _animator.AnimateShowing(() =>
            {
                _confirmButton.onClick.AddListener(OnClosedClick);
                onShow?.Invoke();
            });
        }

        public void Hide(Action onHide)
        {
            _confirmButton.onClick.RemoveListener(OnClosedClick);
            _animator.AnimateHiding(() =>
            {
                onHide?.Invoke();
            });
        }

        private void OnClosedClick()
        {
            _inputHandler.ClickOnClose();
        }

        public void Release()
        {
            Destroy(gameObject);
        }
    }
}
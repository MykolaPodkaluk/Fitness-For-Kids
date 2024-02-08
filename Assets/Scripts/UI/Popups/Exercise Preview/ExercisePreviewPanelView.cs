using FitnessForKids.UI.Animation;
using UnityEngine.UI;
using UnityEngine;
using UnityTimer;
using System;
using TMPro;

namespace FitnessForKids.UI
{
    public interface IExercisePreviewPanelView : IView
    {
        void UpdateView(string name, string description);
        void SetInputHandler(IExercisePreviewInputHandler inputHandler);
        public event Action OnTimerCompleted;
    }

    public class ExercisePreviewPanelView : MonoBehaviour, IExercisePreviewPanelView
    {
        [Header("COMPONENTS:")]
        [SerializeField] private Button _confirmButton;
        [SerializeField] private TMP_Text _nameLebel;
        [SerializeField] private TMP_Text _descriptionLebel;
        [SerializeField] private TMP_Text _timeLabel;
        [SerializeField] private BaseViewAnimator _animator;

        private float _timerDuration = 30f;
        private IExercisePreviewInputHandler _inputHandler;
        private Timer _timer;

        public event Action OnTimerCompleted;

        public void SetInputHandler(IExercisePreviewInputHandler inputHandler)
        {
            _inputHandler = inputHandler;
        }

        public void UpdateView(string title, string description)
        {
            _nameLebel.text = title;
            _descriptionLebel.text = description;
            StartTimer(_timerDuration);
        }

        private void StartTimer(float timerDuration)
        {
            CancelTimer();
            _timer = Timer.Register(timerDuration, HandleOnTimerCompleted, HandleOnTimerUpdated, false, false);
        }

        private void CancelTimer()
        {
            Timer.Cancel(_timer);
        }

        private void HandleOnTimerUpdated(float secondsElapsed)
        {
            UpdateTimeLabel(_timer.GetTimeRemaining());
        }

        private void HandleOnTimerCompleted()
        {
            OnClosedClick();
            OnTimerCompleted?.Invoke();
        }

        private void UpdateTimeLabel(float time)
        {
            if (time >= 60f)
            {
                int minutes = Mathf.FloorToInt(time / 60f);
                int seconds = Mathf.FloorToInt(time % 60f);
                _timeLabel.text = string.Format("{0:D2}:{1:D2}m", minutes, seconds);
            }
            else
            {
                int seconds = Mathf.FloorToInt(time);
                _timeLabel.text = string.Format("{0:D2}s", seconds);
            }
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
            CancelTimer();
            _inputHandler.ClickOnClose();
        }

        public void Release()
        {
            Destroy(gameObject);
        }
    }
}
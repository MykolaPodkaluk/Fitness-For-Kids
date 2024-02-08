//using FitnessForKids.Helpers;
using UnityTimer;
using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using System;

namespace FitnessForKids.UI
{
    public class RadialTimerView : MonoBehaviour
    {
        [SerializeField] private Slider _slider;
        [SerializeField] private TMP_Text _timeLabel;

        [SerializeField] private float _maxTime = 10f;

        private Timer _timer;

        public event Action OnTimerStarted;
        public event Action OnTimerUpdated;
        public event Action OnTimerCompleted;

        private void Awake()
        {
            Reset();
        }

        private void Start()
        {
            //StartTimer(_maxTime);
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

        private void Reset()
        {
            CancelTimer();
        }

        public void StartTimer(float timerDuration)
        {
            Reset();

            // Registering a new timer
            _timer = Timer.Register(timerDuration, OnCompleted, OnUpdated, false, false);
            OnStarted();
        }

        private void OnStarted()
        {
            OnTimerStarted?.Invoke();
        }

        private void OnUpdated(float secondsElapsed)
        {
            _slider.value = _timer.GetRatioComplete();
            UpdateTimeLabel(_timer.GetTimeRemaining());
            OnTimerUpdated?.Invoke();
        }

        private void OnCompleted()
        {
            Reset();
            OnTimerCompleted?.Invoke();
        }

        public void CancelTimer()
        {
            Timer.Cancel(_timer);
        }

        public void PauseTimer()
        {
            Timer.Pause(_timer);
        }

        public void ResumeTimer()
        {
            Timer.Resume(_timer);
        }
    }
}
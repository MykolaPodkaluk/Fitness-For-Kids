using FitnessForKids.Services;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using DG.Tweening;

namespace FitnessForKids.UI
{
    public class StartTrainingButton : MonoBehaviour
    {
        [SerializeField] private Button _button;
        [SerializeField] private List<Sprite> _stateSprites;
        [SerializeField] private Vector3 _tweenScale = new Vector3(-0.05f, 0.05f, 1f);
        [SerializeField] private float _tweenDuration = 0.5f;
        [Inject] private ITrainingService _trainingService;

        private void OnEnable()
        {
            Subscribe(true);
        }

        private void OnDisable()
        {
            Subscribe(false);
        }

        private void Subscribe(bool isSubscribed)
        {
            if (isSubscribed)
            {
                _button.onClick.AddListener(OnClickHandler);
            }
            else
            {
                _button.onClick.RemoveListener(OnClickHandler);
            }
        }

        private void OnClickHandler()
        {
            _button.image.sprite = _stateSprites[1];
            _button.interactable = false;
            transform.DOPunchScale(_tweenScale, _tweenDuration, 10, 1).OnComplete(OnTweenComplete);
        }

        private void OnTweenComplete()
        {
            _button.image.sprite = _stateSprites[0];
            _trainingService.StartSelectedTraining();
        }

        public void SetInteractable(bool isInteractable)
        {
            _button.interactable = isInteractable;
        }
    }
}
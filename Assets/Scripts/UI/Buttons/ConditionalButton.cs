using System;
using UnityEngine;
using UnityEngine.UI;

namespace FitnessForKids.UI.Helpers
{
    public interface IConditionalButton
    {
        void Initialize(
            Func<bool> validateConditions,
            Action positiveCallback,
            Action negativeCallback);
    }

    public class ConditionalButton : MonoBehaviour, IConditionalButton
    {
        private Action _positiveCallback;
        private Action _negativeCallback;
        private Func<bool> _validateConditions;

        [SerializeField] private Button _button;

        public void Initialize(Func<bool> validationMethod, Action positiveMethod, Action negativeMethod)
        {
            _validateConditions = validationMethod;
            _positiveCallback = positiveMethod;
            _negativeCallback = negativeMethod;
        }

        private void OnEnable()
        {
            _button.onClick.AddListener(OnButtonClicked);
        }

        public void OnDisable()
        {
            _button.onClick.RemoveListener(OnButtonClicked);
        }

        private void OnButtonClicked()
        {
            if (_validateConditions != null && _validateConditions())
            {
                _positiveCallback?.Invoke();
            }
            else
            {
                _negativeCallback?.Invoke();
            }
        }
    }
}
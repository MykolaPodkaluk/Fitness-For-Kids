using FitnessForKids.Data;
using UnityEngine.UI;
using UnityEngine;
using DG.Tweening;

namespace FitnessForKids.UI
{
    public class BodyPartView : MonoBehaviour
    {
        [SerializeField] private Image _image;
        [SerializeField] private BodyPart _bodyPart;
        public BodyPart BodyPart => _bodyPart;
        public bool IsActive
        {
            get => gameObject.activeInHierarchy;
            set => SetActive(value);
        }
        private float tweenDuration = 0.5f; 

        private void SetActive(bool value)
        {
            _image.DOFade(value ? 1f : 0f, tweenDuration).
                OnComplete(() => gameObject.SetActive(value));
        }
    }
}
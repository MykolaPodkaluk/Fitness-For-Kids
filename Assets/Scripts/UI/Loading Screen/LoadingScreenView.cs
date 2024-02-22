using FitnessForKids.UI.Animation;
using UnityEngine;
using DG.Tweening;
using System;
using TMPro;  
using UnityEngine.Localization.Settings;

namespace FitnessForKids.UI
{
    public interface ILoadingScreenView : IView
    {

    }

    public class LoadingScreenView : MonoBehaviour, ILoadingScreenView
    {
        #region FIELDS

        [Header("COMPONENTS:")]
        [SerializeField] private TMP_Text titleLebel;
        [SerializeField] private BaseViewAnimator _animator;

        [Header("TWEEN CONFIG:")]
        [SerializeField] private float duration = 1f;
        [SerializeField] private float charDelay = 0.1f;
        [SerializeField] private Vector3 charOffset = new Vector3(0, 10, 0);
        [SerializeField] private Vector3 charScale = new Vector3(1.01f, 1.01f, 0);

        private DOTweenTMPAnimator tweenAnimator;
        private Sequence sequence;
        private int _loadingTweenCounter;
        public int LoadingTweenCounter => _loadingTweenCounter;

        #endregion

        private void Start()
        {
            LocalizeTitle("Loading");
            TweenLoadingText();
        }

        private void OnDestroy()
        {
            sequence.Kill();
        }

        private void LocalizeTitle(string title)
        {
            titleLebel.text = LocalizationSettings.StringDatabase.GetLocalizedString("GUI Elements", title);
        }

        private void TweenLoadingText()
        {
            tweenAnimator = new DOTweenTMPAnimator(titleLebel);
            sequence = DOTween.Sequence();

            for (int i = 0; i < tweenAnimator.textInfo.characterCount; ++i)
            {
                if (!tweenAnimator.textInfo.characterInfo[i].isVisible) continue;
                sequence.Join(tweenAnimator.DOPunchCharOffset(i, charOffset, duration, 2, 1).SetDelay(charDelay));
                sequence.Join(tweenAnimator.DOPunchCharScale(i, charScale, duration, 1, 1).SetDelay(charDelay));
                sequence.OnStepComplete(OnLoadingTweenComplete);
                sequence.SetLoops(-1);
            }
        }

        private void OnLoadingTweenComplete()
        {
            _loadingTweenCounter++;
        }

        public void Show(Action onShow)
        {
            _animator.AnimateShowing(() =>
            {
                onShow?.Invoke();
            });
        }

        public void Hide(Action onHide)
        {
            _animator.AnimateHiding(() =>
            {
                onHide?.Invoke();
            });
        }

        public void Release()
        {
            Destroy(gameObject);
        }
    }
}
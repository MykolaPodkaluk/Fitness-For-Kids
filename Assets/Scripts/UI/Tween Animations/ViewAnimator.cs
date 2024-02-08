using UnityEngine;
using DG.Tweening;
using System;

namespace FitnessForKids.UI.Animation
{
    public interface IViewAnimator
    {
        public void AnimateShowing(Action onComplete);
        public void AnimateHiding(Action onComplete);
    }

    public abstract class BaseViewAnimator : MonoBehaviour, IViewAnimator
    {
        public abstract void AnimateHiding(Action onComplete);
        public abstract void AnimateShowing(Action onComplete);

        protected virtual void OnDisable()
        {
            DOTween.Kill(transform);
        }
    }
}
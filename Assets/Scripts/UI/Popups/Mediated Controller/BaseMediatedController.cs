﻿using Cysharp.Threading.Tasks;
using System;

namespace FitnessForKids.UI
{
    public interface IBaseMediatedController : IView
    {
        UniTask Init(IView view);
    }


    public abstract class BaseMediatedController<TView, TModel> : 
        IBaseMediatedController where TView : IView where TModel : IModel
    {
        protected TView _view;
        protected TModel _model;

        public async UniTask Init(IView view)
        {
            _view = (TView)view;
            _model = await BuildModel();
            await DoOnInit(_view);
        }

        protected abstract UniTask DoOnInit(TView view);
        protected abstract UniTask<TModel> BuildModel();
        public virtual void Show(Action onShow = null)
        {
        }

        public virtual void Hide(Action onHide)
        {
        }

        public virtual void Release()
        {
        }
    }
}
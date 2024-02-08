using FitnessForKids.Data;
using UnityEngine.UI;
using UnityEngine;
using DG.Tweening;
using System;

public class TrainingProgramSelectableButton : MonoBehaviour
{
    [SerializeField] private ProgramType _programType;
    [SerializeField] private Button _button;
    [SerializeField] private Image _selectorImage;
    [SerializeField] private RectTransform _rectTransform;
    [SerializeField] private Vector3 _tweenScale = new Vector3(-0.05f, 0.05f, 1f);
    [SerializeField] private float _tweenDuration = 0.5f;
    public event Action OnClick;
    public ProgramType ProgramType => _programType;
    public RectTransform RectTransform => _rectTransform;

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
            _button.onClick.AddListener(OnCLick);
        }
        else
        {
            _button.onClick.RemoveListener(OnCLick);
        }
    }

    public void Select()
    {
        _selectorImage.enabled = true;
        _selectorImage.color = new Color(1, 1, 1, 0);
        _selectorImage.DOFade(1, 0.5f);
    }

    public void Deselect()
    {
        _selectorImage.enabled = true;
        _selectorImage.DOFade(0, 0.5f).OnComplete(() => _selectorImage.enabled = false);
    }

    private void OnCLick()
    {
        OnClick?.Invoke();
        DOTween.Kill(transform);
        _rectTransform.localScale = Vector3.one;
        _rectTransform.DOPunchScale(_tweenScale, _tweenDuration, 10, 1).SetId(transform);
    }
}

using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class DatePickerPanel : MonoBehaviour
{
    #region FIELDS

    [Header("COMPONENTS:")]
    [SerializeField] private SnapScrollPicker dayPicker;
    [SerializeField] private SnapScrollPicker monthPicker;
    [SerializeField] private SnapScrollPicker yearPicker;

    [Header("CONFIG:")]
    [SerializeField] private int minYear = 1950;
    //[SerializeField] private int maxYear = 2023;
    
    [Header("TWEEN:")]
    [SerializeField] private RectTransform popupPanel;
    [SerializeField] private CanvasGroup background;
    [SerializeField] private float tweenDuration = 0.5f;

    private Sequence openSequence;
    private Sequence closeSequence;

    private int day;
    private int month;
    private int year;
    private List<int> yearRange;
    private List<int> daysOfMonths = new List<int>
    { 31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31};

    public UnityEvent<DateTime> OnClosed { get; set; } = new UnityEvent<DateTime>();

    #endregion

    #region MONO AND INITIALIZATION

    private void Awake()
    {
        Initialize();
    }

    protected virtual void OnEnable()
    {
        OpenPanel();
        //SetScrollPositions();
    }

    private void Initialize()
    {
        InitializeTweens();
        InitializeYearRange();
        SubscribeOnScrollPickers();
    }

    private void SetScrollPositions()
    {
        dayPicker.ScrollPosition = new Vector2(dayPicker.ScrollPosition.x, 0);
        monthPicker.ScrollPosition = new Vector2(monthPicker.ScrollPosition.x, 0);
        //yearPicker.ScrollPosition = Vector2.zero;

    }

    private void InitializeTweens()
    {
        openSequence = DOTween.Sequence().SetAutoKill(false).OnComplete(() => OnComplete(true));
        closeSequence = DOTween.Sequence().SetAutoKill(false).OnComplete(() => OnComplete(false));

        if (background != null)
        {
            openSequence.Join(background.DOFade(0, 0));
            openSequence.Append(background.DOFade(1, tweenDuration));
            closeSequence.Join(background.DOFade(0, tweenDuration));
        }
        if (popupPanel != null)
        {
            openSequence.Join(popupPanel.DOScale(Vector3.zero, 0f));
            openSequence.Append(popupPanel.DOScale(Vector2.one, tweenDuration)).SetEase(Ease.InOutBack);
            closeSequence.Join(popupPanel.DOScale(Vector2.zero, tweenDuration / 2)).SetEase(Ease.InOutBack);
        }            

        openSequence.Pause();
        closeSequence.Pause();
    }

    private void InitializeYearRange()
    {
        yearRange = Enumerable.Range(minYear, DateTime.Today.Year - minYear + 1 ).ToList();
        yearPicker.InitializeElements(yearRange);
    }

    private void SubscribeOnScrollPickers()
    {
        dayPicker.OnElementSelected.AddListener(GetDay);
        monthPicker.OnElementSelected.AddListener(GetMonth);
        monthPicker.OnElementSelected.AddListener(CheckFutureDates);
        yearPicker.OnElementSelected.AddListener(GetYear);
        yearPicker.OnElementSelected.AddListener(CheckFutureDates);
    }

    private void SetDaysOfMonthAmonth()
    {
        var today = DateTime.Now;        
        if (year == today.Year && month == today.Month)
        {
            dayPicker.ActiveElementsAmount = today.Day;
        }
        else
        {
            dayPicker.ActiveElementsAmount = daysOfMonths[monthPicker.ClosestElementIndex];
        }
    }

    private void CheckFutureDates()
    {
        var today = DateTime.Now;
        if (year == today.Year && month == today.Month)
        {
            dayPicker.ActiveElementsAmount = today.Day;
        }
        else
        {
            dayPicker.ActiveElementsAmount = daysOfMonths[monthPicker.ClosestElementIndex];
        }
        monthPicker.ActiveElementsAmount = year == today.Year ? today.Month : 12;
    }

#endregion

    #region GET DATE

    private void GetDay()
    {
        day = dayPicker.ClosestElementIndex + 1;
    }
    private void GetMonth()
    {
        month = monthPicker.ClosestElementIndex + 1;
    }
    private void GetYear()
    {
        year = yearRange[yearPicker.ClosestElementIndex];
    }

    public DateTime GetDate()
    {
        DateTime date;
        date = new DateTime(year, month, day);
        return date;
    }

    public void OnSetDate()
    {
        if (OnClosed != null)
            OnClosed.Invoke(GetDate());
        ClosePanel();
    }

    #endregion

    #region TWEEN

    public void OpenPanel()
    {
        openSequence.Restart();
    }

    public void ClosePanel()
    {
        closeSequence.Restart();
    }

    protected virtual void OnComplete(bool isOpened)
    {
        gameObject.SetActive(isOpened);
    }

    #endregion
}

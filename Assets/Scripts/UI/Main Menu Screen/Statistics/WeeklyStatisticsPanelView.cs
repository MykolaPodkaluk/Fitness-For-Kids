using System.Collections.Generic;
using UnityEngine;
using System;

public interface IWeeklyStatisticsPanelView 
{
    public void InitView(Dictionary<DateTime, List<int>> statistics);
}

public class WeeklyStatisticsPanelView : MonoBehaviour, IWeeklyStatisticsPanelView
{
    [SerializeField] private List<HistogramView> histograms;
    private List<DateTime> allDates = new List<DateTime>();
    private List<List<int>> allValues = new List<List<int>>();

    private void Start()
    {
        LocalizationController.OnLanguageChanged.AddListener(OnLanguageChanged);
    }

    private void OnLanguageChanged()
    {
        InitHistograms();
    }

    private void InitHistograms()
    {
        for (int i = 0; i < histograms.Count; i++)
        {
            histograms[i].Init(allDates[i], allValues[i]);
        }
    }

    public void InitView(Dictionary<DateTime, List<int>> statistics)
    {
        allDates = new List<DateTime>();
        allValues = new List<List<int>>();

        foreach (KeyValuePair<DateTime, List<int>> entry in statistics)
        {
            DateTime date = entry.Key;
            List<int> value = entry.Value;

            allDates.Add(date);
            allValues.Add(value);
        }

        InitHistograms();
    }
}
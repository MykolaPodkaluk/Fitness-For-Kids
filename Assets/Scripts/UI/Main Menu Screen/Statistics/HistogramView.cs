using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HistogramView : MonoBehaviour
{
    [SerializeField] TMP_Text _dateLabel;
    [SerializeField] List<LayoutElement> sections;

    private const string kTableKey = "DateTime";
    private const string kDayTypeKey = "_Short";

    public void Init (List<int> sectionSizes)
    {
        for (int i = 0; i < sections.Count; i++)
        {
            sections[i].flexibleHeight = sectionSizes[i];
            sections[i].gameObject.SetActive(sectionSizes[i] > 0);
        }
    }

    public void Init(DateTime date, List<int> sectionSizes)
    {
        var dayOfWeekKey = $"{date.DayOfWeek}{kDayTypeKey}";
        var dayOfWeek = LocalizationController.GetLocalizedString(kTableKey, dayOfWeekKey);
        var dateText = date.ToString("dd,MM");
        var dayText = $"{dayOfWeek} {dateText}";
        _dateLabel.text = dayText;
        for (int i = 0; i < sections.Count; i++)
        {
            sections[i].flexibleHeight = sectionSizes[i];
            sections[i].gameObject.SetActive(sectionSizes[i] > 0);
        }
    }
}
using System.Collections.Generic;
using UnityEngine;

public class IndicatorCounter : MonoBehaviour
{
    [SerializeField] private List<IndicatorImage> indicators;
    private int currentIndicatorIndex = 0;

    private void Start()
    {
        SetIndicatorsState();
    }

    public void SelectIndicator(int index)
    {
        if (index >= 0 && index < indicators.Count)
        {
            currentIndicatorIndex = index;
            SetIndicatorsState();
        }
        else
        {
            Debug.LogWarning("Invalid indicator index: " + index);
        }
    }

    private void SetIndicatorsState()
    {
        for (int i = 0; i < indicators.Count; i++)
        {
            IndicatorState state = GetIndicatorState(i);
            indicators[i].SetState(state);
        }
    }

    private IndicatorState GetIndicatorState(int index)
    {
        if (index == currentIndicatorIndex)
        {
            return IndicatorState.Active;
        }
        else if (index < currentIndicatorIndex)
        {
            return IndicatorState.Completed;
        }
        else
        {
            return IndicatorState.Inactive;
        }
    }
}
using UnityEngine.UI;
using UnityEngine;
using System;

public class DifficultyIndicatorsView : MonoBehaviour
{
    [SerializeField] Image[] indicators;
    private int _difficulty;
    public void SetDifficulty(int difficulty)
    {
        _difficulty = difficulty;
        UpdateIndicators();
    }

    private void UpdateIndicators()
    {
        for (int i = 0; i < indicators.Length; i++)
        {
            bool isActive = i < _difficulty;
            indicators[i].gameObject.SetActive(isActive);
        }
    }
}

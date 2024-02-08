using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DatePickerElement : MonoBehaviour
{
    #region FIELDS

    [SerializeField] private RectTransform _rectTransform;
    [SerializeField] private TMP_Text textLabel;
    [SerializeField] private int _value;
    public int Value
    {
        get => _value;
        set
        {
            _value = value;
            Init();
        }
    }
    public bool IsActive
    {
        get => gameObject.activeInHierarchy;
        set => gameObject.SetActive(value);
    }
    public Vector2 AnchoredPosition
    {
        get => _rectTransform.anchoredPosition;
    }

    #endregion

    private void Init()
    {
        textLabel.text = _value.ToString();
        textLabel.name = $"{_value} (TMP)";
    }
}

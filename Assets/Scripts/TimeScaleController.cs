using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class TimeScaleController : MonoBehaviour
{
    [SerializeField] private TMP_Text _timeScaleLabel;
    [SerializeField] private Slider _slider;

    private void OnEnable()
    {
        _slider.onValueChanged.AddListener(SetTimeScale);
    }

    private void OnDisable()
    {
        _slider.onValueChanged.RemoveListener(SetTimeScale);
    }

    public void Reset()
    {
        _slider.value = 1;
        SetTimeScale(1);
    }

    private void SetTimeScale(float timeScale)
    {
        Time.timeScale = timeScale;
        _timeScaleLabel.text = timeScale.ToString();
    }
}

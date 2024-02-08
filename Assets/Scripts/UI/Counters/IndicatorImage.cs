using UnityEngine;
using UnityEngine.UI;

public class IndicatorImage : MonoBehaviour, IIndicatorImage
{
    [SerializeField] private Image _image;
    [SerializeField] private Sprite[] stateSprites;
    private IndicatorState _state;
    public IndicatorState State
    {
        get => _state;
    }

    public void SetState(IndicatorState state)
    {
        _state = state;
        _image.sprite = stateSprites[(int)state];
    }
}

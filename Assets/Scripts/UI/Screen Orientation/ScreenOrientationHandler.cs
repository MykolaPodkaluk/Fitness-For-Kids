using UnityEngine; 
using UnityEditor;

public class ScreenOrientationHandler : MonoBehaviour
{
    [SerializeField] private RectTransform panelRectTransform;

    private ScreenOrientation lastOrientation;

    public delegate void OrientationChangedHandler(ScreenOrientation newOrientation);
    public static event OrientationChangedHandler OnOrientationChanged;

    private void Start()
    {
        lastOrientation = Screen.orientation;
        OnOrientationChanged += HandleOrientationChanged;
        OnOrientationChanged?.Invoke(Screen.orientation);
    }

    private void OnDestroy()
    {
        OnOrientationChanged -= HandleOrientationChanged;
    }

    private void Update()
    {
        if (Screen.orientation != lastOrientation)
        {
            lastOrientation = Screen.orientation;
            OnOrientationChanged?.Invoke(lastOrientation);
        }
    }

    private void HandleOrientationChanged(ScreenOrientation newOrientation)
    {
        if (newOrientation == ScreenOrientation.Portrait)
        {
            panelRectTransform.anchorMax = new Vector2(0.95f, 0.65f);
        }
        else if (newOrientation == ScreenOrientation.LandscapeLeft || newOrientation == ScreenOrientation.LandscapeRight)
        {
            panelRectTransform.anchorMax = new Vector2(0.95f, 0.85f);
        }
    }
}
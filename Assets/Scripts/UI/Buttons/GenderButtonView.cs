using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using static UnityEngine.UI.Button;

public class GenderButtonView : MonoBehaviour
{
    #region FIELDS

    [Header("COMPONENTS:")]
    [SerializeField] private Image basicImage;
    [SerializeField] private GameObject selector;
    [SerializeField] private Button button;

    [Header("REFERENCES:")]
    [SerializeField] private List<Sprite> stateSprites;

    private SelectableState state = SelectableState.Default;
    public SelectableState State
    {
        get => state;
        set { SetState(value); }
    }

    public ButtonClickedEvent onClick
    {
        get { return button.onClick; }
        set { button.onClick = value; }
    }

    #endregion

    #region MONO AND INITIALIZATION

    private void Start()
    {
        SetState(SelectableState.Default);
    }

    #endregion

    #region STATE

    private void SetState(SelectableState value)
    {
        switch (value)
        {
            case SelectableState.Default:
                state = SelectableState.Default;
                basicImage.sprite = stateSprites[0];
                selector.SetActive(false);
                break;
            case SelectableState.Selected:
                state = SelectableState.Selected;
                basicImage.sprite = stateSprites[0];
                selector.SetActive(true);
                break;
            case SelectableState.Deselected:
                state = SelectableState.Deselected;
                basicImage.sprite = stateSprites[1];
                selector.SetActive(false);
                break;
        }
    }

    #endregion
}

public enum SelectableState
{
    Default,
    Selected,
    Deselected
}
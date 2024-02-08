using FitnessForKids.UI.Animation;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using System;
using TMPro;

public interface INewProfileScreenView : IView
{
    void Init(NewProfileScreenViewModel model, bool isFirstRegistration);
    public event Action ON_CLOSE_CLICK;
}

public class NewProfileScreenView : MonoBehaviour, INewProfileScreenView
{
    #region FIELDS

    [Header("BASIC COMPONENTS:")]
    [SerializeField] private TMP_Text titleLebel;
    [SerializeField] private TMP_Text continueButtonTextLabel;
    [SerializeField] private Button _continueButton;
    [SerializeField] private Button _backwardButton;
    [SerializeField] private Button _closeButton;
    [SerializeField] private BaseViewAnimator _animator;
    [SerializeField] private List<GameObject> stateContainers;

    [Header("NAME AND DATE COMPONENTS:")]
    [SerializeField] private TMP_InputField nameInputField;
    [SerializeField] private Button datePickerButton;
    [SerializeField] private TMP_Text dateOfBirthLabel;
    [SerializeField] private DatePickerPanel datePicker;

    [Header("GENDER COMPONENTS:")]
    [SerializeField] private GenderButtonView boyGenderButton;
    [SerializeField] private GenderButtonView girlGenderButton;

    [Header("BODY PARAMETERS COMPONENTS:")]
    [SerializeField] private EventToggleGroup measurementSystemToggles;
    [SerializeField] private Toggle metricSystemToggle;
    [SerializeField] private Toggle imperialSystemToggle;
    [SerializeField] private TMP_InputField heightInputField;
    [SerializeField] private TMP_InputField weightInputField;
    [SerializeField] private TMP_Text heightUnitLabel;
    [SerializeField] private TMP_Text weightUnitLabel;

    [Header("GREETING COMPONENTS:")]
    [SerializeField] private TMP_Text greetingLabel;

    private NewProfileScreenViewModel _model;
    public event Action ON_CLOSE_CLICK;

    #endregion

    #region INITIALIZATION

    public void Init(NewProfileScreenViewModel model, bool isFirstRegistration)
    {
        _model = model;
        _closeButton.gameObject.SetActive(!isFirstRegistration);
    }

    private void Subscribe()
    {
        _closeButton.onClick.AddListener(OnCloseClick);
        _continueButton.onClick.AddListener(GoNextState);
        _backwardButton.onClick.AddListener(GoPreviousState);
        boyGenderButton.onClick.AddListener(SelectBoyGender);
        girlGenderButton.onClick.AddListener(SelectGirlGender);
        datePickerButton.onClick.AddListener(OpenDatePickerPanel);
        datePicker.OnClosed.AddListener(SelectDateOfBirth);
        nameInputField.onEndEdit.AddListener(_model.SetName);
        measurementSystemToggles.onActiveTogglesChanged.AddListener(SelectMeasurementSystem);
        heightInputField.onEndEdit.AddListener(SetHeight);
        weightInputField.onEndEdit.AddListener(SetWeight);

        _model.OnContinueButtonStatusChanged += (isInteractable) => SetContinueButtonInteractable(isInteractable);
        _model.OnMeasurementSystemSelected += UpdateMeasurementSystemText;
    }

    private void Unsubscribe()
    {
        _closeButton.onClick.RemoveListener(OnCloseClick);
        _continueButton.onClick.RemoveListener(GoNextState);
        _backwardButton.onClick.RemoveListener(GoPreviousState);
        boyGenderButton.onClick.RemoveListener(SelectBoyGender);
        girlGenderButton.onClick.RemoveListener(SelectGirlGender);
        datePickerButton.onClick.RemoveListener(OpenDatePickerPanel);
        datePicker.OnClosed.RemoveListener(SelectDateOfBirth);
        nameInputField.onEndEdit.RemoveListener(_model.SetName);
        measurementSystemToggles.onActiveTogglesChanged.RemoveListener(SelectMeasurementSystem);
        heightInputField.onEndEdit.RemoveListener(SetHeight);
        weightInputField.onEndEdit.RemoveListener(SetWeight);

        _model.OnContinueButtonStatusChanged -= (isInteractable) => SetContinueButtonInteractable(isInteractable);
        _model.OnMeasurementSystemSelected -= UpdateMeasurementSystemText;
    }

    #endregion

    #region LISTENERS

    private void GoNextState()
    {
        SwitchRegistrationState(true);
    }

    private void GoPreviousState()
    {
        SwitchRegistrationState(false);
    }

    private void SelectBoyGender()
    {
        SelectGender(Gender.Boy);
    }

    private void SelectGirlGender()
    {
        SelectGender(Gender.Girl);
    }

    #endregion

    private void SwitchRegistrationState(bool goForward)
    {
        stateContainers[_model.CurrentRegistrationStateIndex].SetActive(false);

        _model.CurrentRegistrationStateIndex += goForward ? 1 : -1;

        if (_model.CurrentRegistrationStateIndex < 0)
        {
            _model.CurrentRegistrationStateIndex = 0;
        }
        else if (_model.CurrentRegistrationStateIndex >= stateContainers.Count)
        {
            _model.CurrentRegistrationStateIndex = stateContainers.Count - 1;
            _model.CompleteRegistration();
        }

        if (_model.CurrentRegistrationStateIndex == stateContainers.Count - 1)
        {
            greetingLabel.text = _model.LocalizedGreeting;
        }

        stateContainers[_model.CurrentRegistrationStateIndex].SetActive(true);
        _backwardButton.gameObject.SetActive(_model.CurrentRegistrationStateIndex > 0);
        continueButtonTextLabel.text = _model.LocalizedContinueButtonText(stateContainers.Count - 1);
        titleLebel.text = _model.LocalizedTitleLebel(stateContainers.Count);
        _model.ValidateContinueButtonStatus();
    }

    private void OpenDatePickerPanel()
    {
        datePicker.gameObject.SetActive(true);
    }

    private void SelectDateOfBirth(DateTime date)
    {
        dateOfBirthLabel.text = date.ToString(format: "dd/MM/yy");
        _model.SetDateOfBirth(date);
    }

    private void SelectGender(Gender gender)
    {
        if (gender == Gender.Boy)
        {
            boyGenderButton.State = SelectableState.Selected;
            girlGenderButton.State = SelectableState.Deselected;
        }
        else
        {
            boyGenderButton.State = SelectableState.Deselected;
            girlGenderButton.State = SelectableState.Selected;
        }
        _model.SetGender(gender);
    }

    private void SelectMeasurementSystem(Toggle toggle)
    {
        if (toggle == metricSystemToggle)
        {
            _model.SetMeasurementSystem(MeasurementSystem.Metric);
        }
        else
        {
            _model.SetMeasurementSystem(MeasurementSystem.Imperial);
        }
    }

    private void UpdateMeasurementSystemText()
    {
        heightUnitLabel.text = _model.LocalizedHeight;
        weightUnitLabel.text = _model.LocalizedWeight;

        heightInputField.text = _model.ConvertHeight();
        weightInputField.text = _model.ConvertWeight();
    }

    private void SetHeight(string height)
    {
        int result;
        bool success = int.TryParse(height, out result);
        if (success)
        {
            _model.SetHeight(result);
        }
    }

    private void SetWeight(string weight)
    {
        int result;
        bool success = int.TryParse(weight, out result);
        if (success)
        {
            _model.SetWeight(result);
        }
    }

    private void ResetToDefault()
    {
        _model.Init();

        foreach (var state in stateContainers)
        {
            state.SetActive(false);
        }

        stateContainers[_model.CurrentRegistrationStateIndex].SetActive(true);

        nameInputField.text = String.Empty;
        dateOfBirthLabel.text = "DD/MM/YY";
        heightInputField.text = String.Empty;
        weightInputField.text = String.Empty;
        boyGenderButton.State = SelectableState.Default;
        girlGenderButton.State = SelectableState.Default;
    }
    private void SetContinueButtonInteractable(bool isInteractable)
    {
        _continueButton.interactable = isInteractable;
        continueButtonTextLabel.alpha = isInteractable ? 1 : 0.5f;
    }

    public void Show(Action onShow)
    {
        ResetToDefault();
        SetContinueButtonInteractable(false);
        _animator.AnimateShowing(() =>
        {
            Subscribe();
            onShow?.Invoke();
        });
    }

    public void Hide(Action onHide)
    {
        Unsubscribe();
        _animator.AnimateHiding(() =>
        {
            onHide?.Invoke();
        });
    }

    public void Release()
    {
        Destroy(gameObject);
    }

    private void OnCloseClick()
    {
        ON_CLOSE_CLICK?.Invoke();
    }
}
using System.Collections.Generic;
using FitnessForKids.Data;
using UnityEngine.Events;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine;
using System;
using TMPro;

public class NewPlayerScreenView : MonoBehaviour
{
    #region FIELDS

    [Header("BASIC COMPONENTS:")]
    [SerializeField] private TMP_Text titleLebel;
    [SerializeField] private TMP_Text continueButtonTextLabel;
    [SerializeField] private Canvas viewCanvas;
    [SerializeField] private CanvasGroup viewCanvasGroup;
    [SerializeField] private Button continueButton;
    [SerializeField] private Button backwardButton;
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

    [Header("EVENTS:")]
    public UnityEvent<UserProfileData> OnRegistrationCompleted;
    public UnityEvent<string> OnNameSet;
    public UnityEvent<DateTime> OnDateOfBirthSet;
    public UnityEvent<Gender> OnGenderSelect;
    public UnityEvent<MeasurementSystem> OnMeasurementSystemSelect;
    public UnityEvent<int> OnHeightSet;
    public UnityEvent<int> OnWeightSet;

    private int currentRegistrationStateIndex;
    private int currentHeight;
    private int currentWeight;
    private MeasurementSystem currentMeasurementSystem = MeasurementSystem.Metric;
    private const string kTableKey = "Player Registration";
    private const string kKgKey = "Kilogram";
    private const string kLbKey = "Pound";
    private const string kCmKey = "Centimetre";
    private const string kInKey = "Inch";
    private const string kTellUsKey = "Tell Us";
    private const string kWelcomeKey = "Welcome";
    private const string kContinueKey = "Continue";
    private const string kGoKey = "Go";
    private bool isImperialSystem => currentMeasurementSystem == MeasurementSystem.Imperial;
    private UserProfileData newPlayerData = new UserProfileData();

    private int minNameLength = 3;

    public bool IsActive
    {
        get => gameObject.activeInHierarchy;
        set
        {
            viewCanvas.gameObject.SetActive(value);
            if (value)
            {
                OpenPanel();
            }
        }
    }

    #endregion

    #region MONO AND INITIALIZATION

    private void Awake()
    {
        Initialize();
    }

    private void Initialize()
    {
        continueButton.onClick.AddListener(() => SwitchRegistrationState(true));
        backwardButton.onClick.AddListener(() => SwitchRegistrationState(false));
        boyGenderButton.onClick.AddListener(() => SelectGender(Gender.Boy));
        girlGenderButton.onClick.AddListener(() => SelectGender(Gender.Girl));
        datePickerButton.onClick.AddListener(OpenDatePickerPanel);
        datePicker.OnClosed.AddListener(SetDateOfBirth);
        nameInputField.onEndEdit.AddListener(SetName);
        measurementSystemToggles.onActiveTogglesChanged.AddListener(SelectMeasurementSystem);
        heightInputField.onEndEdit.AddListener(SetHeight);
        weightInputField.onEndEdit.AddListener(SetWeight);

        DebugAllEvents();
        UpdateContinueButtonStatus();
    }

    private void DebugAllEvents()
    {
        OnNameSet.AddListener((name) => newPlayerData.Name = name);
        OnDateOfBirthSet.AddListener((date) => newPlayerData.DateOfBirth = date);
        OnGenderSelect.AddListener((gender) => newPlayerData.Gender = gender);
        OnMeasurementSystemSelect.AddListener((system) => newPlayerData.MeasurementSystem = system);
        OnHeightSet.AddListener((height) => newPlayerData.Height = height);
        OnWeightSet.AddListener((weight) => newPlayerData.Weight = weight);

        OnNameSet.AddListener((name) => UpdateContinueButtonStatus());
        OnDateOfBirthSet.AddListener((date) => UpdateContinueButtonStatus());
        OnGenderSelect.AddListener((gender) => UpdateContinueButtonStatus());
        OnMeasurementSystemSelect.AddListener((system) => UpdateContinueButtonStatus());
        OnHeightSet.AddListener((height) => UpdateContinueButtonStatus());
        OnWeightSet.AddListener((weight) => UpdateContinueButtonStatus());
    }

    #endregion

    private bool IsNameValid()
    {
        bool isValid = false;
        isValid = newPlayerData.Name != null && 
            newPlayerData.Name != "" && 
            newPlayerData.Name.Length >= minNameLength;
        return isValid;
    }

    private bool IsDateValid()
    {
        bool isValid = false;
        isValid = newPlayerData.DateOfBirth != null && 
            newPlayerData.DateOfBirth != DateTime.MinValue;
        return isValid;
    }

    private bool IsGenderValid()
    {
        bool isValid = false;
        isValid = newPlayerData.Gender != Gender.None;
        return isValid;
    }

    private bool IsParametersValid()
    {
        bool isValid = false;
        isValid = newPlayerData.Height > 0 && newPlayerData.Weight > 0;
        return isValid;
    }

    private void SetContinueButtonInteractable(bool isInteractable)
    {
        continueButton.interactable = isInteractable;
        continueButtonTextLabel.alpha = isInteractable ? 1 : 0.5f;
    }

    private void UpdateContinueButtonStatus()
    {
        Debug.Log("currentRegistrationStateIndex = " + currentRegistrationStateIndex);
        switch (currentRegistrationStateIndex)
        {
            case 0:
                SetContinueButtonInteractable(IsNameValid() && IsDateValid());
                break;
            case 1:
                SetContinueButtonInteractable(IsGenderValid());
                break;
            case 2:
                SetContinueButtonInteractable(IsParametersValid());
                break;
            case 3:
                SetContinueButtonInteractable(true);
                break;
        }
    }

    private void SwitchRegistrationState(bool goForward)
    {
        stateContainers[currentRegistrationStateIndex].SetActive(false);
        currentRegistrationStateIndex += goForward ? 1 : -1;

        if (currentRegistrationStateIndex < 0)
        {
            currentRegistrationStateIndex = 0;
        }
        else if (currentRegistrationStateIndex >= stateContainers.Count)
        {
            currentRegistrationStateIndex = stateContainers.Count - 1;
            CompleteRegistration();
        }

        if (currentRegistrationStateIndex == stateContainers.Count - 1)
        {
            string localizedGreeting = LocalizationController.GetLocalizedString(kTableKey, "Hi Label") + $" {newPlayerData.Name}!";
            greetingLabel.text = localizedGreeting;
        }

        stateContainers[currentRegistrationStateIndex].SetActive(true);
        backwardButton.gameObject.SetActive(currentRegistrationStateIndex > 0);
        continueButtonTextLabel.text = LocalizationController.GetLocalizedString(kTableKey,
                currentRegistrationStateIndex < stateContainers.Count - 1 ? kContinueKey : kGoKey);
        titleLebel.text = LocalizationController.GetLocalizedString(kTableKey,
                currentRegistrationStateIndex < stateContainers.Count - 1 ? kTellUsKey : kWelcomeKey);
        UpdateContinueButtonStatus();
    }

    private void SetName(string name)
    {
        OnNameSet?.Invoke(name);
    }

    private void OpenDatePickerPanel()
    {
        datePicker.gameObject.SetActive(true);
    }

    private void SetDateOfBirth(DateTime date)
    {
        OnDateOfBirthSet?.Invoke(date);
        dateOfBirthLabel.text = date.ToString(format: "dd/MM/yy");
    }

    private void SelectGender(Gender gender)
    {
        OnGenderSelect?.Invoke(gender);

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
    }

    private void SelectMeasurementSystem(Toggle toggle)
    {
        if (toggle == metricSystemToggle)
        {
            SelectMeasurementSystem(MeasurementSystem.Metric);
        }
        else
        {
            SelectMeasurementSystem(MeasurementSystem.Imperial);
        }
    }

    private void SelectMeasurementSystem(MeasurementSystem system)
    {
        if (currentMeasurementSystem != system)
        {
            OnMeasurementSystemSelect?.Invoke(system);
            currentMeasurementSystem = system;

            heightUnitLabel.text = LocalizationController.GetLocalizedString(kTableKey,
                isImperialSystem ? kInKey : kCmKey);
            weightUnitLabel.text = LocalizationController.GetLocalizedString(kTableKey,
                isImperialSystem ? kLbKey : kKgKey);

            heightInputField.text = ConvertHeight();
            weightInputField.text = ConvertWeight();
        }

        string ConvertHeight()
        {
            var factor = 2.54f;
            var height = isImperialSystem ? 
                (int)(currentHeight / factor + 0.5f) : (int)(currentHeight * factor + 0.5f);
            SetHeight(height);
            return height.ToString();
        }

        string ConvertWeight()
        {
            var factor = 2.20462f;
            var weight = isImperialSystem ?
                (int)(currentWeight * factor + 0.5f) : (int)(currentWeight / factor + 0.5f);
            SetWeight(weight);
            return weight.ToString();
        }
    }

    private void SetHeight(string height)
    {
        int result;
        bool success = int.TryParse(height, out result);
        if (success)
        {
            SetHeight(result);
        }
    }

    private void SetWeight(string weight)
    {
        int result;
        bool success = int.TryParse(weight, out result);
        if (success)
        {
            SetWeight(result);
        }
    }

    private void SetHeight(int height)
    {
        currentHeight = height;
        OnHeightSet?.Invoke(currentHeight);
    }

    private void SetWeight(int weight)
    {
        currentWeight = weight;
        OnWeightSet?.Invoke(currentWeight);
    }

    private void StartRegistration()
    {
        newPlayerData = new UserProfileData();
        currentRegistrationStateIndex = 0;
        foreach (var state in stateContainers)
        {
            state.SetActive(false);
        }
        stateContainers[currentRegistrationStateIndex].SetActive(true);

        nameInputField.text = String.Empty;
        dateOfBirthLabel.text = "DD/MM/YY";
        heightInputField.text = String.Empty;
        weightInputField.text = String.Empty;
        boyGenderButton.State = SelectableState.Default;
        girlGenderButton.State = SelectableState.Default;
    }

    private void CompleteRegistration()
    {
        OnRegistrationCompleted?.Invoke(newPlayerData);
        ClosePanel();
    }

    private void OpenPanel()
    {
        viewCanvasGroup.alpha = 0;
        StartRegistration();
        viewCanvasGroup.DOFade(1, 0.5f);
    }

    private void ClosePanel()
    {
        viewCanvasGroup.DOFade(0, 0.5f).OnComplete(() => viewCanvas.gameObject.SetActive(false));
    }
}

public enum Gender
{
    None,
    Boy,
    Girl
}

public enum MeasurementSystem
{
    Metric,
    Imperial
}
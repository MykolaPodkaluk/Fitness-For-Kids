using System;
using UnityEngine.Events;
using FitnessForKids.Data;

public interface INewProfileScreenModel
{
    event UnityAction<string> OnNameSet;
    event UnityAction<DateTime> OnDateOfBirthSet;
    event UnityAction<Gender> OnGenderSelect;
    event UnityAction OnMeasurementSystemSelected;
    event UnityAction<int> OnHeightSet;
    event UnityAction<int> OnWeightSet;
    event UnityAction<UserProfileData> OnRegistrationCompleted;
    event UnityAction<bool> OnContinueButtonStatusChanged;
    void Init();
}

public class NewProfileScreenViewModel : INewProfileScreenModel
{
    private const int MinNameLength = 3;
    private UserProfileData newUserProfileData = new UserProfileData();
    private int currentHeight => newUserProfileData.Height;
    private int currentWeight => newUserProfileData.Weight;
    private MeasurementSystem currentMeasurementSystem => newUserProfileData.MeasurementSystem;
    private bool isImperialSystem
    {
        get => currentMeasurementSystem == MeasurementSystem.Imperial;
    }

    //LOCALIZATION:
    private const string TableKey = "Player Registration";
    private const string KgKey = "Kilogram";
    private const string LbKey = "Pound";
    private const string CmKey = "Centimetre";
    private const string InKey = "Inch";
    private const string TellUsKey = "Tell Us";
    private const string WelcomeKey = "Welcome";
    private const string ContinueKey = "Continue";
    private const string GoKey = "Go";

    //EVENTS:
    public event UnityAction<string> OnNameSet;
    public event UnityAction<DateTime> OnDateOfBirthSet;
    public event UnityAction<Gender> OnGenderSelect;
    public event UnityAction OnMeasurementSystemSelected;
    public event UnityAction<int> OnHeightSet;
    public event UnityAction<int> OnWeightSet;
    public event UnityAction<UserProfileData> OnRegistrationCompleted;
    public event UnityAction<bool> OnContinueButtonStatusChanged;

    public NewProfileScreenViewModel()
    {
        Init();
    }

    public string LocalizedHeight
    {
        get => LocalizationController.GetLocalizedString(TableKey, isImperialSystem ? InKey : CmKey);
    }
    public string LocalizedWeight
    {
        get => LocalizationController.GetLocalizedString(TableKey, isImperialSystem ? LbKey : KgKey);
    }
    public string LocalizedGreeting
    {
        get => LocalizationController.GetLocalizedString(TableKey, "Hi Label") + $" {newUserProfileData.Name}!";
    }
    public string LocalizedContinueButtonText(int stateCount)
    {
        return LocalizationController.GetLocalizedString(TableKey,
                CurrentRegistrationStateIndex < stateCount ? ContinueKey : GoKey);
    }
    public string LocalizedTitleLebel(int stateCount)
    {
        return LocalizationController.GetLocalizedString(TableKey, 
            CurrentRegistrationStateIndex < stateCount ? TellUsKey : WelcomeKey);
    }

    public int CurrentRegistrationStateIndex { get; set; }
    public string GreetingLabel => LocalizationController.GetLocalizedString("Player Registration", "Hi Label") + $" {newUserProfileData.Name}!";
    public bool IsImperialSystem => newUserProfileData.MeasurementSystem == MeasurementSystem.Imperial;

    public void Init()
    {
        newUserProfileData = new UserProfileData();
        newUserProfileData.TrainingSettings = new TrainingSettingsData(true);
        CurrentRegistrationStateIndex = 0;
    }

    public bool IsNameValid() => !string.IsNullOrEmpty(newUserProfileData.Name) && newUserProfileData.Name.Length >= MinNameLength;

    public bool IsDateValid() => newUserProfileData.DateOfBirth != DateTime.MinValue;

    public bool IsGenderValid() => newUserProfileData.Gender != Gender.None;

    public bool IsParametersValid() => newUserProfileData.Height > 0 && newUserProfileData.Weight > 0;

    public void SetName(string name)
    {
        newUserProfileData.Name = name;
        OnNameSet?.Invoke(name);
        ValidateContinueButtonStatus();
    }

    public void SetDateOfBirth(DateTime date)
    {
        newUserProfileData.DateOfBirth = date;
        OnDateOfBirthSet?.Invoke(date);
        ValidateContinueButtonStatus();
    }

    public void SetGender(Gender gender)
    {
        newUserProfileData.Gender = gender;
        OnGenderSelect?.Invoke(gender);
        ValidateContinueButtonStatus();
    }

    public void SetMeasurementSystem(MeasurementSystem system)
    {
        if (currentMeasurementSystem != system)
        {
            newUserProfileData.MeasurementSystem = system;
            OnMeasurementSystemSelected?.Invoke();
        }
        ValidateContinueButtonStatus();
    }

    public void SetHeight(int height)
    {
        newUserProfileData.Height = height;
        OnHeightSet?.Invoke(height);
        ValidateContinueButtonStatus();
    }

    public void SetWeight(int weight)
    {
        newUserProfileData.Weight = weight;
        OnWeightSet?.Invoke(weight);
        ValidateContinueButtonStatus();
    }

    public string ConvertHeight()
    {
        var factor = 2.54f;
        var height = isImperialSystem ?
            (int)(currentHeight / factor + 0.5f) : (int)(currentHeight * factor + 0.5f);
        SetHeight(height);
        return height.ToString();
    }

    public string ConvertWeight()
    {
        var factor = 2.20462f;
        var weight = isImperialSystem ?
            (int)(currentWeight * factor + 0.5f) : (int)(currentWeight / factor + 0.5f);
        SetWeight(weight);
        return weight.ToString();
    }

    public void ValidateContinueButtonStatus()
    {
        bool isInteractable = false;

        switch (CurrentRegistrationStateIndex)
        {
            case 0: // Name and Date of Birth state
                isInteractable = IsNameValid() && IsDateValid();
                break;
            case 1: // Gender state
                isInteractable = IsGenderValid();
                break;
            case 2: // Parameters state
                isInteractable = IsParametersValid();
                break;
            case 3: // Welcome state
                isInteractable = true;
                break;
        }

        OnContinueButtonStatusChanged?.Invoke(isInteractable);
    }

    public void CompleteRegistration()
    {
        OnRegistrationCompleted?.Invoke(newUserProfileData);
    }
}
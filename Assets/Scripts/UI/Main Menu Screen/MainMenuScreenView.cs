 using System.Collections.Generic;
using FitnessForKids.Services;
using System.Collections;
using UnityEngine.UI;
using FitnessForKids.UI.Helpers;
using UnityEngine;
using DG.Tweening;
using Zenject;
using System;
using TMPro; 
using UnityEngine.Localization.Settings;

namespace FitnessForKids.UI
{
    public interface IMainMenuScreenView : IView
    {
        void Initialize(
            Func<bool> addProfileValidation,
            Action openProfileRegistration);
        void ShowMaxProfilesMessage();
        void SetPlayerProfileText(string name, int age);
        public Action OnShow { get; set; }
        void UpdateProgramPanel();
        GameObject GameObject { get; }
    }

    public class MainMenuScreenView : MonoBehaviour, IMainMenuScreenView
    {
        #region FIELDS

        [Header("BASIC COMPONENTS:")]
        [SerializeField] private TMP_Text playerProfileLebel;
        [SerializeField] private ConditionalButton addNewUserButton;
        [SerializeField] private Button profilesPanelButton;
        [SerializeField] private List<Button> profileButtons;
        [SerializeField] private List<Button> removeProfileButtons;
        [SerializeField] private List<TMP_Text> profileLabels;
        [SerializeField] private CanvasGroup message;
        [SerializeField] private CanvasGroup profilesPanel;
        [SerializeField] private CanvasGroup panelCanvasGroup;
        [SerializeField] private TrainingProgramsPanel _trainingProgramsPanel;
        [SerializeField] private LanguageSettingsPanel _languageSettingsPanel;
        [SerializeField] private Button _settingsButton;

        [SerializeField] private CanvasGroup removeModalWindow;
        [SerializeField] private TMP_Text removeMessageLabel;
        [SerializeField] private Button removeButton;
        [SerializeField] private Button cancelButton;

        private const string LocalizationTable = "PopupStrings";

        [Inject] private IDataService dataService;
        [Inject] private ITrainingService trainingService;

        public GameObject GameObject => gameObject;
        public Action OnShow { get; set; }

        #endregion

        private void Start()
        {
            Application.targetFrameRate = 60;

            _settingsButton.onClick.AddListener(ShowLanguagePanel);
            profilesPanelButton.onClick.AddListener(ShowProfilesPanel);
            for (int i = 0; i < profileButtons.Count; i++)
            {
                var index = i;
                profileButtons[i].onClick.AddListener(() => SelectProfile(index));
            }
        }

        public void Initialize(Func<bool>addProfileValidation, Action openProfileRegistration)
        {
            addNewUserButton.Initialize(addProfileValidation, openProfileRegistration, ShowMaxProfilesMessage);
            //_trainingProgramsPanel.Initialize();
        }

        //Here initialize avatar 
        private void SelectProfile(int index)
        {
            dataService.UserProfileController.SelectProfile(index);
            var currentProfile = dataService.UserProfileController.Profiles[index];
            CloseProfilePanel();
        }

        public void UpdateProgramPanel()
        {
            _trainingProgramsPanel.Initialize(dataService.UserProfileController.ActiveProgramSettings);
        }

        private void ShowProfilesPanel()
        {
            profilesPanel.gameObject.SetActive(true);
            profilesPanel.alpha = 0;
            profilesPanel.DOFade(1, 0.5f);

            int profilesCount = dataService.UserProfileController.Profiles.Count;

            for (int i = 0; i < profileButtons.Count; i++)
            {
                profileButtons[i].gameObject.SetActive(i < profilesCount);
            }
            for (int i = 0; i < profilesCount; i++)
            {
                var index = i;
                var profileName = dataService.UserProfileController.Profiles[i].Name;
                profileLabels[i].text = profileName;
                removeProfileButtons[i].onClick.AddListener(() => ShowRemoveModalWindow(index));
            }
        }

        private void CloseProfilePanel()
        {
            profilesPanel.DOFade(0, 0.5f).OnComplete(() => profilesPanel.gameObject.SetActive(false));
            foreach (var removeButton in removeProfileButtons)
            {
                removeButton.onClick.RemoveAllListeners();
            }
        }

        public void SetPlayerProfileText(string name, int age)
        {
            var profileText = string.Format(LocalizationSettings.StringDatabase.GetLocalizedString(LocalizationTable, "profile_text"), name, age);
            playerProfileLebel.text = profileText;
        }

        private void ShowRemoveModalWindow(int index)
        {
            var profileName = dataService.UserProfileController.Profiles[index].Name;
            removeMessageLabel.text = string.Format(LocalizationSettings.StringDatabase.GetLocalizedString(LocalizationTable, "remove_message"), profileName);

            removeButton.onClick.AddListener(() => RemoveProfile(index));
            cancelButton.onClick.AddListener(CloseRemoveModalWindow);

            removeModalWindow.gameObject.SetActive(true);
            removeModalWindow.alpha = 0;
            removeModalWindow.DOFade(1, 0.5f);
        }

        private void CloseRemoveModalWindow()
        {
            removeModalWindow.DOFade(0, 0.5f).OnComplete(() =>
            {
                removeButton.onClick.RemoveAllListeners();
                removeModalWindow.gameObject.SetActive(false);
            });
        }

        private void RemoveProfile(int index)
        {
            dataService.UserProfileController.RemoveProfile(index);
            CloseRemoveModalWindow();
            CloseProfilePanel();
        }

        public void ShowMaxProfilesMessage()
        {
            StartCoroutine(ShowMessage());
        }

        private IEnumerator ShowMessage()
        {
            message.gameObject.SetActive(true);
            message.alpha = 0;
            message.DOFade(1, 0.5f);
            yield return new WaitForSeconds(2);
            message.DOFade(0, 0.5f).OnComplete(() => message.gameObject.SetActive(false));
        }

        private void ShowLanguagePanel()
        {
            _languageSettingsPanel.Show();
        }

        public void Show(Action onShow)
        {
            panelCanvasGroup.gameObject.SetActive(true);
            panelCanvasGroup.DOFade(1, 0.5f);
            onShow?.Invoke();
        }

        public void Hide(Action onHide)
        {
            _trainingProgramsPanel.HideLineSelector();
            panelCanvasGroup.DOFade(0, 0.5f).OnComplete(() => panelCanvasGroup.gameObject.SetActive(false));
            onHide?.Invoke();
        }

        public void Release()
        {
            Destroy(panelCanvasGroup.gameObject);
        }
    }
}
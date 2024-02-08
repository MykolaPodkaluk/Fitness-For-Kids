using FitnessForKids.Services;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ProgramsScreenView : MonoBehaviour
{
    [Header("BASIC COMPONENTS:")]
    [SerializeField] private Button closeButton;
    [SerializeField] private Button backwardButton;
    [SerializeField] private TMP_Text programDescriptionLebel;
    [SerializeField] private CanvasGroup descriptionPopUp;
    [SerializeField] private CanvasGroup panelCanvasGroup;

    [SerializeField] private List<string> programDescriptionKeys;
    private const string kTableKey = "Sports Programs";

    private void OnEnable()
    {
        OpenPanel();
    }

    public void ShowProgramDescription(int programIndex)
    {
        string localizedDescription = LocalizationController.GetLocalizedString(kTableKey, programDescriptionKeys[programIndex]);
        programDescriptionLebel.text = localizedDescription;

        descriptionPopUp.gameObject.SetActive(true);
        descriptionPopUp.alpha = 0;
        descriptionPopUp.DOFade(1, 0.5f);

        backwardButton.onClick.RemoveAllListeners();
        backwardButton.onClick.AddListener(CloseProgramDescription);
    }

    public void CloseProgramDescription()
    {
        descriptionPopUp.DOFade(0, 0.5f).OnComplete(() => descriptionPopUp.gameObject.SetActive(false));

        backwardButton.onClick.RemoveAllListeners();
        backwardButton.onClick.AddListener(ClosePanel);
    }

    public void OpenPanel()
    {
        panelCanvasGroup.alpha = 0;
        panelCanvasGroup.DOFade(1, 0.5f);

        closeButton.onClick.AddListener(ClosePanel);
        backwardButton.onClick.AddListener(ClosePanel);
    }

    public void ClosePanel()
    {
        closeButton.onClick.RemoveAllListeners();
        backwardButton.onClick.RemoveAllListeners();

        panelCanvasGroup.DOFade(0, 0.5f).OnComplete(() => 
        {
            descriptionPopUp.gameObject.SetActive(false);
            panelCanvasGroup.gameObject.SetActive(false);
        });
    }
}

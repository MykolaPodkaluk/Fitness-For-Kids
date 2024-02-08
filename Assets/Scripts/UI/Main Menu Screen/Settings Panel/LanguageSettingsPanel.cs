using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class LanguageSettingsPanel : MonoBehaviour
{
    [SerializeField] private CanvasGroup _panel;

    public void Show()
    {
        _panel.gameObject.SetActive(true);
        _panel.alpha = 0;
        _panel.DOFade(1, 0.5f);
    }

    public void Hide()
    {
        _panel.DOFade(0, 0.5f).OnComplete(() => _panel.gameObject.SetActive(false));
    }
}

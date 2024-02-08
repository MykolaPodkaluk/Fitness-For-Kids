using UnityEngine.UI;
using UnityEngine;

namespace FitnessForKids.UI.Helpers
{
    public class LanguageButton : MonoBehaviour
    {
        [SerializeField] private LanguageSettingsPanel _panel;
        [SerializeField] private Button _button;
        [SerializeField] private Language _language;

        private void Start()
        {
            _button.onClick.AddListener(OnButtonClick);
        }

        private void OnDestroy()
        {
            _button.onClick.RemoveListener(OnButtonClick);
        }

        private void OnButtonClick()
        {
            string localCode;

            switch (_language)
            {
                case Language.English:
                    localCode = "en";
                    break;
                case Language.Ukrainian:
                    localCode = "uk";
                    break;
                default:
                    goto case Language.English;
            }

            LocalizationController.SetLanguage(localCode);
            _panel.Hide();
        }
    }
}

public enum Language
{
    English = 0,
    Ukrainian = 1,
}
using UnityEngine.Localization.Settings;
using UnityEngine.Localization;
using UnityEngine.Events;
using UnityEngine;
using System.Linq;

public static class LocalizationController
{
    #region FIELDS

    public static UnityEvent OnLanguageChanged = new UnityEvent();

    #endregion

    #region SET LANGUAGE

    public static void SetLanguage(Locale locale)
    {
        LocalizationSettings.SelectedLocale = locale;
        OnLanguageChanged?.Invoke();

        //GameSettingsManager.Instance.SaveLanguageSettings(locale.Identifier.Code);
    }

    public static void SetLanguage(string localeCode)
    {
        SetLanguage(GetLocale(localeCode));
    }

    #endregion

    #region GETTERS

    public static Locale GetLocale(string localeCode) => LocalizationSettings.AvailableLocales.Locales
        .First(locale => locale.Identifier.Code == localeCode);

    public static string GetLocalizedString(string table, string key)
    {
        string localizedString = LocalizationSettings.StringDatabase.GetLocalizedString(table, key);
        return localizedString;
    }

    public static Sprite GetLocalizedSprite(string table, string key)
    {
        Sprite localizedTexture = LocalizationSettings.AssetDatabase.GetLocalizedAsset<Sprite>(table, key, null);
        return localizedTexture;
    }

    #endregion
}

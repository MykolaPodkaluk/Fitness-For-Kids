using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public class ScreenOrientationHandlerEditor
{
    static ScreenOrientationHandlerEditor()
    {
        EditorApplication.update += CheckOrientation;
    }

    private static void CheckOrientation()
    {
        if (Application.isPlaying)
            return;

        if (Screen.orientation == ScreenOrientation.Portrait)
        {
            // Викликати код для вертикальної орієнтації екрану в режимі редагування
            // наприклад, змінювати значення Anchors для панелі
        }
        else if (Screen.orientation == ScreenOrientation.LandscapeLeft || Screen.orientation == ScreenOrientation.LandscapeRight)
        {
            // Викликати код для горизонтальної орієнтації екрану в режимі редагування
            // наприклад, змінювати значення Anchors для панелі
        }
    }
}

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
            // ��������� ��� ��� ����������� �������� ������ � ����� �����������
            // ���������, �������� �������� Anchors ��� �����
        }
        else if (Screen.orientation == ScreenOrientation.LandscapeLeft || Screen.orientation == ScreenOrientation.LandscapeRight)
        {
            // ��������� ��� ��� ������������� �������� ������ � ����� �����������
            // ���������, �������� �������� Anchors ��� �����
        }
    }
}

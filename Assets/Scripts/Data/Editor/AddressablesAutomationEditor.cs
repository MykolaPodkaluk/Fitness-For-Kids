#if UNITY_EDITOR

using UnityEditor.AddressableAssets.Settings.GroupSchemas;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets;
using UnityEditor;

public class AddressablesAutomationEditor
{
    private const string kGroupName = "ExerciseData";
    private const string kLabelToSet = "ExerciseData";

    [MenuItem("Tools/Automation/Set Label for All Assets in Group")]
    public static void SetLabelForAllAssetsInGroup()
    {
        AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;

        // Get or create group "ExerciseData"
        AddressableAssetGroup group = settings.FindGroup(kGroupName);
        if (group == null)
        {
            group = settings.CreateGroup(kGroupName, false, false, true, null, typeof(BundledAssetGroupSchema));
        }

        // Set the Label for all assets in the group
        foreach (AddressableAssetEntry entry in group.entries)
        {
            entry.labels.Add(kLabelToSet);
        }

        // Save changes to Addressables settings
        EditorUtility.SetDirty(settings);
        AssetDatabase.SaveAssets();
    }
}

#endif
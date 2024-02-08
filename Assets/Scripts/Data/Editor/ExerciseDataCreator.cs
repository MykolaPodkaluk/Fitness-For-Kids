#if UNITY_EDITOR
using BodyPart = FitnessForKids.Data.BodyPart;
using System.Collections.Generic;
using FitnessForKids.Data;
using UnityEditor;
using UnityEngine;

namespace FitnessForKids.Helpers
{
    public class ExerciseDataCreator
    {
        static string idDefault = "Exercise_001";
        static string nameDefault = "Enter the exercise name";
        static string descriptionDefault = "Enter the exercise description";
        static string workingPartsDefault = "Quadriceps, Hamstrings, Glutes";
        static int minAgeDefault = 18;
        static int minRepsDefault = 8;
        static int maxRepsDefault = 10;
        static List<Skill> skillsDefault = new List<Skill> { Skill.Power, Skill.Flexibility };

        [MenuItem("Assets/Create/ScriptableObjects/New ExerciseData")]
        public static void CreateExerciseData()
        {
            ExerciseData exerciseData = new ExerciseData(
                idDefault,
                nameDefault,
                descriptionDefault,
                BodyPart.Legs,
                workingPartsDefault,
                Difficulty.Medium,
                minAgeDefault,
                minRepsDefault,
                maxRepsDefault,
                false,
                skillsDefault
            );

            string path = AssetDatabase.GetAssetPath(Selection.activeObject);
            if (string.IsNullOrEmpty(path))
            {
                path = "Assets";
            }
            else if (!string.IsNullOrEmpty(System.IO.Path.GetExtension(path)))
            {
                path = path.Replace(System.IO.Path.GetFileName(AssetDatabase.GetAssetPath(Selection.activeObject)), "");
            }

            string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(path + "/ExerciseData.asset");
            AssetDatabase.CreateAsset(exerciseData, assetPathAndName);
            AssetDatabase.SaveAssets();

            EditorUtility.FocusProjectWindow();
            Selection.activeObject = exerciseData;

            Debug.Log("ExerciseData created at path: " + assetPathAndName);
        }
    }
}
#endif
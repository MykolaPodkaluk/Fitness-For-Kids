#if UNITY_EDITOR
using BodyPart = FitnessForKids.Data.BodyPart;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using FitnessForKids.Data;
using UnityEditor;
using UnityEngine;
using System;

namespace FitnessForKids.Helpers
{
    public static class ExerciseDatasGenerator
    {
        private const string csvFilePath = "Assets/Data/CSV/Exercises - Вправи.csv";
        private const string outputPath = "Assets/Data/GenerationOutput/";
        private static readonly int[] rowsToSkip = new int[] { 0, 1, 15, 79, 88, 101, 117, 123, 224, 293, 395, 557 };
        //private static readonly int[] rowsToSkip = new int[] { 0, 1, 11, 17 };

        [MenuItem("Tools/Automation/Exercise Datas from CSV")]
        public static void Generate()
        {
            List<string[]> csvLines = CSVReader.ReadCSVFile(csvFilePath);

            RemoveSkippedRows(csvLines, rowsToSkip);

            for (int i = 0; i < csvLines.Count; i++)
            {
                string[] line = csvLines[i];

                ExerciseData exerciseData = CreateExerciseData(line);

                string assetPath = AssetDatabase.GenerateUniqueAssetPath(outputPath + exerciseData.ID + ".asset");
                AssetDatabase.CreateAsset(exerciseData, assetPath);

                Debug.Log("ExerciseData created at path: " + assetPath);
            }
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private static void RemoveSkippedRows(List<string[]> csvLines, int[] rowsToSkip)
        {
            for (int i = rowsToSkip.Length - 1; i >= 0; i--)
            {
                int rowIndex = rowsToSkip[i];
                if (rowIndex >= 0 && rowIndex < csvLines.Count)
                {
                    csvLines.RemoveAt(rowIndex);
                }
            }
        }

        private static BodyPart ParseBodyPart(string id)
        {
            foreach (BodyPart bodyPart in Enum.GetValues(typeof(BodyPart)))
            {
                if (id.Contains(bodyPart.ToString()))
                {
                    return bodyPart;
                }
            }
            return BodyPart.FullBody;
        }

        private static int ParseMinAge(string minAgeString)
        {
            string numericString = Regex.Match(minAgeString, @"\d+").Value;
            if (int.TryParse(numericString, out int minAge))
            {
                return minAge;
            }
            return 0;
        }

        private static (int minReps, int maxReps) ParseReps(string repsString)
        {
            string numericPattern = @"\d+";
            MatchCollection matches = Regex.Matches(repsString, numericPattern);

            if (matches.Count >= 2)
            {
                int.TryParse(matches[0].Value, out int minReps);
                int.TryParse(matches[1].Value, out int maxReps);
                return (minReps, maxReps);
            }
            else if (matches.Count == 1)
            {
                int.TryParse(matches[0].Value, out int reps);
                return (reps, reps);
            }

            return (0, 0);
        }

        private static List<Skill> ParseSkills(string powerString, string flexibilityString, string speedString,
            string enduranceString, string agilityString)
        {
            var skills = new List<Skill>();
            if (!string.IsNullOrEmpty(powerString))
                skills.Add(Skill.Power);
            if (!string.IsNullOrEmpty(flexibilityString))
                skills.Add(Skill.Flexibility);
            if (!string.IsNullOrEmpty(speedString))
                skills.Add(Skill.Speed);
            if (!string.IsNullOrEmpty(enduranceString))
                skills.Add(Skill.Endurance);
            if (!string.IsNullOrEmpty(agilityString))
                skills.Add(Skill.Agility);
            return skills;
        }

        private static Difficulty ParseDifficulty(string difficultyString)
        {
            if (!Enum.TryParse(difficultyString, out Difficulty difficulty))
            {
                Debug.LogWarning("Failed to parse Difficulty value: " + difficultyString);
                difficulty = Difficulty.Medium;
            }
            return difficulty;
        }

        private static ExerciseData CreateExerciseData(string[] line)
        {
            string id = line[2];
            string name = line[0];
            string description = line[5];
            BodyPart bodyPart = ParseBodyPart(id);
            int minAge = ParseMinAge(line[3]);
            string workingParts = line[1];
            Difficulty difficulty = ParseDifficulty(line[14]);
            (int minReps, int maxReps) = ParseReps(line[6]);
            bool isWarmUp = !string.IsNullOrEmpty(line[8]);
            List<Skill> skills = ParseSkills(line[9], line[10], line[11], line[12], line[13]);

            ExerciseData exerciseData = new ExerciseData(id, name, description, bodyPart, workingParts, difficulty, minAge, minReps, maxReps, isWarmUp, skills);

            return exerciseData;
        }
    }
}

#endif
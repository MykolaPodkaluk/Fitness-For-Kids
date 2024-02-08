using System.Collections.Generic;
using UnityEngine;
using System;

namespace FitnessForKids.Data
{
    [CreateAssetMenu(fileName = "New ExerciseData", menuName = "ScriptableObjects/Empty ExerciseData")]
    [Serializable]
    public class ExerciseData : ScriptableObject
    {
        public string ID;
        public string Name;
        public string Description;
        public BodyPart BodyPart;
        public string WorkingParts;
        public Difficulty Difficulty;
        public int MinAge;
        public int MinReps;
        public int MaxReps;
        public bool IsWarmUp;
        public List<Skill> Skills;

        public string Identifier => ID;
        public List<Skill> ActiveSkills => Skills;

        public ExerciseData(string id, string name, string description, BodyPart bodyPart, string workingParts,
                            Difficulty difficulty, int minAge, int minReps, int maxReps, bool isWarmUp, List<Skill> skills)
        {
            ID = id;
            Name = name;
            Description = description;
            BodyPart = bodyPart;
            WorkingParts = workingParts;
            Difficulty = difficulty;
            MinAge = minAge;
            MinReps = minReps;
            MaxReps = maxReps;
            IsWarmUp = isWarmUp;
            Skills = skills;
        }
    }

    [Serializable]
    public class ExerciseSaveData
    {
        public string ID;
        public BodyPart BodyPart;
        public string WorkingParts;
        public Difficulty Difficulty;
        public List<Skill> Skills;

        public string Identifier => ID;
        public List<Skill> ActiveSkills => Skills;

        public ExerciseSaveData(string id, BodyPart bodyPart, string workingParts,
                            Difficulty difficulty, List<Skill> skills)
        {
            ID = id;
            BodyPart = bodyPart;
            WorkingParts = workingParts;
            Difficulty = difficulty;
            Skills = skills;
        }
    }

    public enum BodyPart
    {
        None,
        Neck,
        Shoulders,
        Elbows,
        Biceps,
        Triceps,
        Hands,
        Back,
        Core,
        Glutes,
        Legs,
        FullBody
    }

    public enum WorkingBodyPart
    {
        None,
    }

    public enum Skill
    {
        None,
        Power,
        Flexibility,
        Speed,
        Endurance,
        Agility
    }

    public enum Difficulty
    {
        Easy,
        Medium,
        Hard
    }
}
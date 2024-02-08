using System;
using System.Collections.Generic;

namespace FitnessForKids.Data
{
    public interface IUserProfileData
    {
        string Name { get; set; }
        DateTime DateOfBirth { get; set; }
        int Age { get; }
        MeasurementSystem MeasurementSystem { get; set; }
        Gender Gender { get; set; }
        int Height { get; set; }
        int Weight { get; set; }
        TrainingStatisticsData Statistics { get; set; }
        InAppPurchasingData InAppPurchasingData { get; set; }
    }

    [Serializable]
    public class UserProfileData : IUserProfileData
    {
        public string Name { get; set; }
        public DateTime DateOfBirth { get; set; }
        public int Age
        {
            get
            {
                int age = DateTime.Today.Year - DateOfBirth.Year;

                if (DateTime.Today.Month < DateOfBirth.Month ||
                    (DateTime.Today.Month == DateOfBirth.Month && DateTime.Today.Day < DateOfBirth.Day))
                {
                    age--;
                }

                return age;
            }
        }
        public Gender Gender { get; set; }
        public MeasurementSystem MeasurementSystem { get; set; }
        public int Height { get; set; }
        public int Weight { get; set; }

        public int Power { get; set; }
        public int Flexibility { get; set; }
        public int Speed { get; set; }
        public int Endurance { get; set; }
        public int Agility { get; set; }
        public TrainingSettingsData TrainingSettings { get; set; } = new TrainingSettingsData();
        public TrainingStatisticsData Statistics { get; set; } = new TrainingStatisticsData();
        public InAppPurchasingData InAppPurchasingData { get; set; } = new InAppPurchasingData();
    }
}
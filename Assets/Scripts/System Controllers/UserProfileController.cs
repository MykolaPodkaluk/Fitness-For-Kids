using System.Collections.Generic;
using FitnessForKids.Training;
using FitnessForKids.Services;
using Newtonsoft.Json;
using System.Linq;
using UnityEngine;
using System.IO;
using System;

namespace FitnessForKids.Data
{
    public interface IUserProfileController
    {
        UserProfileData ActiveProfile { get; }
        List<UserProfileData> Profiles { get; }
        List<TrainingProgramSettings> ActiveProgramSettings { get; }
        bool HasActiveProfile { get; }
        bool SelectProfile(int index);
        bool RemoveProfile(int index);
        bool CanCreateProfile();
        void CreateProfile(UserProfileData newProfile);
        void CheckActiveProfile();
        event Action<UserProfileData> OnProfilesChanged;
        //List<List<int>> GetWeeklyStatistics();
        Dictionary<DateTime, List<int>> GetWeeklyStatistics();
        bool IsSubscribed();
        DateTime GetFreeTrialEndDateOrDefault(DateTime defaultEndDate);
        bool IsAdsRemoved();
        void UpdateSubscription(int days, bool isSubscribed);
        void RemoveAds(bool isRemoved);
        Gender CurrentGender { get; }
    }

    /// <summary>
    /// Manages user profiles and provides methods for creating, selecting, and removing profiles.
    /// </summary>
    public class UserProfileController : IUserProfileController
    {
        #region FIELDS 
        private int activeProfileIndex = -1;
        private List<UserProfileData> profiles = new List<UserProfileData>();
        public int MaxProfiles => kMaxProfiles;
        public int ActiveProfileIndex => activeProfileIndex;
        public bool HasActiveProfile => activeProfileIndex != -1;
        public UserProfileData ActiveProfile => HasActiveProfile ? profiles[activeProfileIndex] : null;
        public List<UserProfileData> Profiles => profiles;
        public event Action<UserProfileData> OnProfilesChanged;
        private string saveFilePath => _dataService.SaveFilePath;
        private const int kMaxProfiles = 3;
        private readonly DataService _dataService;
        public List<TrainingProgramSettings> ActiveProgramSettings => ActiveProfile.TrainingSettings.ProgramSettings;
        public Gender CurrentGender => HasActiveProfile ? profiles[activeProfileIndex].Gender : Gender.None;

        #endregion

        #region INITIALIZATION

        private List<TrainingProgramSettings> DefaultProgramSettings()
        {
            var settings = new List<TrainingProgramSettings>();
            foreach (ProgramType programType in Enum.GetValues(typeof(ProgramType)))
            {
                if (programType != ProgramType.Custom)
                {
                    var programSettings = new TrainingProgramSettings(programType, true);
                    settings.Add(programSettings);
                }
            }
            return settings;
        }

        public UserProfileController(DataService dataService)
        {
            _dataService = dataService;
        }

        public void Init()
        {
            LoadActiveProfileIndex();
            LoadProfiles();
            CheckActiveProfile();
        }

        #endregion

        private void LoadActiveProfileIndex()
        {
            activeProfileIndex = PlayerPrefs.GetInt("activeProfileIndex", -1);
        }

        private void SaveActiveProfileIndex()
        {
            PlayerPrefs.SetInt("activeProfileIndex", activeProfileIndex);
            PlayerPrefs.Save();
        }

        private void LoadProfiles()
        {
            if (File.Exists(saveFilePath))
            {
                var profilesJson = File.ReadAllText(saveFilePath);
                profiles = JsonConvert.DeserializeObject<List<UserProfileData>>(profilesJson);
            }
        }

        private void SaveProfiles()
        {
            var profilesJson = JsonConvert.SerializeObject(profiles);
            File.WriteAllText(saveFilePath, profilesJson);
        }

        public void CheckActiveProfile()
        {
            if (HasActiveProfile && activeProfileIndex >= profiles.Count)
            {
                activeProfileIndex = -1;
                SaveActiveProfileIndex();
            }
            OnProfilesChanged?.Invoke(ActiveProfile); 
        }

        public bool CanCreateProfile()
        {
            return profiles.Count < kMaxProfiles;
        }

        public void CreateProfile(UserProfileData newProfile)
        {
            if (CanCreateProfile())
            {
                profiles.Add(newProfile);
                SaveProfiles();

                if (!HasActiveProfile)
                {
                    activeProfileIndex = 0;
                    SaveActiveProfileIndex();
                }
                else
                {
                    activeProfileIndex++;
                    SaveActiveProfileIndex();
                }
                OnProfilesChanged?.Invoke(ActiveProfile);
            }
            else
            {
                Debug.Log("Can't create a new profile! Maximum amount of profiles has been reached!");
            }
        }

        public bool SelectProfile(int index)
        {
            if (index < 0 || index >= profiles.Count)
            {
                return false;
            }

            activeProfileIndex = index;
            SaveActiveProfileIndex(); 

            OnProfilesChanged?.Invoke(ActiveProfile);
            return true;
        }

        public bool RemoveProfile(int index)
        {
            if (index < 0 || index >= profiles.Count)
            {
                return false;
            }

            if (HasActiveProfile)
            {
                if (profiles.Count > 1)
                {
                    activeProfileIndex = 0;
                }
                else
                {
                    activeProfileIndex = -1;
                }
                SaveActiveProfileIndex();
            }

            profiles.RemoveAt(index);
            SaveProfiles();

            OnProfilesChanged?.Invoke(ActiveProfile);
            return true;
        }

        public void ClearData()
        {
            profiles = new List<UserProfileData>();
            activeProfileIndex = -1;
            SaveProfiles();
            OnProfilesChanged?.Invoke(ActiveProfile);
        }

        public void UpgradeSkills(List<Skill> updatedSkills)
        {
            if(updatedSkills.Contains(Skill.Power))
                ActiveProfile.Power++;
            if (updatedSkills.Contains(Skill.Flexibility))
                ActiveProfile.Flexibility++;
            if (updatedSkills.Contains(Skill.Speed))
                ActiveProfile.Speed++;
            if (updatedSkills.Contains(Skill.Endurance))
                ActiveProfile.Endurance++;
            if (updatedSkills.Contains(Skill.Agility))
                ActiveProfile.Agility++;
            SaveProfiles();
        }

        public void UpdateStatistics(TrainingProgram completedProgram)
        {
            var completedSaveData = new TrainingProgramStatistics(completedProgram.Type, completedProgram.ExerciseSaveDatas);
            DateTime today = DateTime.Today;
            StatisticsDayData todayDayData = ActiveProfile.Statistics.DailyData.FirstOrDefault(stat => stat.Date == today);

            if (todayDayData != null)
            {
                if (todayDayData.CompletedPrograms != null)
                {
                    todayDayData.CompletedPrograms.Add(completedSaveData);
                }
                else
                {
                    todayDayData.CompletedPrograms = new List<TrainingProgramStatistics> { completedSaveData };
                }
            }
            else
            {
                todayDayData = new StatisticsDayData(today, new List<TrainingProgramStatistics> { completedSaveData });
                ActiveProfile.Statistics.DailyData.Add(todayDayData);
            }

            SaveProfiles();
        }

        public void UpdateTrainingSettings(List<TrainingProgramSettings> programSettings)
        {
            ActiveProfile.TrainingSettings.ProgramSettings = programSettings;
            SaveProfiles();
        }

        //public List<List<int>> GetWeeklyStatistics()
        public Dictionary<DateTime, List<int>> GetWeeklyStatistics()
        {
            DateTime today = DateTime.Today;
            DateTime startDate = today.AddDays(-(int)today.DayOfWeek +1); // Start of last week
            DateTime endDate = startDate.AddDays(6); // End of last week

            //List<List<int>> weeklyStatistics = new List<List<int>>();
            Dictionary<DateTime, List<int>> weeklyStatistics = new Dictionary<DateTime, List<int>>();

            for (int i = 0; i < 7; i++)
            {
                DateTime currentDate = startDate.AddDays(i);
                List<TrainingProgramStatistics> programsForDay = GetCompletedProgramsForDay(currentDate);

                int totalProgramsCount = programsForDay.Count;

                List<int> programTypePercentages = new List<int>();

                foreach (ProgramType programType in Enum.GetValues(typeof(ProgramType)))
                {
                    if (programType != ProgramType.Custom)
                    {
                        int count = programsForDay.Count(program => program.Type == programType);
                        int percentage = (int)(count / (double)totalProgramsCount * 100);
                        programTypePercentages.Add(percentage);
                    }
                }

                //weeklyStatistics.Add(programTypePercentages);
                weeklyStatistics.Add(currentDate, programTypePercentages);
            }

            return weeklyStatistics;
        }

        private List<TrainingProgramStatistics> GetCompletedProgramsForDay(DateTime date)
        {
            StatisticsDayData dayData = ActiveProfile.Statistics.DailyData.FirstOrDefault(data => data.Date == date);

            if (dayData != null)
            {
                return dayData.CompletedPrograms;
            }

            return new List<TrainingProgramStatistics>();
        }

        public bool IsSubscribed()
        {
            var subscriptionData = ActiveProfile.InAppPurchasingData;
            var subscriptionDate = subscriptionData.SubscriptionDate;
            var subscriptionDays = subscriptionData.SubscriptionDays;

            if (subscriptionDate == null)
            {
                return false;
            }

            var presentTime = DateTime.UtcNow;
            var expireTime = subscriptionDate.AddDays(subscriptionDays);

            if (presentTime > expireTime)
            {
                SeveSubscriptionDays(0);
                return false;
            }
            else
            {
                return true;
            }
        }

        public void UpdateSubscription(int days, bool isSubscribed)
        {
            var value = isSubscribed ? days : 0;
            SeveSubscriptionDays(value);
        }

        private void SeveSubscriptionDays(int days)
        {
            var subscriptionData = ActiveProfile.InAppPurchasingData;
            subscriptionData.SubscriptionDate = DateTime.UtcNow;
            subscriptionData.SubscriptionDays = days;
            SaveProfiles();
        }

        public DateTime GetFreeTrialEndDateOrDefault(DateTime defaultEndDate)
        {
            var iAPdata = ActiveProfile.InAppPurchasingData;
            var freeTrialEndDate = iAPdata.FreeTrialEndDate;
            if (!freeTrialEndDate.HasValue)
            {
                iAPdata.FreeTrialEndDate = defaultEndDate;
                SaveProfiles();
            }
            freeTrialEndDate = iAPdata.FreeTrialEndDate;
            return (DateTime)freeTrialEndDate;
        }

        public void RemoveAds(bool isRemoved)
        {
            var subscriptionData = ActiveProfile.InAppPurchasingData;
            subscriptionData.IsAdsRemoved = isRemoved;
            SaveProfiles();
        }

        public bool IsAdsRemoved()
        {
            var isAdsRemoved = ActiveProfile.InAppPurchasingData.IsAdsRemoved;
            return isAdsRemoved;
        }
    }
}
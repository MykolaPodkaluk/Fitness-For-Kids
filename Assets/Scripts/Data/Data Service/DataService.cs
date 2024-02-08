using FitnessForKids.Data;
using UnityEngine;
using System.IO;
using System;
using System.Collections.Generic;

namespace FitnessForKids.Services
{
    public class DataService : IDataService
    {
        private const string kSaveFileName = "saveData.json";
        private const string kSaveFolderName = "/Saves/";

        //WARNING! Do not change if not nesassery!
        //using for for DB reset if versions are different 
        private const int kSaveDataControl = 1;

        private UserProfileController _userProfileController;
        //private StatisticsController _statisticsController;
        //private SettingsController _settingsController;
        private string _saveDirectoryPath;
        private string _saveFilePath;
        private bool _isFirstCreation;

        public bool IsInitiated { get; private set; }
        public string SaveFilePath => _saveFilePath;

        public IUserProfileController UserProfileController => _userProfileController;
        //public IStatisticsController StatisticsController => _statisticsController;
        //public ISettingsController SettingsController => _settingsController;

        public event Action ON_RESET;

        public DataService()
        {
            string dataPath = Application.persistentDataPath;
            _saveDirectoryPath = dataPath + kSaveFolderName;
            _saveFilePath = _saveDirectoryPath + kSaveFileName;
            if (!Directory.Exists(_saveDirectoryPath))
            {
                Directory.CreateDirectory(_saveDirectoryPath);
            }

            _userProfileController = new UserProfileController(this);
            //_statisticsController = new StatisticsController(this);
            //_settingsController = new SettingsController(this);
            InitControllers();
        }

        public void ResetProgress()
        {
            _userProfileController.ClearData();
            //_statisticsController.ClearData();
            //_settingsController.ClearData();

            InitControllers();

            ON_RESET?.Invoke();
            Debug.Log("Data has been reset!");
        }

        private void InitControllers()
        {
            _userProfileController.Init();
            //_statisticsController.Init();
            //_settingsController.Init();
            IsInitiated = true;
        }

        public void SaveProgramData(TrainingProgram completedProgram)
        {
            _userProfileController.UpgradeSkills(completedProgram.GetAllSkills());
            _userProfileController.UpdateStatistics(completedProgram);
        }

        public void SaveTrainingSettings(List<TrainingProgramSettings> programSettings)
        {
            _userProfileController.UpdateTrainingSettings(programSettings);
        }
    }
}
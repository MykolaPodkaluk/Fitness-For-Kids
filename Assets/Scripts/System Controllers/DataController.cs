using FitnessForKids.Data.Addressables;
using System.Collections.Generic;
using FitnessForKids.UI.Styles;
using Cysharp.Threading.Tasks;
using FitnessForKids.Services;
using FitnessForKids.Training;
using FitnessForKids.Data;
using UnityEngine;
using System.Linq;
using Zenject;
using System;
using UnityEngine.PlayerLoop;

public class DataController : MonoBehaviour
{
    [Inject] IAddressableRefsHolder _refsHolder;

    private async void Awake()
    { 
        var viewStyles = await _refsHolder.Trainings.Views.GetAllData<TrainingProgramPanelViewStyleData>();
        var colorStyles = await _refsHolder.Trainings.Styles.GetAllData<TrainingProgramColorStyleData>();
        var localizations = await _refsHolder.Trainings.LocalizationKeys.GetAllData<TrainingProgramLocalizationData>();
    }
}

using System.Collections.Generic;
using System;

namespace FitnessForKids.Data
{
    public interface IStatisticsDayData
    {
        DateTime Date { get; set; }
        List<TrainingProgramStatistics> CompletedPrograms { get; set; }
    }

    [Serializable]
    public class StatisticsDayData : IStatisticsDayData
    {
        public DateTime Date { get; set; }
        public List<TrainingProgramStatistics> CompletedPrograms { get; set; }

        public StatisticsDayData(DateTime date, List<TrainingProgramStatistics> completedPrograms)
        {
            this.Date = date;
            this.CompletedPrograms = completedPrograms;
        }
    }

    [Serializable]
    public class TrainingStatisticsData
    {
        public List<StatisticsDayData> DailyData { get; set; }

        public TrainingStatisticsData()
        {
            DailyData = new List<StatisticsDayData>();
        }
    }
}
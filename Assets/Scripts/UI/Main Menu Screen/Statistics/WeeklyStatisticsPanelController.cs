using System;
using System.Collections.Generic;

public interface IWeeklyStatisticsPanelController
{
    IWeeklyStatisticsPanelView View { get; }

    //void ShowWeeklyStatistics(List<List<int>> weeklyStatistics);
    void ShowWeeklyStatistics(Dictionary<DateTime, List<int>> weeklyStatistics);
}

public class WeeklyStatisticsPanelController : IWeeklyStatisticsPanelController
{
    private IWeeklyStatisticsPanelView view;
    public IWeeklyStatisticsPanelView View => view;

    public WeeklyStatisticsPanelController(IWeeklyStatisticsPanelView view)
    {
        this.view = view;
    }

    //public void ShowWeeklyStatistics(List<List<int>> weeklyStatistics)
    public void ShowWeeklyStatistics(Dictionary<DateTime, List<int>> weeklyStatistics)
    {
        view.InitView(weeklyStatistics);
    }
}
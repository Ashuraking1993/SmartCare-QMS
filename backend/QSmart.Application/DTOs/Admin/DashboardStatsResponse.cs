namespace QSmart.Application.DTOs.Admin;

public class DashboardStatsResponse
{
    public int TotalWaiting { get; set; }

    public int TotalServing { get; set; }

    public int TotalCompletedToday { get; set; }

    public int ActiveCounters { get; set; }
}
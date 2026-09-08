namespace QSmart.Application.DTOs.Analytics;

public class PatientInsightsDto
{
    public int TotalPatientsToday { get; set; }

    public int WaitingNow { get; set; }

    public int ServingNow { get; set; }

    public int CompletedToday { get; set; }

    public double AverageWaitMinutes { get; set; }

    public double AverageServiceMinutes { get; set; }

    public string BusiestDepartment { get; set; }
        = string.Empty;

    public string BusiestHour { get; set; }
        = string.Empty;

    public List<DepartmentInsightDto> Departments { get; set; }
        = new();

    public List<HourlyInsightDto> HourlyTrend { get; set; }
        = new();
}

public class DepartmentInsightDto
{
    public Guid ServiceId { get; set; }

    public string ServiceName { get; set; }
        = string.Empty;

    public int PatientCount { get; set; }

    public double AverageWaitMinutes { get; set; }

    public int WaitingNow { get; set; }
}

public class HourlyInsightDto
{
    public int Hour { get; set; }

    public string Label { get; set; }
        = string.Empty;

    public int PatientCount { get; set; }
}
using Microsoft.ML.Data;

namespace QSmart.Infrastructure.MachineLearning;

public class QueueModelInput
{
    [ColumnName("Label")]
    public float ActualWaitMinutes { get; set; }

    public float PeopleAhead { get; set; }
    public float HourOfDay { get; set; }
    public float DayOfWeek { get; set; }
    public float CurrentQueueSize { get; set; }
    public float RecentAverageServiceMinutes { get; set; }
    public float RecentAverageWaitingMinutes { get; set; }

    public float AvailableDoctorCount { get; set; }

    public string BranchId { get; set; } = string.Empty;

    public string ServiceId { get; set; } = string.Empty;
}
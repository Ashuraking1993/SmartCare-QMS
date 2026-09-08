namespace QSmart.Application.DTOs.Prediction;

public class QueuePredictionDto
{
    public string TicketNumber { get; set; } = string.Empty;

    public string ServiceName { get; set; } = string.Empty;

    public int PeopleAhead { get; set; }

    public int EstimatedWaitMinutes { get; set; }

    public double AverageServiceMinutes { get; set; }

    public string QueueLoad { get; set; } = string.Empty;

    public int Confidence { get; set; }

    public string PredictionSource { get; set; } = string.Empty;
}
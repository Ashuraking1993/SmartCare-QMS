using Microsoft.ML.Data;

namespace QSmart.Infrastructure.MachineLearning;

public class QueueModelOutput
{
    [ColumnName("Score")]
    public float PredictedWaitMinutes { get; set; }
}
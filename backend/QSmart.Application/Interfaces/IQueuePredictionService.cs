using QSmart.Application.DTOs.Prediction;
using QSmart.Domain.Entities;

namespace QSmart.Application.Interfaces;

public interface IQueuePredictionService
{
    Task<QueuePredictionDto> PredictAsync(
        QueueTicket ticket);
}
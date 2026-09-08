using QSmart.Application.DTOs.Prediction;
using QSmart.Application.Interfaces;
using QSmart.Domain.Entities;
using QSmart.Infrastructure.MachineLearning;

namespace QSmart.Infrastructure.Services;

public class QueuePredictionService
    : IQueuePredictionService
{
    private readonly IQueueRepository _queueRepository;
    private readonly QueueMLService _queueMLService;

    public QueuePredictionService(
        IQueueRepository queueRepository,
        QueueMLService queueMLService)
    {
        _queueRepository = queueRepository;
        _queueMLService = queueMLService;
    }

    public async Task<QueuePredictionDto> PredictAsync(
        QueueTicket ticket)
    {
        var peopleAhead =
            await _queueRepository
                .GetPeopleAheadAsync(ticket);

        var history =
            await _queueRepository
                .GetCompletedTicketsForPredictionAsync(
                    ticket.BranchId,
                    ticket.ServiceId);

        // Default estimate until enough historical
        // SmartCare data has been collected.
        double averageServiceMinutes = 6;

        var validDurations = history
            .Where(x =>
                x.CalledAt.HasValue &&
                x.CompletedAt.HasValue)
            .Select(x =>
                (x.CompletedAt!.Value -
                 x.CalledAt!.Value)
                .TotalMinutes)
            .Where(x =>
                x >= 1 &&
                x <= 120)
            .ToList();

        if (validDurations.Count > 0)
        {
            averageServiceMinutes =
                validDurations.Average();
        }

        averageServiceMinutes =
            Math.Round(
                averageServiceMinutes,
                1);

        // =====================================================
// SMARTCARE ML PREDICTION
// =====================================================

var allHistory =
    await _queueRepository.GetHistoryAsync();

var doctors =
    await _queueRepository
        .GetDoctorsWithAvailabilityAsync();

var trainingData =
    _queueMLService.BuildTrainingData(
        allHistory,
        doctors);

bool useML =
    trainingData.Count >= 10;

float? mlPrediction = null;

if (useML)
{
    _queueMLService.Train(trainingData);

    var liveQueue =
        await _queueRepository.GetLiveQueueAsync(
            ticket.BranchId,
            ticket.ServiceId);

            
   var hospitalLocalNow =
    DateTime.UtcNow.AddHours(8);

    var availableDoctorCount =
        await _queueRepository
        .GetAvailableDoctorCountAsync(
            ticket.BranchId,
            ticket.ServiceId,
            hospitalLocalNow);       

    var recentAverageWait =
        trainingData
            .Where(x =>
                x.ServiceId ==
                ticket.ServiceId.ToString())
            .TakeLast(20)
            .Select(x =>
                (double)x.ActualWaitMinutes)
            .DefaultIfEmpty(0)
            .Average();

    var input =
        new QueueModelInput
        {
            PeopleAhead =
                peopleAhead,

            HourOfDay =
                hospitalLocalNow.Hour,

            DayOfWeek =
                (float)hospitalLocalNow.DayOfWeek,

            CurrentQueueSize =
                liveQueue.Count,

            RecentAverageWaitingMinutes =
            (float)recentAverageWait,

            AvailableDoctorCount =
                availableDoctorCount,

            BranchId =
                ticket.BranchId.ToString(),

            ServiceId =
                ticket.ServiceId.ToString()
                    };

                mlPrediction =
        _queueMLService.Predict(input);
}

      int estimatedWaitMinutes;

        if (ticket.Status == "Serving")
        {
            estimatedWaitMinutes = 0;
        }
      else if (mlPrediction.HasValue)
{
    var predictedMinutes =
        Math.Max(
            1,
            (int)Math.Ceiling(
                mlPrediction.Value));

    // Safety guard:
    // ML prediction must remain realistic
    // relative to the live queue position.
    var baselineEstimate =
        peopleAhead == 0
            ? Math.Max(
                1,
                (int)Math.Ceiling(
                    averageServiceMinutes / 2))
            : (int)Math.Ceiling(
                peopleAhead *
                averageServiceMinutes);

    var maximumReasonableWait =
        baselineEstimate +
        (int)Math.Ceiling(
            averageServiceMinutes * 2);

    estimatedWaitMinutes =
        Math.Min(
            predictedMinutes,
            maximumReasonableWait);
}
        else
        {
            estimatedWaitMinutes =
                (int)Math.Ceiling(
                    peopleAhead *
                    averageServiceMinutes);

            if (peopleAhead == 0)
            {
                estimatedWaitMinutes =
                    Math.Max(
                        1,
                        (int)Math.Ceiling(
                            averageServiceMinutes / 2));
            }
        }

        string queueLoad =
            peopleAhead switch
            {
                <= 2 => "Low",
                <= 5 => "Moderate",
                <= 10 => "High",
                _ => "Very High"
            };

        int confidence =
            validDurations.Count switch
            {
                >= 50 => 90,
                >= 25 => 85,
                >= 10 => 78,
                >= 5 => 70,
                _ => 55
            };

        return new QueuePredictionDto
        {
            TicketNumber =
                ticket.TicketNumber,

            ServiceName =
                ticket.Service?.Name ??
                ticket.ServiceType,

            PeopleAhead =
                peopleAhead,

            EstimatedWaitMinutes =
                estimatedWaitMinutes,

            AverageServiceMinutes =
                averageServiceMinutes,

            QueueLoad =
                queueLoad,

            Confidence =
                confidence,

            PredictionSource =
            mlPrediction.HasValue
                ? "SmartCare ML Model"
                : validDurations.Count > 0
                    ? "Historical Queue Data"
                    : "SmartCare Baseline"
        };
    }
}
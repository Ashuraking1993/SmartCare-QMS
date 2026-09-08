using Microsoft.ML;
using QSmart.Domain.Entities;

namespace QSmart.Infrastructure.MachineLearning;

public class QueueMLService
{
    private readonly MLContext _mlContext;

    private ITransformer? _model;

    public QueueMLService()
    {
        _mlContext = new MLContext(seed: 1);
    }

    public bool IsModelReady =>
        _model != null;

    public List<QueueModelInput> BuildTrainingData(
    List<QueueTicket> history,
    List<Doctor> doctors)
{
    var validTickets = history
        .Where(x =>
            x.Status == "Completed" &&
            x.CalledAt.HasValue &&
            x.CompletedAt.HasValue)
        .OrderBy(x => x.CreatedAt)
        .ToList();

    var trainingData =
        new List<QueueModelInput>();

    foreach (var ticket in validTickets)
    {
        var actualWait =
            (ticket.CalledAt!.Value -
             ticket.CreatedAt)
            .TotalMinutes;

        var serviceTime =
            (ticket.CompletedAt!.Value -
             ticket.CalledAt!.Value)
            .TotalMinutes;
        
        var localCreatedAt =
        ticket.CreatedAt.AddHours(8);

        var localDay =
            localCreatedAt.DayOfWeek;

        var localTime =
            localCreatedAt.TimeOfDay;

        var availableDoctorCount =
        doctors.Count(doctor =>
            doctor.BranchId == ticket.BranchId &&
            doctor.ServiceId == ticket.ServiceId &&
            doctor.IsActive &&
            doctor.Availabilities.Any(schedule =>
            schedule.DayOfWeek == localDay &&
            schedule.IsAvailable &&
            schedule.StartTime <= localTime &&
            schedule.EndTime >= localTime));
        // Ignore obviously invalid/outlier records.
        if (actualWait < 0 ||
            actualWait > 240 ||
            serviceTime < 1 ||
            serviceTime > 120)
        {
            continue;
        }

        var peopleAhead =
            validTickets.Count(x =>
                x.BranchId == ticket.BranchId &&
                x.ServiceId == ticket.ServiceId &&
                x.CreatedAt < ticket.CreatedAt &&
                x.CalledAt > ticket.CreatedAt);

       var queueSize =
        validTickets.Count(x =>
        x.BranchId == ticket.BranchId &&
        x.ServiceId == ticket.ServiceId &&
        x.CreatedAt < ticket.CreatedAt &&
        x.CalledAt > ticket.CreatedAt);

        var previousTickets =
            validTickets
                .Where(x =>
                    x.ServiceId == ticket.ServiceId &&
                    x.CreatedAt < ticket.CreatedAt)
                .TakeLast(20)
                .ToList();

        double recentAvgService = 6;
        double recentAvgWait = 0;

        if (previousTickets.Count > 0)
        {
            recentAvgService =
                previousTickets.Average(x =>
                    (x.CompletedAt!.Value -
                     x.CalledAt!.Value)
                    .TotalMinutes);

            recentAvgWait =
                previousTickets.Average(x =>
                    (x.CalledAt!.Value -
                     x.CreatedAt)
                    .TotalMinutes);
        }

        trainingData.Add(
            new QueueModelInput
            {
                ActualWaitMinutes =
                    (float)actualWait,

                PeopleAhead =
                    peopleAhead,

               HourOfDay =
                localCreatedAt.Hour,

                DayOfWeek =
                (float)localCreatedAt.DayOfWeek,

              CurrentQueueSize =
                queueSize,

            RecentAverageServiceMinutes =
                (float)recentAvgService,

            RecentAverageWaitingMinutes =
                (float)recentAvgWait,

                // Doctors scheduled for this
                // branch/service/date/time.
                AvailableDoctorCount =
                    availableDoctorCount,

                BranchId =
                    ticket.BranchId.ToString(),

                ServiceId =
                ticket.ServiceId.ToString()
            });
    }

    return trainingData;
}
    public void Train(
        List<QueueModelInput> trainingData)
    {
        if (trainingData.Count < 10)
        {
            _model = null;
            return;
        }

        var dataView =
            _mlContext.Data.LoadFromEnumerable(
                trainingData);

        var pipeline =
            _mlContext.Transforms.Categorical
                .OneHotEncoding(
                    outputColumnName: "ServiceEncoded",
                    inputColumnName: nameof(
                        QueueModelInput.ServiceId))


             .Append(
            _mlContext.Transforms.Categorical
                .OneHotEncoding(
                    outputColumnName: "BranchEncoded",
                    inputColumnName: nameof(
                        QueueModelInput.BranchId)))

            .Append(
                _mlContext.Transforms.Concatenate(
                    "Features",

                "ServiceEncoded",
                "BranchEncoded",

                nameof(
                    QueueModelInput.PeopleAhead),

                nameof(
                    QueueModelInput.HourOfDay),

                nameof(
                    QueueModelInput.DayOfWeek),

                nameof(
                    QueueModelInput.CurrentQueueSize),

                nameof(
                    QueueModelInput.AvailableDoctorCount),

                nameof(
                    QueueModelInput.RecentAverageServiceMinutes),

                nameof(
                    QueueModelInput.RecentAverageWaitingMinutes)
                                ))
            .Append(
                _mlContext.Regression.Trainers
                    .Sdca(
                        labelColumnName: "Label",
                        featureColumnName: "Features"));

        _model =
            pipeline.Fit(dataView);
    }

    public float Predict(
        QueueModelInput input)
    {
        if (_model == null)
        {
            throw new InvalidOperationException(
                "SmartCare ML model has not been trained.");
        }

        var predictionEngine =
            _mlContext.Model
                .CreatePredictionEngine<
                    QueueModelInput,
                    QueueModelOutput>(_model);

        var prediction =
            predictionEngine.Predict(input);

        return Math.Max(
            0,
            prediction.PredictedWaitMinutes);
    }
}
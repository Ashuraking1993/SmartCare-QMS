using QSmart.Domain.Entities;

namespace QSmart.Application.Interfaces;

public interface IQueueRepository
{
    Task<QueueTicket?> GetLastTicketAsync(
        Guid branchId);

    Task AddAsync(
        QueueTicket ticket);

    Task SaveChangesAsync();

    Task<QueueTicket?> GetNextWaitingTicketAsync();

    Task<QueueTicket?> GetByTicketNumberAsync(
    string ticketNumber);

    Task<int> GetWaitingCountAsync();

    Task<int> GetCompletedCountAsync();

    Task<QueueTicket?> GetCurrentServingAsync();

    Task<List<QueueTicket>> GetHistoryAsync();

    Task<List<QueueTicket>> GetServingTicketsAsync();

    Task<List<QueueTicket>> GetUpNextTicketsAsync();

    Task<QueueTicket?> GetCurrentServingByCounterAsync(
    Guid counterId);

       // =========================
    // MOBILE PATIENT QUEUE
    // =========================

    Task<QueueTicket?> GetActiveTicketByUserAsync(
        Guid userId);

    Task<List<QueueTicket>> GetPatientHistoryAsync(
    Guid userId);
    
    Task<int> GetPeopleAheadAsync(
        QueueTicket ticket);

    Task<List<QueueTicket>> GetLiveQueueAsync(
        Guid branchId,
        Guid serviceId);

    Task<List<QueueTicket>> GetCompletedTicketsForPredictionAsync(
    Guid branchId,
    Guid serviceId,
    int limit = 100);  

    Task<int> GetAvailableDoctorCountAsync(
    Guid branchId,
    Guid serviceId,
    DateTime dateTime);  

    Task<List<Doctor>> GetDoctorsWithAvailabilityAsync();
}
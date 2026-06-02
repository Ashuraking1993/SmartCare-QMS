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
}
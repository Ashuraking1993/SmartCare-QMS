using Microsoft.EntityFrameworkCore;
using QSmart.Application.Interfaces;
using QSmart.Domain.Entities;
using QSmart.Persistence.Context;

namespace QSmart.Infrastructure.Repositories;

public class QueueRepository : IQueueRepository
{
    private readonly AppDbContext _context;

    private static int _normalServeCount = 0;

    public QueueRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<QueueTicket?> GetLastTicketAsync(
        Guid branchId)
    {
        return await _context.QueueTickets
            .Where(x => x.BranchId == branchId)
            .OrderByDescending(x => x.CreatedAt)
            .FirstOrDefaultAsync();
    }

    public async Task AddAsync(
        QueueTicket ticket)
    {
        await _context.QueueTickets.AddAsync(ticket);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }

    public async Task<QueueTicket?> GetNextWaitingTicketAsync()
    {
        QueueTicket? ticket = null;

        // Every 3rd ticket, try priority first
        if (_normalServeCount >= 2)
        {
            ticket = await _context.QueueTickets
                .Include(x => x.Service)
                .Where(x =>
                    x.Status == "Waiting" &&
                    x.Service!.IsPriority)
                .OrderBy(x => x.CreatedAt)
                .FirstOrDefaultAsync();

            if (ticket != null)
            {
                _normalServeCount = 0;
                return ticket;
            }
        }

        // Get normal ticket
        ticket = await _context.QueueTickets
            .Include(x => x.Service)
            .Where(x =>
                x.Status == "Waiting" &&
                !x.Service!.IsPriority)
            .OrderBy(x => x.CreatedAt)
            .FirstOrDefaultAsync();

        // If no normal ticket exists, get ANY waiting ticket
        if (ticket == null)
        {
            ticket = await _context.QueueTickets
                .Include(x => x.Service)
                .Where(x => x.Status == "Waiting")
                .OrderBy(x => x.CreatedAt)
                .FirstOrDefaultAsync();
        }

        if (ticket != null)
        {
            _normalServeCount++;
        }

        return ticket;
    }

    public async Task<QueueTicket?> GetByTicketNumberAsync(
        string ticketNumber)
    {
        return await _context.QueueTickets
            .FirstOrDefaultAsync(
                x => x.TicketNumber == ticketNumber);
    }

    public async Task<int> GetWaitingCountAsync()
    {
        return await _context.QueueTickets
            .CountAsync(x => x.Status == "Waiting");
    }

    public async Task<int> GetCompletedCountAsync()
    {
        return await _context.QueueTickets
            .CountAsync(x => x.Status == "Completed");
    }

    public async Task<QueueTicket?> GetCurrentServingAsync()
    {
        return await _context.QueueTickets
            .Where(x => x.Status == "Serving")
            .OrderByDescending(x => x.CalledAt)
            .FirstOrDefaultAsync();
    }

    public async Task<List<QueueTicket>> GetHistoryAsync()
    {
        return await _context.QueueTickets
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();
    }

    public async Task<QueueTicket?>
        GetCurrentServingByCounterAsync(Guid counterId)
    {
        return await _context.QueueTickets
            .Where(x =>
                x.Status == "Serving" &&
                x.CounterId == counterId)
            .OrderByDescending(x => x.CalledAt)
            .FirstOrDefaultAsync();
    }

    public async Task<List<QueueTicket>>
        GetServingTicketsAsync()
    {
        return await _context.QueueTickets
            .Where(x => x.Status == "Serving")
            .OrderBy(x => x.CalledAt)
            .ToListAsync();
    }

    public async Task<List<QueueTicket>>
        GetUpNextTicketsAsync()
    {
        return await _context.QueueTickets
            .Where(x => x.Status == "Waiting")
            .OrderBy(x => x.CreatedAt)
            .Take(5)
            .ToListAsync();
    }

    // =====================================================
    // MOBILE PATIENT QUEUE
    // =====================================================

    public async Task<QueueTicket?>
        GetActiveTicketByUserAsync(Guid userId)
    {
        return await _context.QueueTickets
            .Include(x => x.Service)
            .Include(x => x.Branch)
            .Include(x => x.Counter)
            .Where(x =>
                x.UserId == userId &&
                (
                    x.Status == "Waiting" ||
                    x.Status == "Serving"
                ))
            .OrderByDescending(x => x.CreatedAt)
            .FirstOrDefaultAsync();
    }

    public async Task<List<QueueTicket>> GetPatientHistoryAsync(
    Guid userId)
{
    return await _context.QueueTickets
        .AsNoTracking()
        .Include(x => x.Service)
        .Include(x => x.Branch)
        .Include(x => x.Counter)
        .Where(x =>
            x.UserId == userId &&
            (
                x.Status == "Completed" ||
                x.Status == "Cancelled" ||
                x.Status == "NoShow"
            ))
        .OrderByDescending(x => x.CreatedAt)
        .ToListAsync();
}

        public async Task<int> GetPeopleAheadAsync(
            QueueTicket ticket)
        {
            if (ticket.Status != "Waiting")
            {
                return 0;
            }

            return await _context.QueueTickets
                .CountAsync(x =>
                    x.BranchId == ticket.BranchId &&
                    x.ServiceId == ticket.ServiceId &&
                    x.Status == "Waiting" &&
                    x.CreatedAt < ticket.CreatedAt);
        }

    public async Task<List<QueueTicket>>
        GetLiveQueueAsync(
            Guid branchId,
            Guid serviceId)
    {
        return await _context.QueueTickets
            .Include(x => x.Service)
            .Where(x =>
                x.BranchId == branchId &&
                x.ServiceId == serviceId &&
                (
                    x.Status == "Waiting" ||
                    x.Status == "Serving"
                ))
            .OrderBy(x =>
                x.Status == "Serving" ? 0 : 1)
            .ThenBy(x => x.CreatedAt)
            .Take(10)
            .ToListAsync();
    }

    public async Task<List<QueueTicket>>
    GetCompletedTicketsForPredictionAsync(
        Guid branchId,
        Guid serviceId,
        int limit = 100)
{
    return await _context.QueueTickets
        .AsNoTracking()
        .Where(x =>
            x.BranchId == branchId &&
            x.ServiceId == serviceId &&
            x.Status == "Completed" &&
            x.CalledAt != null &&
            x.CompletedAt != null)
        .OrderByDescending(x => x.CompletedAt)
        .Take(limit)
        .ToListAsync();
}

    public async Task<int> GetAvailableDoctorCountAsync(
        Guid branchId,
        Guid serviceId,
        DateTime dateTime)
    {
        var day =
            dateTime.DayOfWeek;

        var time =
            dateTime.TimeOfDay;

        return await _context.Doctors
            .Where(x =>
                x.BranchId == branchId &&
                x.ServiceId == serviceId &&
                x.IsActive)
            .Where(x =>
                x.Availabilities.Any(a =>
                    a.DayOfWeek == day &&
                    a.IsAvailable &&
                    a.StartTime <= time &&
                    a.EndTime >= time))
            .CountAsync();
    }

    public async Task<List<Doctor>>
        GetDoctorsWithAvailabilityAsync()
    {
        return await _context.Doctors
            .AsNoTracking()
            .Include(x => x.Availabilities)
            .Where(x => x.IsActive)
            .ToListAsync();
    }
}
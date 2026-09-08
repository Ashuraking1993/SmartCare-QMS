using Microsoft.EntityFrameworkCore;
using QSmart.Application.Interfaces;
using QSmart.Domain.Entities;
using QSmart.Persistence.Context;

namespace QSmart.Infrastructure.Repositories;

public class AppointmentRepository
    : IAppointmentRepository
{
    private readonly AppDbContext _context;

    public AppointmentRepository(
        AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(
        Appointment appointment)
    {
        await _context.Appointments
            .AddAsync(appointment);
    }

    public async Task<Appointment?> GetByIdAsync(
        Guid id)
    {
        return await _context.Appointments
            .Include(x => x.Doctor)
            .Include(x => x.Service)
            .Include(x => x.Branch)
            .Include(x => x.QueueTicket)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<List<Appointment>>
        GetByUserIdAsync(Guid userId)
    {
        return await _context.Appointments
            .AsNoTracking()
            .Include(x => x.Doctor)
            .Include(x => x.Service)
            .Include(x => x.Branch)
            .Include(x => x.QueueTicket)
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.AppointmentDate)
            .ToListAsync();
    }

    public async Task<bool>
        HasDoctorConflictAsync(
            Guid doctorId,
            DateTime appointmentDate)
    {
        return await _context.Appointments
            .AnyAsync(x =>
                x.DoctorId == doctorId &&
                x.AppointmentDate == appointmentDate &&
                x.Status == "Scheduled");
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }

    public async Task<List<DateTime>> GetBookedSlotsAsync(
    Guid doctorId,
    DateTime startUtc,
    DateTime endUtc)
{
    return await _context.Appointments
        .AsNoTracking()
        .Where(x =>
            x.DoctorId == doctorId &&
            x.Status == "Scheduled" &&
            x.AppointmentDate >= startUtc &&
            x.AppointmentDate < endUtc)
        .Select(x => x.AppointmentDate)
        .ToListAsync();
}
}
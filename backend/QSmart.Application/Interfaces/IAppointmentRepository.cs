using QSmart.Domain.Entities;

namespace QSmart.Application.Interfaces;

public interface IAppointmentRepository
{
    Task AddAsync(Appointment appointment);

    Task<Appointment?> GetByIdAsync(Guid id);

    Task<List<Appointment>> GetByUserIdAsync(Guid userId);

    Task<bool> HasDoctorConflictAsync(
        Guid doctorId,
        DateTime appointmentDate);

    Task SaveChangesAsync();

    Task<List<DateTime>> GetBookedSlotsAsync(
    Guid doctorId,
    DateTime startUtc,
    DateTime endUtc);
}
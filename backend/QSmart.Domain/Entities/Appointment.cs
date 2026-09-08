namespace QSmart.Domain.Entities;

public class Appointment
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }
    public User? User { get; set; }

    public Guid BranchId { get; set; }
    public Branch? Branch { get; set; }

    public Guid ServiceId { get; set; }
    public QueueService? Service { get; set; }

    public Guid DoctorId { get; set; }
    public Doctor? Doctor { get; set; }

    public DateTime AppointmentDate { get; set; }

    public string Status { get; set; } = "Scheduled";

    public DateTime CreatedAt { get; set; } =
        DateTime.UtcNow;

    public DateTime? CancelledAt { get; set; }

    public DateTime? CheckedInAt { get; set; }

    public Guid? QueueTicketId { get; set; }
    public QueueTicket? QueueTicket { get; set; }
}
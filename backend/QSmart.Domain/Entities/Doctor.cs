namespace QSmart.Domain.Entities;

public class Doctor
{
    public Guid Id { get; set; }

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string Specialty { get; set; } = string.Empty;

    public Guid BranchId { get; set; }

    public Branch? Branch { get; set; }

    public Guid ServiceId { get; set; }

    public QueueService? Service { get; set; }

    public bool IsActive { get; set; } = true;

    public ICollection<DoctorAvailability> Availabilities { get; set; }
        = new List<DoctorAvailability>();
}
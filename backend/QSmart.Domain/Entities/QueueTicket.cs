namespace QSmart.Domain.Entities;

public class QueueTicket
{
    public Guid Id { get; set; }

    public string TicketNumber { get; set; }
        = string.Empty;

    public Guid BranchId { get; set; }

    public Branch? Branch { get; set; }

    public Guid CounterId { get; set; }

    public Counter? Counter { get; set; }

    public string ServiceType { get; set; }
        = string.Empty;

    public string Status { get; set; }
        = "Waiting";

    public DateTime CreatedAt { get; set; }
        = DateTime.UtcNow;

    public DateTime? CalledAt { get; set; }

    public DateTime? CompletedAt { get; set; }

    public DateTime? CancelledAt { get; set; }

    public int Number { get; set; }

    public Guid ServiceId { get; set; }

    public QueueService? Service { get; set; }

     public Guid? UserId { get; set; }

    public User? User { get; set; }
}
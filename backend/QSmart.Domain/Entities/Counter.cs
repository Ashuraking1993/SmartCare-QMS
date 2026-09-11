namespace QSmart.Domain.Entities;

public class Counter
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;

    public Guid BranchId { get; set; }

    public Branch? Branch { get; set; }

    public Guid ServiceId { get; set; }

    public QueueService? Service { get; set; }

    public ICollection<QueueTicket> QueueTickets { get; set; }
        = new List<QueueTicket>();
}
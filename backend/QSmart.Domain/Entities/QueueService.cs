namespace QSmart.Domain.Entities;

public class QueueService
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Prefix { get; set; } = string.Empty;

    public bool IsPriority { get; set; }

    public bool IsActive { get; set; } = true;
}
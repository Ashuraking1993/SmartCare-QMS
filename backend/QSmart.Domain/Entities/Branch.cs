namespace QSmart.Domain.Entities;

public class Branch
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Code { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;

    public ICollection<Counter> Counters { get; set; }
        = new List<Counter>();
}
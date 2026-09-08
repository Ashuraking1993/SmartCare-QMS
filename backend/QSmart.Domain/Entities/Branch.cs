namespace QSmart.Domain.Entities;

public class Branch
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Code { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;

    // Hospital location information
    public string Address { get; set; } = string.Empty;

    public string City { get; set; } = string.Empty;

    public double Latitude { get; set; }

    public double Longitude { get; set; }

    public string ContactNumber { get; set; } = string.Empty;

    public ICollection<Counter> Counters { get; set; }
        = new List<Counter>();
}
namespace QSmart.Application.DTOs.Counter;

public class CreateCounterRequest
{
    public string Name { get; set; } = string.Empty;

    public Guid BranchId { get; set; }
}
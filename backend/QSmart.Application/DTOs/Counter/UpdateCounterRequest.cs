namespace QSmart.Application.DTOs.Counter;

public class UpdateCounterRequest
{
    public string Name { get; set; } = string.Empty;

    public bool IsActive { get; set; }

    public Guid BranchId { get; set; }
}
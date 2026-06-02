namespace QSmart.Application.DTOs.Branch;

public class UpdateBranchRequest
{
    public string Name { get; set; } = string.Empty;

    public string Code { get; set; } = string.Empty;

    public bool IsActive { get; set; }
}
namespace QSmart.Application.DTOs.User;

public class CreateAgentRequest
{
    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;

    public Guid? BranchId { get; set; }

    public Guid? CounterId { get; set; }
}
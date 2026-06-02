namespace QSmart.Application.DTOs.Auth;

public class RegisterRequest
{
    public string FirstName { get; set; } = "";

    public string LastName { get; set; } = "";

    public string Email { get; set; } = "";

    public string Password { get; set; } = "";

    public Guid? BranchId { get; set; }

    public Guid? CounterId { get; set; }
}
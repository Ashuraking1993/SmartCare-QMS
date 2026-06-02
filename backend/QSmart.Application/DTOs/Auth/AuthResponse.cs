namespace QSmart.Application.DTOs.Auth;

public class AuthResponse
{
    public string Token { get; set; } = "";

    public string RefreshToken { get; set; } = "";

    public string Email { get; set; } = "";

    public string Role { get; set; } = "";

    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";

    public string CounterName { get; set; } = "";
    }

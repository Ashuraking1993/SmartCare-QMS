using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using QSmart.Application.Interfaces;

namespace QSmart.Infrastructure.Services;

public class JwtService : IJwtService
{
    private readonly IConfiguration _configuration;

    public JwtService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

   public string GenerateToken(
    Guid userId,
    string email,
    string role,
    Guid? branchId,
    Guid? counterId)
    
    {
       var claims = new List<Claim>
        {
             new(ClaimTypes.NameIdentifier, userId.ToString()),
            new(ClaimTypes.Email, email),
            new(ClaimTypes.Role, role)
        };

        if (branchId.HasValue)
            {
               claims.Add(
               new Claim(
                "BranchId",
                branchId.Value.ToString()));
            }

            if (counterId.HasValue)
            {
                claims.Add(
                new Claim(
                "CounterId",
                counterId.Value.ToString()));
            }

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(
                _configuration["Jwt:Key"]!));

        var creds = new SigningCredentials(
            key,
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler()
            .WriteToken(token);
    }
}
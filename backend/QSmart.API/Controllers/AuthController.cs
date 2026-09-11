using Microsoft.AspNetCore.Mvc;
using QSmart.Application.DTOs.Auth;
using QSmart.Application.Interfaces;
using QSmart.Domain.Entities;

namespace QSmart.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;

    private readonly IJwtService _jwtService;

    public AuthController(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IJwtService jwtService)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _jwtService = jwtService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(
        RegisterRequest request)
    {
        var existingUser =
            await _userRepository
                .GetByEmailAsync(request.Email);

        if (existingUser != null)
        {
            return BadRequest(
                "Email already exists");
        }

        var user = new User
        {
            Id = Guid.NewGuid(),
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            PasswordHash =
                _passwordHasher.Hash(
                    request.Password),

            RoleId = Guid.Parse(
            "44444444-4444-4444-4444-444444444444"),

            BranchId = request.BranchId,

            CounterId = request.CounterId,    

            CreatedAt = DateTime.UtcNow
            
        };

        await _userRepository.AddAsync(user);

        await _userRepository.SaveChangesAsync();

        return Ok("User registered successfully");
    }


    [HttpPost("login")]
    public async Task<IActionResult> Login(
        LoginRequest request)
    {
    var user =
        await _userRepository
            .GetByEmailAsync(request.Email);

    if (user == null)
    {
        return Unauthorized();
    }

    var valid =
        _passwordHasher.Verify(
            request.Password,
            user.PasswordHash);

    if (!valid)
    {
        return Unauthorized();
    }

   var token =
    _jwtService.GenerateToken(
        user.Id,
        user.Email,
        user.Role!.Name,
        user.BranchId,
        user.CounterId);

   return Ok(new AuthResponse
    {
        Token = token,
        Email = user.Email,
        Role = user.Role.Name,
        CounterName = user.Counter?.Name ?? "",
        FirstName = user.FirstName,
        LastName = user.LastName
    });

    
    }

    [HttpPost("create-demo-agents")]
public async Task<IActionResult> CreateDemoAgents()
{
    var tellerRoleId =
        Guid.Parse(
            "33333333-3333-3333-3333-333333333333");

    var branchId =
        Guid.Parse(
            "aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");

    var agents = new[]
    {
        new
        {
            FirstName = "Specialist",
            LastName = "Agent",
            Email = "specialist@smartcare.com",
            CounterId = Guid.Parse(
                "bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb3")
        },
        new
        {
            FirstName = "Laboratory",
            LastName = "Agent",
            Email = "laboratory@smartcare.com",
            CounterId = Guid.Parse(
                "bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb4")
        },
        new
        {
            FirstName = "Dental",
            LastName = "Agent",
            Email = "dental@smartcare.com",
            CounterId = Guid.Parse(
                "bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb5")
        },
        new
        {
            FirstName = "Other",
            LastName = "Services",
            Email = "other@smartcare.com",
            CounterId = Guid.Parse(
                "bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb6")
        }
    };

    foreach (var agent in agents)
    {
        var existing =
            await _userRepository
                .GetByEmailAsync(agent.Email);

        if (existing != null)
        {
            continue;
        }

        var user = new User
        {
            Id = Guid.NewGuid(),

            FirstName =
                agent.FirstName,

            LastName =
                agent.LastName,

            Email =
                agent.Email,

            PasswordHash =
                _passwordHasher.Hash(
                    "Agent123!"),

            RoleId =
                tellerRoleId,

            BranchId =
                branchId,

            CounterId =
                agent.CounterId,

            CreatedAt =
                DateTime.UtcNow
        };

        await _userRepository
            .AddAsync(user);
    }

    await _userRepository
        .SaveChangesAsync();

    return Ok(new
    {
        message =
            "Demo department agents created."
    });
}

    [HttpPost("reset-admin")]
    public async Task<IActionResult> ResetAdminPassword()
    {
        var admin =
            await _userRepository
                .GetByEmailAsync("admin@smartcare.com");

        if (admin == null)
        {
            return NotFound("Admin account not found.");
        }

        admin.PasswordHash =
            _passwordHasher.Hash("Admin123!");

        await _userRepository.SaveChangesAsync();

        return Ok(new
        {
            message = "Admin password reset successfully.",
            email = "admin@smartcare.com"
        });
    }
}
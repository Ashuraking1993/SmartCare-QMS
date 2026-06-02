using Microsoft.AspNetCore.Mvc;
using QSmart.Application.Interfaces;
using QSmart.Application.DTOs.User;
using QSmart.Domain.Entities;

namespace QSmart.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserRepository _userRepository;
   
    private readonly IPasswordHasher _passwordHasher;

   public UserController(
    IUserRepository userRepository,
    IPasswordHasher passwordHasher)
{
    _userRepository = userRepository;
    _passwordHasher = passwordHasher;
}

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var users =
            await _userRepository.GetAllAsync();

        return Ok(users);
    }

    [HttpPut("{id}/assign-branch")]
    public async Task<IActionResult> AssignBranch(
        Guid id,
        AssignBranchRequest request)
    {
        var user =
            await _userRepository.GetByIdAsync(id);

        if (user == null)
            return NotFound();

        user.BranchId = request.BranchId;

        await _userRepository.SaveChangesAsync();

        return Ok(user);
    }

    [HttpPut("{id}/assign-counter")]
    public async Task<IActionResult> AssignCounter(
        Guid id,
        AssignCounterRequest request)
    {
        var user =
            await _userRepository.GetByIdAsync(id);

        if (user == null)
            return NotFound();

        user.CounterId = request.CounterId;

        await _userRepository.SaveChangesAsync();

        return Ok(user);
    }

    [HttpPost("create-agent")]
public async Task<IActionResult> CreateAgent(
    CreateAgentRequest request)
{
    var existingUser =
        await _userRepository.GetByEmailAsync(request.Email);

    if (existingUser != null)
        return BadRequest("Email already exists");

    var user = new User
    {
        Id = Guid.NewGuid(),
        FirstName = request.FirstName,
        LastName = request.LastName,
        Email = request.Email,
        PasswordHash = _passwordHasher.Hash(request.Password),

        // Teller Role
        RoleId = Guid.Parse(
            "33333333-3333-3333-3333-333333333333"),

        BranchId = request.BranchId,
        CounterId = request.CounterId,
        CreatedAt = DateTime.UtcNow
    };

    await _userRepository.AddAsync(user);
    await _userRepository.SaveChangesAsync();

    return Ok(user);
    }
}
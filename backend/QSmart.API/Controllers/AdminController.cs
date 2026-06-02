using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QSmart.Application.DTOs.Admin;
using QSmart.Persistence.Context;

namespace QSmart.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AdminController : ControllerBase
{
    private readonly AppDbContext _context;

    public AdminController(
        AppDbContext context)
    {
        _context = context;
    }

    [HttpGet("dashboard")]
    public async Task<IActionResult> Dashboard()
    {
        var result =
            new DashboardStatsResponse
            {
                TotalWaiting =
                    await _context.QueueTickets
                        .CountAsync(x =>
                            x.Status == "Waiting"),

                TotalServing =
                    await _context.QueueTickets
                        .CountAsync(x =>
                            x.Status == "Serving"),

                TotalCompletedToday =
                    await _context.QueueTickets
                        .CountAsync(x =>
                            x.Status == "Completed" &&
                            x.CompletedAt.HasValue &&
                            x.CompletedAt.Value.Date ==
                            DateTime.UtcNow.Date),

                ActiveCounters =
                    await _context.Counters
                        .CountAsync(x =>
                            x.IsActive)
            };

        return Ok(result);
    }

    [HttpGet("waiting")]
    public async Task<IActionResult> Waiting()
    {
        var tickets = await _context.QueueTickets
            .Include(x => x.Branch)
            .Include(x => x.Counter)
            .Where(x => x.Status == "Waiting")
            .OrderBy(x => x.CreatedAt)
            .ToListAsync();

        return Ok(tickets);
    }

    [HttpGet("serving")]
    public async Task<IActionResult> Serving()
    {
        var tickets = await _context.QueueTickets
            .Include(x => x.Branch)
            .Include(x => x.Counter)
            .Where(x => x.Status == "Serving")
            .OrderBy(x => x.CalledAt)
            .ToListAsync();

        return Ok(tickets);
    }

    [HttpGet("completed")]
    public async Task<IActionResult> Completed()
    {
        var tickets = await _context.QueueTickets
            .Include(x => x.Branch)
            .Include(x => x.Counter)
            .Where(x => x.Status == "Completed")
            .OrderByDescending(x => x.CompletedAt)
            .ToListAsync();

        return Ok(tickets);
    }

    
}
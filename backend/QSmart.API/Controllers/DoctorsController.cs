using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QSmart.Persistence.Context;

namespace QSmart.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DoctorsController : ControllerBase
{
    private readonly AppDbContext _context;

    public DoctorsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetDoctors()
    {
        var hospitalNow = DateTime.UtcNow.AddHours(8);

        var day = hospitalNow.DayOfWeek;
        var time = hospitalNow.TimeOfDay;

        var doctors = await _context.Doctors
            .AsNoTracking()
            .Include(x => x.Branch)
            .Include(x => x.Service)
            .Include(x => x.Availabilities)
            .Where(x => x.IsActive)
            .OrderBy(x => x.LastName)
            .Select(x => new
            {
                id = x.Id,

                name = $"Dr. {x.FirstName} {x.LastName}",

                specialty = x.Specialty,

                serviceName =
                    x.Service != null
                        ? x.Service.Name
                        : "",

                hospitalName =
                    x.Branch != null
                        ? x.Branch.Name
                        : "",

                branchId = x.BranchId,

                serviceId = x.ServiceId,

                isAvailableNow =
                    x.Availabilities.Any(a =>
                        a.DayOfWeek == day &&
                        a.IsAvailable &&
                        a.StartTime <= time &&
                        a.EndTime >= time),

                todaySchedule =
                    x.Availabilities
                        .Where(a =>
                            a.DayOfWeek == day &&
                            a.IsAvailable)
                        .Select(a => new
                        {
                            startTime = a.StartTime,
                            endTime = a.EndTime
                        })
                        .FirstOrDefault()
            })
            .ToListAsync();

        return Ok(doctors);
    }
}
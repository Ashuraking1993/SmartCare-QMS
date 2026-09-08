using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QSmart.Application.DTOs.Analytics;
using QSmart.Persistence.Context;

namespace QSmart.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AnalyticsController : ControllerBase
{
    private readonly AppDbContext _context;

    public AnalyticsController(AppDbContext context)
    {
        _context = context;
    }

    [Authorize]
    [HttpGet("patient-insights")]
    public async Task<ActionResult<PatientInsightsDto>>
        GetPatientInsights()
    {
        // Database stores timestamps in UTC.
        var today = DateTime.UtcNow.Date;
        var tomorrow = today.AddDays(1);

        var tickets = await _context.QueueTickets
            .AsNoTracking()
            .Include(x => x.Service)
            .Where(x =>
                x.CreatedAt >= today &&
                x.CreatedAt < tomorrow)
            .ToListAsync();

        var result = new PatientInsightsDto
        {
            TotalPatientsToday = tickets.Count,

            WaitingNow = tickets.Count(x =>
                x.Status == "Waiting"),

            ServingNow = tickets.Count(x =>
                x.Status == "Serving"),

            CompletedToday = tickets.Count(x =>
                x.Status == "Completed")
        };

        // =========================================
        // AVERAGE WAITING TIME
        // CalledAt - CreatedAt
        // =========================================

        var calledTickets = tickets
            .Where(x => x.CalledAt.HasValue)
            .ToList();

        if (calledTickets.Count > 0)
        {
            result.AverageWaitMinutes =
                Math.Round(
                    calledTickets.Average(x =>
                        (x.CalledAt!.Value -
                         x.CreatedAt).TotalMinutes),
                    1);
        }

        // =========================================
        // AVERAGE SERVICE TIME
        // CompletedAt - CalledAt
        // =========================================

        var completedTickets = tickets
            .Where(x =>
                x.CalledAt.HasValue &&
                x.CompletedAt.HasValue)
            .ToList();

        if (completedTickets.Count > 0)
        {
            result.AverageServiceMinutes =
                Math.Round(
                    completedTickets.Average(x =>
                        (x.CompletedAt!.Value -
                         x.CalledAt!.Value)
                        .TotalMinutes),
                    1);
        }

        // =========================================
        // DEPARTMENT ANALYTICS
        // =========================================

        result.Departments = tickets
            .Where(x => x.Service != null)
            .GroupBy(x => new
            {
                x.ServiceId,
                ServiceName = x.Service!.Name
            })
            .Select(group =>
            {
                var departmentCalled =
                    group.Where(x =>
                        x.CalledAt.HasValue)
                    .ToList();

                var averageWait =
                    departmentCalled.Count > 0
                        ? Math.Round(
                            departmentCalled.Average(x =>
                                (x.CalledAt!.Value -
                                 x.CreatedAt)
                                .TotalMinutes),
                            1)
                        : 0;

                return new DepartmentInsightDto
                {
                    ServiceId =
                        group.Key.ServiceId,

                    ServiceName =
                        group.Key.ServiceName,

                    PatientCount =
                        group.Count(),

                    WaitingNow =
                        group.Count(x =>
                            x.Status == "Waiting"),

                    AverageWaitMinutes =
                        averageWait
                };
            })
            .OrderByDescending(x =>
                x.PatientCount)
            .ToList();

        // =========================================
        // BUSIEST DEPARTMENT
        // =========================================

        result.BusiestDepartment =
            result.Departments
                .FirstOrDefault()
                ?.ServiceName
            ?? "No data";

        // =========================================
        // HOURLY TREND
        // =========================================

        result.HourlyTrend = tickets
            .GroupBy(x => x.CreatedAt.Hour)
            .OrderBy(group => group.Key)
            .Select(group =>
                new HourlyInsightDto
                {
                    Hour = group.Key,

                    Label = new DateTime(
                            2000,
                            1,
                            1,
                            group.Key,
                            0,
                            0)
                        .ToString("h tt"),

                    PatientCount =
                        group.Count()
                })
            .ToList();

        // =========================================
        // BUSIEST HOUR
        // =========================================

        var busiestHour =
            result.HourlyTrend
                .OrderByDescending(x =>
                    x.PatientCount)
                .FirstOrDefault();

        result.BusiestHour =
            busiestHour?.Label
            ?? "No data";

        return Ok(result);
    }
}
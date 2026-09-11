using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QSmart.Application.DTOs.Queue;
using QSmart.Application.Interfaces;
using QSmart.Domain.Entities;

namespace QSmart.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class QueueController : ControllerBase
{
   private readonly IQueueRepository _queueRepository;
   private readonly ICounterRepository _counterRepository;
    private readonly IQueuePredictionService _queuePredictionService;

  public QueueController(
    IQueueRepository queueRepository,
    IQueuePredictionService queuePredictionService,
    ICounterRepository counterRepository)
{
    _queueRepository = queueRepository;
    _queuePredictionService = queuePredictionService;
    _counterRepository = counterRepository;
}

    // =====================================================
    // KIOSK / WALK-IN
    // =====================================================

    [HttpPost("generate")]
    public async Task<IActionResult> Generate(
        [FromBody] GenerateTicketRequest request)
    {
        var branchId =
            Guid.Parse(
                "aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");

        var lastTicket =
            await _queueRepository
                .GetLastTicketAsync(branchId);

        var nextNumber =
            lastTicket?.Number + 1 ?? 1;

        var serviceId = request.ServiceId;

        var prefix =
            GetServicePrefix(serviceId);

        if (prefix == null)
        {
            return BadRequest(
                "Invalid queue service.");
        }

        var ticket = new QueueTicket
        {
            Id = Guid.NewGuid(),
            Number = nextNumber,

            TicketNumber =
                $"{prefix}{nextNumber:000}",

            ServiceId = serviceId,
            BranchId = branchId,

            CounterId =
                Guid.Parse(
                    "bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),

            // Walk-in ticket has no patient account
            UserId = null,

            Status = "Waiting",
            CreatedAt = DateTime.UtcNow
        };

        await _queueRepository
            .AddAsync(ticket);

        await _queueRepository
            .SaveChangesAsync();

        return Ok(new
        {
            ticket.TicketNumber
        });
    }

    // =====================================================
    // MOBILE PATIENT - JOIN QUEUE
    // =====================================================

    [Authorize(Roles = "Patient")]
    [HttpPost("join")]
    public async Task<IActionResult> JoinQueue(
        [FromBody] GenerateTicketRequest request)
    {
        var userId = GetCurrentUserId();

        if (userId == null)
        {
            return Unauthorized();
        }

        // Prevent patient from having multiple
        // active queue tickets.
        var existingTicket =
            await _queueRepository
                .GetActiveTicketByUserAsync(
                    userId.Value);

        if (existingTicket != null)
        {
            return Conflict(new
            {
                message =
                    "You already have an active queue.",

                ticketNumber =
                    existingTicket.TicketNumber,

                status =
                    existingTicket.Status
            });
        }

        var branchId =
            Guid.Parse(
                "aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");

        var serviceId =
            request.ServiceId;

        var prefix =
            GetServicePrefix(serviceId);

        if (prefix == null)
        {
            return BadRequest(
                "Invalid queue service.");
        }

        var lastTicket =
            await _queueRepository
                .GetLastTicketAsync(branchId);

        var nextNumber =
            lastTicket?.Number + 1 ?? 1;

        var ticket =
            new QueueTicket
            {
                Id = Guid.NewGuid(),

                Number = nextNumber,

                TicketNumber =
                    $"{prefix}{nextNumber:000}",

                BranchId = branchId,

                CounterId =
                    Guid.Parse(
                        "bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),

                ServiceId = serviceId,

                // THIS connects ticket to patient.
                UserId = userId.Value,

                Status = "Waiting",

                CreatedAt =
                    DateTime.UtcNow
            };

        await _queueRepository
            .AddAsync(ticket);

        await _queueRepository
            .SaveChangesAsync();

        return Ok(new
        {
            message =
                "Successfully joined the queue.",

            ticket.TicketNumber,

            ticket.Status,

            ticket.CreatedAt
        });
    }

    // =====================================================
    // MOBILE PATIENT - MY QUEUE
    // =====================================================

    [Authorize(Roles = "Patient")]
    [HttpGet("my-queue")]
    public async Task<IActionResult> MyQueue()
    {
        var userId =
            GetCurrentUserId();

        if (userId == null)
        {
            return Unauthorized();
        }

        var ticket =
            await _queueRepository
                .GetActiveTicketByUserAsync(
                    userId.Value);

        if (ticket == null)
        {
            return NotFound(new
            {
                message =
                    "You do not have an active queue."
            });
        }

        var peopleAhead =
            await _queueRepository
                .GetPeopleAheadAsync(ticket);

        var liveQueue =
            await _queueRepository
                .GetLiveQueueAsync(
                    ticket.BranchId,
                    ticket.ServiceId);

        return Ok(new
        {
            ticketNumber =
                ticket.TicketNumber,

            status =
                ticket.Status,

            serviceId =
                ticket.ServiceId,

            serviceName =
                ticket.Service?.Name
                ?? "Hospital Service",

            branchName =
                ticket.Branch?.Name
                ?? "SmartCare Hospital",

            counterName =
                ticket.Counter?.Name
                ?? "",

            peopleAhead,

            waitingSince =
                ticket.CreatedAt,

             calledAt =
                ticket.CalledAt,

            checkedInAt =
                ticket.CheckedInAt,

            liveQueue =
            liveQueue.Select(x => new
                {
                    ticketNumber =
                        x.TicketNumber,

                    status =
                        x.Status
                })
        });
    }


    // =====================================================
// MOBILE PATIENT - CHECK IN
// =====================================================

[Authorize(Roles = "Patient")]
[HttpPost("check-in")]
public async Task<IActionResult> CheckIn()
{
    var userId = GetCurrentUserId();

    if (userId == null)
    {
        return Unauthorized();
    }

    var ticket =
        await _queueRepository
            .GetActiveTicketByUserAsync(
                userId.Value);

    if (ticket == null)
    {
        return NotFound(new
        {
            message =
                "You do not have an active queue."
        });
    }

    // Only waiting patients can check in.
    if (!string.Equals(
            ticket.Status,
            "Waiting",
            StringComparison.OrdinalIgnoreCase))
    {
        return BadRequest(new
        {
            message =
                "Only waiting patients can check in."
        });
    }

    // Prevent duplicate check-in.
    if (ticket.CheckedInAt.HasValue)
    {
        return Ok(new
        {
            message =
                "You are already checked in.",

            ticketNumber =
                ticket.TicketNumber,

            checkedInAt =
                ticket.CheckedInAt
        });
    }

    ticket.CheckedInAt =
        DateTime.UtcNow;

    await _queueRepository
        .SaveChangesAsync();

    return Ok(new
    {
        message =
            "Check-in successful.",

        ticketNumber =
            ticket.TicketNumber,

        status =
            ticket.Status,

        checkedInAt =
            ticket.CheckedInAt
    });
}

    // =====================================================
// MOBILE PATIENT - QUEUE PREDICTION
// =====================================================

[Authorize(Roles = "Patient")]
[HttpGet("prediction")]
public async Task<IActionResult> Prediction()
{
    var userId = GetCurrentUserId();

    if (userId == null)
    {
        return Unauthorized();
    }

    var ticket =
        await _queueRepository
            .GetActiveTicketByUserAsync(
                userId.Value);

    if (ticket == null)
    {
        return NotFound(new
        {
            message =
                "You do not have an active queue."
        });
    }

    var prediction =
        await _queuePredictionService
            .PredictAsync(ticket);

    return Ok(prediction);
}

    // =====================================================
    // MOBILE PATIENT - LEAVE / CANCEL QUEUE
    // =====================================================

    [Authorize(Roles = "Patient")]
    [HttpPost("leave")]
    public async Task<IActionResult> LeaveQueue()
    {
        var userId =
            GetCurrentUserId();

        if (userId == null)
        {
            return Unauthorized();
        }

        var ticket =
            await _queueRepository
                .GetActiveTicketByUserAsync(
                    userId.Value);

        if (ticket == null)
        {
            return NotFound(new
            {
                message =
                    "You do not have an active queue."
            });
        }

        // Don't allow cancellation once serving.
        if (ticket.Status == "Serving")
        {
            return BadRequest(new
            {
                message =
                    "Your queue is already being served."
            });
        }

            ticket.Status =
            "Cancelled";

            ticket.CancelledAt =
                DateTime.UtcNow;

            await _queueRepository
            .SaveChangesAsync();

        return Ok(new
        {
            message =
                "You have left the queue.",

            ticketNumber =
                ticket.TicketNumber,

            status =
                ticket.Status
        });
    }

    // =====================================================
    // AGENT - CALL NEXT
    // =====================================================

  [Authorize]
[HttpPost("next")]
public async Task<IActionResult> Next()
{
    // Get the logged-in agent's counter.
    var counterIdClaim =
        User.FindFirst("CounterId")?.Value;

    if (string.IsNullOrWhiteSpace(counterIdClaim))
    {
        return Unauthorized(new
        {
            message =
                "No counter is assigned to this agent."
        });
    }

            if (!Guid.TryParse(
                    counterIdClaim,
                    out var counterId))
            {
                return Unauthorized(new
                {
                    message =
                        "Invalid counter assignment."
                });
            }

            var counter =
            await _counterRepository
                .GetByIdAsync(counterId);

        if (counter == null)
        {
            return NotFound(new
            {
                message = "Assigned department was not found."
            });
        }

    // IMPORTANT:
    // One counter can only serve one ticket at a time.
    var currentServing =
        await _queueRepository
            .GetCurrentServingByCounterAsync(
                counterId);

    if (currentServing != null)
    {
        return Conflict(new
        {
            message =
                "Please complete the current ticket before calling the next patient.",

            ticketNumber =
                currentServing.TicketNumber
        });
    }

    // Get next waiting patient.
   var ticket =
    await _queueRepository
        .GetNextWaitingTicketAsync(
            counter.BranchId,
            counter.ServiceId);

    if (ticket == null)
    {
        return NotFound(new
        {
            message =
                "No waiting tickets."
        });
    }

    // IMPORTANT:
    // Assign the ticket to THIS agent's counter.
    ticket.CounterId = counterId;

    ticket.Status = "Serving";

    ticket.CalledAt =
        DateTime.UtcNow;

    await _queueRepository
        .SaveChangesAsync();

    return Ok(new
    {
        ticket.TicketNumber,
        ticket.Status,
        counterId
    });
}

    // =====================================================
    // AGENT - COMPLETE
    // =====================================================

    [Authorize]
[HttpPost("complete/{ticketNumber}")]
public async Task<IActionResult> Complete(
    string ticketNumber)
{
    var counterIdClaim =
        User.FindFirst("CounterId")?.Value;

    if (string.IsNullOrWhiteSpace(counterIdClaim))
    {
        return Unauthorized(new
        {
            message = "No counter is assigned to this agent."
        });
    }

    if (!Guid.TryParse(counterIdClaim, out var counterId))
    {
        return Unauthorized(new
        {
            message = "Invalid counter assignment."
        });
    }

    // Get the actual ticket currently being served
    // by THIS logged-in counter.
    var currentServing =
        await _queueRepository
            .GetCurrentServingByCounterAsync(counterId);

    if (currentServing == null)
    {
        return NotFound(new
        {
            message = "No ticket is currently being served at this counter."
        });
    }

    // Prevent completing another counter's / stale ticket.
    if (!string.Equals(
        currentServing.TicketNumber,
        ticketNumber,
        StringComparison.OrdinalIgnoreCase))
    {
        return Conflict(new
        {
            message =
                $"This counter is currently serving {currentServing.TicketNumber}, not {ticketNumber}."
        });
    }

    currentServing.Status = "Completed";
    currentServing.CompletedAt = DateTime.UtcNow;

    await _queueRepository.SaveChangesAsync();

    return Ok(new
    {
        currentServing.TicketNumber,
        currentServing.Status,
        currentServing.CompletedAt,
        counterId
    });
}

    // =====================================================
    // DASHBOARD
    // =====================================================

    [Authorize]
[HttpGet("dashboard")]
public async Task<IActionResult> Dashboard()
{
    // Get logged-in agent's assigned counter.
    var counterIdClaim =
        User.FindFirst("CounterId")?.Value;

    if (string.IsNullOrWhiteSpace(counterIdClaim))
    {
        return Unauthorized(new
        {
            message =
                "No counter is assigned to this agent."
        });
    }

    if (!Guid.TryParse(
            counterIdClaim,
            out var counterId))
    {
        return Unauthorized(new
        {
            message =
                "Invalid counter assignment."
        });
    }

    // Only show the ticket being served
    // by THIS agent's counter.
    var serving =
        await _queueRepository
            .GetCurrentServingByCounterAsync(
                counterId);

    var counters =
    await _counterRepository
        .GetAllAsync();

        
    var waiting =
        await _queueRepository
            .GetWaitingCountAsync();

    var completed =
        await _queueRepository
            .GetCompletedCountAsync();

    return Ok(new
    {
        nowServing =
            serving?.TicketNumber,

        waitingCount =
            waiting,

        completedCount =
            completed,

        counterId
    });
}

    // =====================================================
    // HISTORY
    // =====================================================

    [Authorize]
    [HttpGet("history")]
    public async Task<IActionResult> History()
    {
        var tickets =
            await _queueRepository
                .GetHistoryAsync();

        return Ok(
            tickets.Select(x => new
            {
                x.TicketNumber,
                x.Status,
                x.CreatedAt,
                x.CalledAt,
                x.CompletedAt
            }));
    }

    // =====================================================
    // CURRENT COUNTER
    // =====================================================

    [Authorize]
    [HttpGet("current")]
    public async Task<IActionResult> Current()
    {
        var counterIdClaim =
            User.FindFirst(
                "CounterId")?.Value;

        if (counterIdClaim == null)
        {
            return Unauthorized();
        }

        var counterId =
            Guid.Parse(
                counterIdClaim);

        var ticket =
            await _queueRepository
                .GetCurrentServingByCounterAsync(
                    counterId);

        if (ticket == null)
        {
            return NotFound(
                "No current serving ticket");
        }

        return Ok(new
        {
            ticket.TicketNumber,
            ticket.Status,
            ticket.CalledAt
        });
    }

    // =====================================================
    // PUBLIC QUEUE DISPLAY
    // =====================================================

   [HttpGet("display")]
        public async Task<IActionResult> Display()
        {
            var serving =
                await _queueRepository
                    .GetServingTicketsAsync();

            var counters =
                await _counterRepository
                    .GetAllAsync();

            var upNext =
                await _queueRepository
                    .GetUpNextTicketsAsync();

            var waiting =
                await _queueRepository
                    .GetWaitingCountAsync();

            return Ok(new
            {
                serving =
            serving.Select(x => new
            {
                x.CounterId,
                x.TicketNumber,

                departmentName =
                    counters
                        .FirstOrDefault(c =>
                            c.Id == x.CounterId)
                        ?.Name
                        ?? "Hospital Department"
            }),

            upNext =
                upNext.Select((x, index) => new
                {
                    ticketNumber =
                        x.TicketNumber,

                    checkedIn =
                        x.CheckedInAt.HasValue,

                        

                    checkedInAt =
                        x.CheckedInAt,

                    queueState =
                        index == 0 && x.CheckedInAt.HasValue
                            ? "READY"
                            : x.CheckedInAt.HasValue
                                ? "CHECKED IN"
                                : "NOT ARRIVED"
                }),

            waitingCount =
                waiting
        });
    }

    // =====================================================
    // HELPERS
    // =====================================================

    private Guid? GetCurrentUserId()
    {
        var claim =
            User.FindFirst(
                ClaimTypes.NameIdentifier);

        if (claim == null)
        {
            return null;
        }

        if (!Guid.TryParse(
                claim.Value,
                out var userId))
        {
            return null;
        }

        return userId;
    }

    private static string?
        GetServicePrefix(Guid serviceId)
    {
        if (serviceId ==
            Guid.Parse(
                "10000000-0000-0000-0000-000000000001"))
        {
            return "G";
        }

        if (serviceId ==
            Guid.Parse(
                "10000000-0000-0000-0000-000000000002"))
        {
            return "E";
        }

        if (serviceId ==
            Guid.Parse(
                "10000000-0000-0000-0000-000000000003"))
        {
            return "S";
        }

        if (serviceId ==
            Guid.Parse(
                "10000000-0000-0000-0000-000000000004"))
        {
            return "L";
        }

        if (serviceId ==
            Guid.Parse(
                "10000000-0000-0000-0000-000000000005"))
        {
            return "D";
        }

        if (serviceId ==
            Guid.Parse(
                "10000000-0000-0000-0000-000000000006"))
        {
            return "O";
        }

        return null;
    }

    // =====================================================
// MOBILE PATIENT - QUEUE HISTORY
// =====================================================

[Authorize(Roles = "Patient")]
[HttpGet("my-history")]
public async Task<IActionResult> MyHistory()
{
    var userId = GetCurrentUserId();

    if (userId == null)
    {
        return Unauthorized();
    }

    var tickets =
        await _queueRepository
            .GetPatientHistoryAsync(userId.Value);

    return Ok(
        tickets.Select(x => new
        {
            ticketNumber = x.TicketNumber,

            status = x.Status,

            serviceName =
                x.Service?.Name
                ?? "Hospital Service",

            branchName =
                x.Branch?.Name
                ?? "SmartCare Hospital",

            counterName =
                x.Counter?.Name
                ?? "",

            createdAt = x.CreatedAt,

            calledAt = x.CalledAt,

            completedAt = x.CompletedAt,

            cancelledAt = x.CancelledAt,

            waitMinutes =
                x.CalledAt.HasValue
                    ? Math.Max(
                        0,
                        Math.Round(
                            (x.CalledAt.Value - x.CreatedAt)
                            .TotalMinutes))
                    : (double?)null,

            serviceMinutes =
                x.CalledAt.HasValue &&
                x.CompletedAt.HasValue
                    ? Math.Max(
                        0,
                        Math.Round(
                            (x.CompletedAt.Value -
                             x.CalledAt.Value)
                            .TotalMinutes))
                    : (double?)null
        }));
}
}
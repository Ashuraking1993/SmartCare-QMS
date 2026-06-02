using Microsoft.AspNetCore.Mvc;
using QSmart.Application.Interfaces;
using QSmart.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using QSmart.Application.DTOs.Queue;

namespace QSmart.API.Controllers;

[ApiController]
[Route("api/[controller]")]


public class QueueController : ControllerBase
{
    private readonly IQueueRepository _queueRepository;

    public QueueController(
        IQueueRepository queueRepository)
    {
        _queueRepository = queueRepository;
    }

    [HttpPost("generate")]
    public async Task<IActionResult> Generate(
    [FromBody] GenerateTicketRequest request)
    {
        var branchId =
            Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");

        var lastTicket =
            await _queueRepository
                .GetLastTicketAsync(branchId);

        int nextNumber = 1;

        if (lastTicket != null)
        {
            nextNumber =
                lastTicket.Number + 1;
        }

        var serviceId = request.ServiceId;

        string prefix = "G";

        if (serviceId == Guid.Parse("50BBFB12-49A0-441A-A06D-13DC5890631C"))
            prefix = "P";

        if (serviceId == Guid.Parse("8B746602-0209-4E42-AF06-6B7870CB69BD"))
            prefix = "G";

        if (serviceId == Guid.Parse("CA6AD9BE-54EB-4706-AAC6-93256C5610CF"))
            prefix = "B";

        if (serviceId == Guid.Parse("8740E138-B984-49B5-BE28-CEC86996E594"))
            prefix = "O";

        if (serviceId == Guid.Parse("EE9C8F2C-85D1-40D2-9E57-CF17A1CB28AE"))
            prefix = "V";

        if (serviceId == Guid.Parse("35827DD6-25B5-4D5B-BCE2-F77785A4B80F"))
            prefix = "D";    

        var ticket = new QueueTicket
        {
            Id = Guid.NewGuid(),
            Number = nextNumber,
            TicketNumber = $"{prefix}{nextNumber:000}",
            ServiceId = serviceId,
            BranchId = branchId,
            CounterId =
                Guid.Parse(
                    "bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
            Status = "Waiting",
            CreatedAt = DateTime.UtcNow
        };

        await _queueRepository.AddAsync(ticket);
        await _queueRepository.SaveChangesAsync();

        return Ok(new
        {
            ticket.TicketNumber
        });
    }


    [Authorize]
    [HttpPost("next")]
    public async Task<IActionResult> Next()
    {
        var ticket =
            await _queueRepository
                .GetNextWaitingTicketAsync();

        if (ticket == null)
        {
            return NotFound(
                "No waiting tickets");
        }

    ticket.Status = "Serving";
    ticket.CalledAt = DateTime.UtcNow;

    await _queueRepository.SaveChangesAsync();

    return Ok(new
    {
        ticket.TicketNumber,
        ticket.Status
    });
   }


    [Authorize]
   [HttpPost("complete/{ticketNumber}")]
    public async Task<IActionResult> Complete(
        string ticketNumber)
    {
        var ticket =
            await _queueRepository
                .GetByTicketNumberAsync(ticketNumber);

        if (ticket == null)
        {
            return NotFound();
        }

        ticket.Status = "Completed";
        ticket.CompletedAt = DateTime.UtcNow;

        await _queueRepository.SaveChangesAsync();

        return Ok(new
        {
            ticket.TicketNumber,
            ticket.Status
        });
    }


    [Authorize]
    [HttpGet("dashboard")]
    public async Task<IActionResult> Dashboard()
    {
    var serving =
        await _queueRepository
            .GetCurrentServingAsync();

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
            completed
    });
    }


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


    [Authorize]
    [HttpGet("current")]
    public async Task<IActionResult> Current()
    {
        var counterIdClaim =
            User.FindFirst("CounterId")?.Value;

        if (counterIdClaim == null)
        {
            return Unauthorized();
        }

        var counterId =
            Guid.Parse(counterIdClaim);

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

    [HttpGet("display")]
        public async Task<IActionResult> Display()
        {
            var serving =
                await _queueRepository
                    .GetServingTicketsAsync();

            var upNext =
                await _queueRepository
                    .GetUpNextTicketsAsync();

            var waiting =
                await _queueRepository
                    .GetWaitingCountAsync();

            return Ok(new
            {
                serving = serving.Select(x => new
                {
                    x.CounterId,
                    x.TicketNumber
                }),

                upNext = upNext.Select(x =>
                    x.TicketNumber),

                waitingCount = waiting
            });
        }

}
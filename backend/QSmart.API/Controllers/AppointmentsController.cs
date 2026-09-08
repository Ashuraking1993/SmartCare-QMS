using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QSmart.Application.DTOs.Appointment;
using QSmart.Application.Interfaces;
using QSmart.Domain.Entities;
using QSmart.Persistence.Context;

namespace QSmart.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Patient")]
public class AppointmentsController : ControllerBase
{
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly AppDbContext _context;

    public AppointmentsController(
        IAppointmentRepository appointmentRepository,
        AppDbContext context)
    {
        _appointmentRepository = appointmentRepository;
        _context = context;
    }

    // =====================================================
    // CREATE APPOINTMENT
    // =====================================================

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateAppointmentRequest request)
    {
        var userId = GetCurrentUserId();

        if (userId == null)
        {
            return Unauthorized();
        }

        var doctor = await _context.Doctors
            .AsNoTracking()
            .Include(x => x.Availabilities)
            .FirstOrDefaultAsync(x =>
                x.Id == request.DoctorId &&
                x.IsActive);

        if (doctor == null)
        {
            return NotFound(new
            {
                message = "Doctor not found."
            });
        }

                var appointmentLocal =
                DateTime.SpecifyKind(
                    request.AppointmentDate,
                    DateTimeKind.Unspecified);

            var appointmentUtc =
                TimeZoneInfo.ConvertTimeToUtc(
                    appointmentLocal,
                    TimeZoneInfo.FindSystemTimeZoneById(
                        OperatingSystem.IsWindows()
                            ? "Singapore Standard Time"
                            : "Asia/Manila"));

            var hospitalNow =
                DateTime.UtcNow.AddHours(8);

            if (appointmentLocal <= hospitalNow)
            {
                return BadRequest(new
                {
                    message =
                        "Appointment must be scheduled in the future."
                });
            }

        var appointmentDay =
            request.AppointmentDate.DayOfWeek;

        var appointmentTime =
            request.AppointmentDate.TimeOfDay;

        var availability =
            doctor.Availabilities
                .FirstOrDefault(x =>
                    x.DayOfWeek == appointmentDay &&
                    x.IsAvailable &&
                    x.StartTime <= appointmentTime &&
                    x.EndTime > appointmentTime);

        if (availability == null)
        {
            return BadRequest(new
            {
                message =
                    "Doctor is not available at the selected date and time."
            });
        }

        var hasConflict =
            await _appointmentRepository
            .HasDoctorConflictAsync(
                doctor.Id,
                appointmentUtc);

        if (hasConflict)
        {
            return Conflict(new
            {
                message =
                    "This appointment slot is already booked."
            });
        }

        var appointment =
            new Appointment
            {
                Id = Guid.NewGuid(),

                UserId = userId.Value,

                DoctorId = doctor.Id,

                BranchId = doctor.BranchId,

                ServiceId = doctor.ServiceId,

               AppointmentDate =
                    appointmentUtc,

                Status = "Scheduled",

                CreatedAt = DateTime.UtcNow
            };

        await _appointmentRepository
            .AddAsync(appointment);

        await _appointmentRepository
            .SaveChangesAsync();

        return Ok(new
        {
            appointment.Id,

            appointment.Status,

            appointment.AppointmentDate,

            appointment.DoctorId,

            appointment.BranchId,

            appointment.ServiceId
        });
    }

    // =====================================================
    // MY APPOINTMENTS
    // =====================================================

    [HttpGet("my")]
    public async Task<IActionResult> MyAppointments()
    {
        var userId = GetCurrentUserId();

        if (userId == null)
        {
            return Unauthorized();
        }

        var appointments =
            await _appointmentRepository
                .GetByUserIdAsync(userId.Value);

        return Ok(
            appointments.Select(x => new
            {
                id = x.Id,

                status = x.Status,

                appointmentDate =
                    x.AppointmentDate,

                doctorId = x.DoctorId,

                doctorName =
                    x.Doctor != null
                        ? $"Dr. {x.Doctor.FirstName} {x.Doctor.LastName}"
                        : "",

                specialty =
                    x.Doctor?.Specialty ?? "",

                serviceId = x.ServiceId,

                serviceName =
                    x.Service?.Name ?? "",

                branchId = x.BranchId,

                hospitalName =
                    x.Branch?.Name ?? "",

                cancelledAt =
                    x.CancelledAt,

                checkedInAt =
                    x.CheckedInAt,

                queueTicketNumber =
                    x.QueueTicket?.TicketNumber
            }));
    }

    // =====================================================
    // CANCEL APPOINTMENT
    // =====================================================

    [HttpPost("{id:guid}/cancel")]
    public async Task<IActionResult> Cancel(
        Guid id)
    {
        var userId = GetCurrentUserId();

        if (userId == null)
        {
            return Unauthorized();
        }

        var appointment =
            await _appointmentRepository
                .GetByIdAsync(id);

        if (appointment == null ||
            appointment.UserId != userId.Value)
        {
            return NotFound(new
            {
                message = "Appointment not found."
            });
        }

        if (appointment.Status != "Scheduled")
        {
            return Conflict(new
            {
                message =
                    "Only scheduled appointments can be cancelled."
            });
        }

        appointment.Status = "Cancelled";
        appointment.CancelledAt = DateTime.UtcNow;

        await _appointmentRepository
            .SaveChangesAsync();

        return Ok(new
        {
            appointment.Id,
            appointment.Status,
            appointment.CancelledAt
        });
    }

    // =====================================================
    // CURRENT PATIENT
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

        return Guid.TryParse(
            claim.Value,
            out var userId)
                ? userId
                : null;
    }

    [HttpGet("available-slots")]
public async Task<IActionResult> GetAvailableSlots(
    [FromQuery] Guid doctorId,
    [FromQuery] DateTime date)
{
    var doctor = await _context.Doctors
        .AsNoTracking()
        .Include(x => x.Availabilities)
        .FirstOrDefaultAsync(x =>
            x.Id == doctorId &&
            x.IsActive);

    if (doctor == null)
    {
        return NotFound(new
        {
            message = "Doctor not found."
        });
    }

    var localDate = date.Date;

    var availability =
        doctor.Availabilities.FirstOrDefault(x =>
            x.DayOfWeek == localDate.DayOfWeek &&
            x.IsAvailable);

    if (availability == null)
    {
        return Ok(new
        {
            date = localDate.ToString("yyyy-MM-dd"),
            doctorId,
            slots = Array.Empty<string>()
        });
    }

    var timeZone =
        TimeZoneInfo.FindSystemTimeZoneById(
            OperatingSystem.IsWindows()
                ? "Singapore Standard Time"
                : "Asia/Manila");

    var localStart =
        DateTime.SpecifyKind(
            localDate.Add(availability.StartTime),
            DateTimeKind.Unspecified);

    var localEnd =
        DateTime.SpecifyKind(
            localDate.Add(availability.EndTime),
            DateTimeKind.Unspecified);

    var startUtc =
        TimeZoneInfo.ConvertTimeToUtc(
            localStart,
            timeZone);

    var endUtc =
        TimeZoneInfo.ConvertTimeToUtc(
            localEnd,
            timeZone);

    var bookedSlots =
        await _appointmentRepository
            .GetBookedSlotsAsync(
                doctorId,
                startUtc,
                endUtc);

    var bookedSet = bookedSlots
        .Select(x =>
            TimeZoneInfo
                .ConvertTimeFromUtc(
                    DateTime.SpecifyKind(
                        x,
                        DateTimeKind.Utc),
                    timeZone)
                .ToString("HH:mm"))
        .ToHashSet();

    var hospitalNow =
        TimeZoneInfo.ConvertTimeFromUtc(
            DateTime.UtcNow,
            timeZone);

    var slots = new List<string>();

    var current = localStart;

    while (current.AddMinutes(30) <= localEnd)
    {
        var slot = current.ToString("HH:mm");

        var isPast =
            localDate == hospitalNow.Date &&
            current <= hospitalNow;

        if (!bookedSet.Contains(slot) &&
            !isPast)
        {
            slots.Add(slot);
        }

        current = current.AddMinutes(30);
    }

    return Ok(new
    {
        doctorId,
        date = localDate.ToString("yyyy-MM-dd"),
        slots
    });
}
}
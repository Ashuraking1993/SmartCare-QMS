using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QSmart.Domain.Entities;
using QSmart.Persistence.Context;

namespace QSmart.API.Controllers;

[ApiController]
[Route("api/ml-training")]
public class MLTrainingController : ControllerBase
{
    private readonly AppDbContext _context;

    public MLTrainingController(AppDbContext context)
    {
        _context = context;
    }

    [HttpPost("seed")]
    public async Task<IActionResult> SeedTrainingData()
    {
        var branchId =
            Guid.Parse(
                "aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");

        var counterId =
            Guid.Parse(
                "bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb");

        var serviceIds = new[]
        {
            Guid.Parse("10000000-0000-0000-0000-000000000001"),
            Guid.Parse("10000000-0000-0000-0000-000000000002"),
            Guid.Parse("10000000-0000-0000-0000-000000000003"),
            Guid.Parse("10000000-0000-0000-0000-000000000004"),
            Guid.Parse("10000000-0000-0000-0000-000000000005"),
            Guid.Parse("10000000-0000-0000-0000-000000000006")
        };

        // Prevent accidental duplicate development seed.
        var alreadySeeded =
            await _context.QueueTickets
                .AnyAsync(x =>
                    x.TicketNumber.StartsWith("MLTEST-"));

        if (alreadySeeded)
        {
            return Conflict(new
            {
                message =
                    "ML development training data already exists."
            });
        }

        var random = new Random(42);

        var tickets =
            new List<QueueTicket>();

        var baseDate =
            DateTime.UtcNow.Date.AddDays(-7);

        for (var i = 1; i <= 30; i++)
        {
            var serviceId =
                serviceIds[(i - 1) % serviceIds.Length];

            // Spread examples over several days/hours.
            var createdAt =
                baseDate
                    .AddDays((i - 1) / 5)
                    .AddHours(8 + ((i - 1) % 9))
                    .AddMinutes(random.Next(0, 45));

            // Synthetic but plausible development values.
            var waitMinutes =
                random.Next(5, 36);

            var serviceMinutes =
                random.Next(5, 21);

            var calledAt =
                createdAt.AddMinutes(waitMinutes);

            var completedAt =
                calledAt.AddMinutes(serviceMinutes);

            tickets.Add(
                new QueueTicket
                {
                    Id = Guid.NewGuid(),

                    TicketNumber =
                        $"MLTEST-{i:000}",

                    BranchId = branchId,

                    CounterId = counterId,

                    ServiceId = serviceId,

                    ServiceType = "ML Training",

                    Status = "Completed",

                    Number = 9000 + i,

                    CreatedAt = createdAt,

                    CalledAt = calledAt,

                    CompletedAt = completedAt,

                    // Development training records
                    // are intentionally not linked
                    // to real patient accounts.
                    UserId = null
                });
        }

        await _context.QueueTickets
            .AddRangeAsync(tickets);

        await _context.SaveChangesAsync();

        return Ok(new
        {
            inserted = tickets.Count,
            message =
                "SmartCare ML development training data created."
        });
    }

    [HttpPost("seed-hospitals")]
public async Task<IActionResult> SeedHospitals()
{
    var hospitals = new List<Branch>
    {
        new()
        {
            Id = Guid.Parse(
                "aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
            Name = "SmartCare Medical Center - Manila",
            Code = "SC-MNL",
            Address = "Demo Hospital Location - Manila",
            City = "Manila",
            Latitude = 14.5995,
            Longitude = 120.9842,
            ContactNumber = "Demo Contact",
            IsActive = true
        },

        new()
        {
            Id = Guid.Parse(
                "aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa2"),
            Name = "SmartCare Medical Center - Makati",
            Code = "SC-MKT",
            Address = "Demo Hospital Location - Makati",
            City = "Makati",
            Latitude = 14.5547,
            Longitude = 121.0244,
            ContactNumber = "Demo Contact",
            IsActive = true
        },

        new()
        {
            Id = Guid.Parse(
                "aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa3"),
            Name = "SmartCare Medical Center - Quezon City",
            Code = "SC-QC",
            Address = "Demo Hospital Location - Quezon City",
            City = "Quezon City",
            Latitude = 14.6760,
            Longitude = 121.0437,
            ContactNumber = "Demo Contact",
            IsActive = true
        },

        new()
        {
            Id = Guid.Parse(
                "aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa4"),
            Name = "SmartCare Medical Center - Pasig",
            Code = "SC-PSG",
            Address = "Demo Hospital Location - Pasig",
            City = "Pasig",
            Latitude = 14.5764,
            Longitude = 121.0851,
            ContactNumber = "Demo Contact",
            IsActive = true
        },

        new()
        {
            Id = Guid.Parse(
                "aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa5"),
            Name = "SmartCare Medical Center - Mandaluyong",
            Code = "SC-MDL",
            Address = "Demo Hospital Location - Mandaluyong",
            City = "Mandaluyong",
            Latitude = 14.5794,
            Longitude = 121.0359,
            ContactNumber = "Demo Contact",
            IsActive = true
        }
    };

    var inserted = 0;
    var updated = 0;

    foreach (var hospital in hospitals)
    {
        var existing =
            await _context.Branches
                .FirstOrDefaultAsync(x =>
                    x.Id == hospital.Id);

        if (existing == null)
        {
            await _context.Branches.AddAsync(
                hospital);

            inserted++;
        }
        else
        {
            existing.Name =
                hospital.Name;

            existing.Code =
                hospital.Code;

            existing.Address =
                hospital.Address;

            existing.City =
                hospital.City;

            existing.Latitude =
                hospital.Latitude;

            existing.Longitude =
                hospital.Longitude;

            existing.ContactNumber =
                hospital.ContactNumber;

            existing.IsActive = true;

            updated++;
        }
    }

    await _context.SaveChangesAsync();

    return Ok(new
    {
        inserted,
        updated,
        total = hospitals.Count,
        message =
            "SmartCare demo hospitals created successfully."
    });
}

[HttpPost("seed-doctors")]
public async Task<IActionResult> SeedDoctors()
{
    var hospitalIds = new[]
    {
        Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
        Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa2"),
        Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa3"),
        Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa4"),
        Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa5")
    };

    var services = new[]
    {
        new
        {
            Id = Guid.Parse(
                "10000000-0000-0000-0000-000000000001"),
            Specialty = "General Medicine"
        },
        new
        {
            Id = Guid.Parse(
                "10000000-0000-0000-0000-000000000002"),
            Specialty = "Emergency Medicine"
        },
        new
        {
            Id = Guid.Parse(
                "10000000-0000-0000-0000-000000000003"),
            Specialty = "Internal Medicine"
        },
        new
        {
            Id = Guid.Parse(
                "10000000-0000-0000-0000-000000000004"),
            Specialty = "Laboratory Medicine"
        },
        new
        {
            Id = Guid.Parse(
                "10000000-0000-0000-0000-000000000005"),
            Specialty = "Dentistry"
        },
        new
        {
            Id = Guid.Parse(
                "10000000-0000-0000-0000-000000000006"),
            Specialty = "Family Medicine"
        }
    };

    var firstNames = new[]
    {
        "Miguel",
        "Angela",
        "Paolo",
        "Sofia",
        "Daniel",
        "Bianca",
        "Marco",
        "Isabella",
        "Gabriel",
        "Andrea"
    };

    var lastNames = new[]
    {
        "Santos",
        "Reyes",
        "Cruz",
        "Garcia",
        "Mendoza",
        "Torres",
        "Flores",
        "Ramos",
        "Navarro",
        "Castillo"
    };

    var doctorsCreated = 0;
    var schedulesCreated = 0;

    for (var hospitalIndex = 0;
         hospitalIndex < hospitalIds.Length;
         hospitalIndex++)
    {
        for (var serviceIndex = 0;
             serviceIndex < services.Length;
             serviceIndex++)
        {
            var doctorId =
                Guid.Parse(
                    $"20000000-0000-0000-{hospitalIndex + 1:0000}-{serviceIndex + 1:000000000000}");

            var doctor =
                await _context.Doctors
                    .FirstOrDefaultAsync(x =>
                        x.Id == doctorId);

            if (doctor == null)
            {
                doctor = new Doctor
                {
                    Id = doctorId,

                    FirstName =
                        firstNames[
                            (hospitalIndex * 2 +
                             serviceIndex) %
                            firstNames.Length],

                    LastName =
                        lastNames[
                            (hospitalIndex * 3 +
                             serviceIndex) %
                            lastNames.Length],

                    Specialty =
                        services[serviceIndex]
                            .Specialty,

                    BranchId =
                        hospitalIds[hospitalIndex],

                    ServiceId =
                        services[serviceIndex].Id,

                    IsActive = true
                };

                await _context.Doctors
                    .AddAsync(doctor);

                doctorsCreated++;
            }

            var hasSchedule =
                await _context
                    .DoctorAvailabilities
                    .AnyAsync(x =>
                        x.DoctorId == doctorId);

            if (hasSchedule)
            {
                continue;
            }

            // Emergency doctors:
            // available every day.
            if (serviceIndex == 1)
            {
                for (var day = 0;
                     day <= 6;
                     day++)
                {
                    _context.DoctorAvailabilities.Add(
                        new DoctorAvailability
                        {
                            Id = Guid.NewGuid(),

                            DoctorId = doctorId,

                            DayOfWeek =
                                (DayOfWeek)day,

                            StartTime =
                                new TimeSpan(
                                    8, 0, 0),

                            EndTime =
                                new TimeSpan(
                                    20, 0, 0),

                            IsAvailable = true
                        });

                    schedulesCreated++;
                }

                continue;
            }

            // Dental:
            // Tuesday to Saturday.
            if (serviceIndex == 4)
            {
                for (var day = 2;
                     day <= 6;
                     day++)
                {
                    _context.DoctorAvailabilities.Add(
                        new DoctorAvailability
                        {
                            Id = Guid.NewGuid(),

                            DoctorId = doctorId,

                            DayOfWeek =
                                (DayOfWeek)day,

                            StartTime =
                                new TimeSpan(
                                    10, 0, 0),

                            EndTime =
                                new TimeSpan(
                                    18, 0, 0),

                            IsAvailable = true
                        });

                    schedulesCreated++;
                }

                continue;
            }

            // Specialist:
            // Monday, Wednesday, Friday.
            if (serviceIndex == 2)
            {
                var specialistDays =
                    new[]
                    {
                        DayOfWeek.Monday,
                        DayOfWeek.Wednesday,
                        DayOfWeek.Friday
                    };

                foreach (var day
                         in specialistDays)
                {
                    _context.DoctorAvailabilities.Add(
                        new DoctorAvailability
                        {
                            Id = Guid.NewGuid(),

                            DoctorId = doctorId,

                            DayOfWeek = day,

                            StartTime =
                                new TimeSpan(
                                    9, 0, 0),

                            EndTime =
                                new TimeSpan(
                                    17, 0, 0),

                            IsAvailable = true
                        });

                    schedulesCreated++;
                }

                continue;
            }

            // General, Laboratory and Other:
            // Monday to Friday.
            for (var day = 1;
                 day <= 5;
                 day++)
            {
                _context.DoctorAvailabilities.Add(
                    new DoctorAvailability
                    {
                        Id = Guid.NewGuid(),

                        DoctorId = doctorId,

                        DayOfWeek =
                            (DayOfWeek)day,

                        StartTime =
                            new TimeSpan(
                                8, 0, 0),

                        EndTime =
                            new TimeSpan(
                                17, 0, 0),

                        IsAvailable = true
                    });

                schedulesCreated++;
            }
        }
    }

    await _context.SaveChangesAsync();

    return Ok(new
    {
        doctorsCreated,
        schedulesCreated,

        totalDoctors =
            await _context.Doctors.CountAsync(),

        totalSchedules =
            await _context
                .DoctorAvailabilities
                .CountAsync(),

        message =
            "SmartCare demo doctors and availability schedules created successfully."
    });

    
}

[HttpPost("seed-history")]
public async Task<IActionResult> SeedHistoricalData()
{
    const int targetTickets = 1000;

    var alreadyExists =
        await _context.QueueTickets
            .AnyAsync(x =>
                x.TicketNumber.StartsWith("MLHIST-"));

    if (alreadyExists)
    {
        return Conflict(new
        {
            message =
                "ML historical development data already exists."
        });
    }

    var branches =
        await _context.Branches
            .Where(x => x.IsActive)
            .OrderBy(x => x.Code)
            .ToListAsync();

    var services =
        await _context.QueueServices
            .Where(x => x.IsActive)
            .OrderBy(x => x.Prefix)
            .ToListAsync();

    var doctors =
        await _context.Doctors
            .Include(x => x.Availabilities)
            .Where(x => x.IsActive)
            .ToListAsync();

    var counters =
        await _context.Counters
            .Where(x => x.IsActive)
            .ToListAsync();

    if (branches.Count == 0 ||
        services.Count == 0 ||
        doctors.Count == 0)
    {
        return BadRequest(new
        {
            message =
                "Hospitals, services and doctors must be seeded first."
        });
    }

    var random =
        new Random(20260906);

    var tickets =
        new List<QueueTicket>();

    var startDate =
        DateTime.UtcNow.Date.AddDays(-90);

    for (var i = 1; i <= targetTickets; i++)
    {
        var branch =
            branches[random.Next(branches.Count)];

        var service =
            services[random.Next(services.Count)];

        // -------------------------------------------------
        // Spread historical activity over the last 90 days
        // -------------------------------------------------

        var dayOffset =
            random.Next(0, 90);

        var localDate =
            startDate
                .AddDays(dayOffset)
                .Date;

        // Most hospital traffic occurs during daytime.
        var hourRoll =
            random.Next(100);

        int localHour;

        if (hourRoll < 35)
        {
            // Morning rush
            localHour =
                random.Next(8, 11);
        }
        else if (hourRoll < 75)
        {
            // Midday / early afternoon
            localHour =
                random.Next(11, 15);
        }
        else
        {
            // Late afternoon
            localHour =
                random.Next(15, 18);
        }

        var localCreatedAt =
            localDate
                .AddHours(localHour)
                .AddMinutes(
                    random.Next(0, 60));

        // Database QueueTicket timestamps use UTC.
        var createdAt =
            localCreatedAt.AddHours(-8);

        // -------------------------------------------------
        // Determine doctors actually scheduled at this
        // hospital/service/day/time.
        // -------------------------------------------------

        var availableDoctorCount =
            doctors.Count(doctor =>
                doctor.BranchId == branch.Id &&
                doctor.ServiceId == service.Id &&
                doctor.IsActive &&
                doctor.Availabilities.Any(schedule =>
                    schedule.DayOfWeek ==
                        localCreatedAt.DayOfWeek &&
                    schedule.IsAvailable &&
                    schedule.StartTime <=
                        localCreatedAt.TimeOfDay &&
                    schedule.EndTime >=
                        localCreatedAt.TimeOfDay));

        // -------------------------------------------------
        // Service-specific processing characteristics
        // -------------------------------------------------

        double baseServiceMinutes =
            service.Prefix switch
            {
                "G" => 12,
                "E" => 18,
                "S" => 22,
                "L" => 10,
                "D" => 25,
                "O" => 14,
                _ => 12
            };

        // Small realistic variation.
        var serviceNoise =
            random.Next(-3, 5);

        var serviceMinutes =
            Math.Max(
                4,
                baseServiceMinutes +
                serviceNoise);

        // -------------------------------------------------
        // Simulated demand / people ahead
        // -------------------------------------------------

        var peopleAhead =
            random.Next(0, 5);

        // Morning rush increases demand.
        if (localHour >= 8 &&
            localHour <= 10)
        {
            peopleAhead +=
                random.Next(2, 6);
        }

        // Monday tends to be busier.
        if (localCreatedAt.DayOfWeek ==
            DayOfWeek.Monday)
        {
            peopleAhead +=
                random.Next(1, 4);
        }

        // Friday gets a smaller demand increase.
        if (localCreatedAt.DayOfWeek ==
            DayOfWeek.Friday)
        {
            peopleAhead +=
                random.Next(0, 3);
        }

        // Emergency demand is less predictable.
        if (service.Prefix == "E")
        {
            peopleAhead +=
                random.Next(0, 4);
        }

        // -------------------------------------------------
        // Branch-specific load
        // -------------------------------------------------

        double branchLoadFactor =
            branch.Code switch
            {
                "SC-MNL" => 1.25,
                "SC-MKT" => 1.15,
                "SC-QC" => 1.10,
                "SC-PSG" => 1.00,
                "SC-MDL" => 0.95,
                _ => 1.00
            };

        // -------------------------------------------------
        // Doctor capacity
        // -------------------------------------------------

        double doctorCapacityFactor;

        if (availableDoctorCount <= 0)
        {
            // No doctor currently scheduled:
            // simulate delayed handling / coverage.
            doctorCapacityFactor = 1.75;
        }
        else
        {
            doctorCapacityFactor =
                1.0 /
                Math.Min(
                    availableDoctorCount,
                    3);
        }

        // -------------------------------------------------
        // Actual wait target
        // -------------------------------------------------

        var calculatedWait =
            (
                peopleAhead *
                baseServiceMinutes *
                branchLoadFactor *
                doctorCapacityFactor
            );

        // Minimum operational wait.
        calculatedWait +=
            random.Next(2, 8);

        // Additional morning congestion.
        if (localHour >= 8 &&
            localHour <= 10)
        {
            calculatedWait +=
                random.Next(4, 12);
        }

        // Small random noise prevents perfectly
        // deterministic synthetic data.
        calculatedWait +=
            random.Next(-3, 5);

        var waitMinutes =
            Math.Clamp(
                calculatedWait,
                1,
                180);

        var calledAt =
            createdAt.AddMinutes(
                waitMinutes);

        var completedAt =
            calledAt.AddMinutes(
                serviceMinutes);

        // -------------------------------------------------
        // Counter
        // -------------------------------------------------

        var branchCounter =
            counters.FirstOrDefault(x =>
                x.BranchId == branch.Id);

        // Currently only Manila originally has a seeded
        // counter. Use that development counter when a
        // demo branch has no dedicated counter yet.
        var counterId =
            branchCounter?.Id ??
            Guid.Parse(
                "bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb");

        tickets.Add(
            new QueueTicket
            {
                Id = Guid.NewGuid(),

                TicketNumber =
                    $"MLHIST-{i:0000}",

                BranchId =
                    branch.Id,

                CounterId =
                    counterId,

                ServiceId =
                    service.Id,

                ServiceType =
                    service.Name,

                Status =
                    "Completed",

                Number =
                    10000 + i,

                CreatedAt =
                    createdAt,

                CalledAt =
                    calledAt,

                CompletedAt =
                    completedAt,

                UserId = null
            });
    }

    await _context.QueueTickets
        .AddRangeAsync(tickets);

    await _context.SaveChangesAsync();

    return Ok(new
    {
        inserted =
            tickets.Count,

        hospitals =
            branches.Count,

        services =
            services.Count,

        doctors =
            doctors.Count,

        dateRangeDays = 90,

        message =
            "SmartCare patterned ML historical development data created successfully."
    });
}
}
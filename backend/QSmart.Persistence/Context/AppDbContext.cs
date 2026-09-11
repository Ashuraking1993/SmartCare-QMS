using Microsoft.EntityFrameworkCore;
using QSmart.Domain.Entities;

namespace QSmart.Persistence.Context;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();

    public DbSet<Role> Roles => Set<Role>();

    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

   public DbSet<Branch> Branches => Set<Branch>();

    public DbSet<Counter> Counters => Set<Counter>();

    public DbSet<QueueTicket> QueueTickets => Set<QueueTicket>();

    public DbSet<QueueService> QueueServices { get; set; }

    public DbSet<Doctor> Doctors => Set<Doctor>();

    public DbSet<DoctorAvailability> DoctorAvailabilities
    => Set<DoctorAvailability>();
    public DbSet<Appointment> Appointments { get; set; }


    protected override void OnModelCreating(
    ModelBuilder modelBuilder)
{
    base.OnModelCreating(modelBuilder);

    modelBuilder.Entity<User>()
    .HasOne(x => x.Branch)
    .WithMany()
    .HasForeignKey(x => x.BranchId)
    .OnDelete(DeleteBehavior.NoAction);

    modelBuilder.Entity<User>()
    .HasOne(x => x.Counter)
    .WithMany()
    .HasForeignKey(x => x.CounterId)
    .OnDelete(DeleteBehavior.NoAction);

    modelBuilder.Entity<User>()
        .HasOne(x => x.Role)
        .WithMany(x => x.Users)
        .HasForeignKey(x => x.RoleId);

    modelBuilder.Entity<Counter>()
        .HasOne(x => x.Branch)
        .WithMany(x => x.Counters)
        .HasForeignKey(x => x.BranchId);   

    modelBuilder.Entity<Counter>()
    .HasOne(x => x.Service)
    .WithMany()
    .HasForeignKey(x => x.ServiceId)
    .OnDelete(DeleteBehavior.NoAction);


    modelBuilder.Entity<QueueTicket>()
    .HasOne(x => x.Counter)
    .WithMany(x => x.QueueTickets)
    .HasForeignKey(x => x.CounterId)
    .OnDelete(DeleteBehavior.NoAction);

    modelBuilder.Entity<QueueTicket>()
    .HasOne(x => x.User)
    .WithMany()
    .HasForeignKey(x => x.UserId)
    .OnDelete(DeleteBehavior.SetNull);

    modelBuilder.Entity<Counter>()
    .HasOne(x => x.Branch)
    .WithMany(x => x.Counters)
    .HasForeignKey(x => x.BranchId);

    modelBuilder.Entity<QueueTicket>()
        .HasOne(x => x.Branch)
        .WithMany()
        .HasForeignKey(x => x.BranchId)
        .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<Role>().HasData(
    new Role
    {
        Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
        Name = "Admin"
    },
    new Role
    {
        Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
        Name = "Manager"
    },
    new Role
    {
        Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
        Name = "Teller"
    },
     new Role
    {
        Id = Guid.Parse("44444444-4444-4444-4444-444444444444"),
        Name = "Patient"
    }
    );

    modelBuilder.Entity<Branch>().HasData(
        new Branch
        { Id = Guid.Parse(
        "aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),

            Name = "SmartCare Medical Center - Manila",
            Code = "SC-MNL",
            IsActive = true,

            Address = "Demo Hospital Location - Manila",
            City = "Manila",

            Latitude = 14.5995,
            Longitude = 120.9842,

            ContactNumber = "Demo Contact"
        });

   modelBuilder.Entity<Counter>().HasData(

    // GENERAL CONSULTATION
    new Counter
    {
        Id = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
        Name = "General Consultation",
        BranchId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
        ServiceId = Guid.Parse("10000000-0000-0000-0000-000000000001"),
        IsActive = true
    },

    // EMERGENCY
    new Counter
    {
        Id = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb2"),
        Name = "Emergency",
        BranchId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
        ServiceId = Guid.Parse("10000000-0000-0000-0000-000000000002"),
        IsActive = true
    },

    // SPECIALIST
    new Counter
    {
        Id = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb3"),
        Name = "Specialist",
        BranchId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
        ServiceId = Guid.Parse("10000000-0000-0000-0000-000000000003"),
        IsActive = true
    },

    // LABORATORY
    new Counter
    {
        Id = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb4"),
        Name = "Laboratory",
        BranchId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
        ServiceId = Guid.Parse("10000000-0000-0000-0000-000000000004"),
        IsActive = true
    },

    // DENTAL CARE
    new Counter
    {
        Id = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb5"),
        Name = "Dental Care",
        BranchId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
        ServiceId = Guid.Parse("10000000-0000-0000-0000-000000000005"),
        IsActive = true
    },

    // OTHER SERVICES
    new Counter
    {
        Id = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb6"),
        Name = "Other Services",
        BranchId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
        ServiceId = Guid.Parse("10000000-0000-0000-0000-000000000006"),
        IsActive = true
    }
);

    // ================= QUEUE SERVICES =================

modelBuilder.Entity<QueueService>().HasData(
    new QueueService
    {
        Id = Guid.Parse("10000000-0000-0000-0000-000000000001"),
        Name = "General Consultation",
        Prefix = "G",
        IsPriority = false,
        IsActive = true
    },
    new QueueService
    {
        Id = Guid.Parse("10000000-0000-0000-0000-000000000002"),
        Name = "Emergency",
        Prefix = "E",
        IsPriority = true,
        IsActive = true
    },
    new QueueService
    {
        Id = Guid.Parse("10000000-0000-0000-0000-000000000003"),
        Name = "Specialist",
        Prefix = "S",
        IsPriority = false,
        IsActive = true
    },
    new QueueService
    {
        Id = Guid.Parse("10000000-0000-0000-0000-000000000004"),
        Name = "Laboratory",
        Prefix = "L",
        IsPriority = false,
        IsActive = true
    },
    new QueueService
    {
        Id = Guid.Parse("10000000-0000-0000-0000-000000000005"),
        Name = "Dental Care",
        Prefix = "D",
        IsPriority = false,
        IsActive = true
    },
    new QueueService
    {
        Id = Guid.Parse("10000000-0000-0000-0000-000000000006"),
        Name = "Other Services",
        Prefix = "O",
        IsPriority = false,
        IsActive = true
    }
    );

    // ================= DOCTOR RELATIONSHIPS =================

    modelBuilder.Entity<Doctor>()
        .HasOne(x => x.Branch)
        .WithMany()
        .HasForeignKey(x => x.BranchId)
        .OnDelete(DeleteBehavior.NoAction);

    modelBuilder.Entity<Doctor>()
        .HasOne(x => x.Service)
        .WithMany()
        .HasForeignKey(x => x.ServiceId)
        .OnDelete(DeleteBehavior.NoAction);

    modelBuilder.Entity<DoctorAvailability>()
        .HasOne(x => x.Doctor)
        .WithMany(x => x.Availabilities)
        .HasForeignKey(x => x.DoctorId)
        .OnDelete(DeleteBehavior.Cascade);
        
       
    }

    
}


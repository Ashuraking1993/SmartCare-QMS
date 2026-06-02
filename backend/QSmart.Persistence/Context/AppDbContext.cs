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

    modelBuilder.Entity<QueueTicket>()
    .HasOne(x => x.Counter)
    .WithMany(x => x.QueueTickets)
    .HasForeignKey(x => x.CounterId)
    .OnDelete(DeleteBehavior.NoAction);

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
    }
    );

    modelBuilder.Entity<Branch>().HasData(
        new Branch
        {
            Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
            Name = "Main Branch",
            Code = "MAIN",
            IsActive = true
        });

    modelBuilder.Entity<Counter>().HasData(
        new Counter
        {
        Id = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
        Name = "Counter 1",
        BranchId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
        IsActive = true
    });
       
    }

    
}


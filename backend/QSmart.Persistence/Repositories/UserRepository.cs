using Microsoft.EntityFrameworkCore;
using QSmart.Application.Interfaces;
using QSmart.Domain.Entities;
using QSmart.Persistence.Context;

namespace QSmart.Persistence.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;

    public UserRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByEmailAsync(
        string email)
    {
       return await _context.Users
        .Include(x => x.Role)
        .Include(x => x.Branch)
        .Include(x => x.Counter)
        .FirstOrDefaultAsync(
            x => x.Email == email);
    }

    public async Task AddAsync(User user)
    {
        await _context.Users.AddAsync(user);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }

    public async Task<List<User>> GetAllAsync()
    {
        return await _context.Users
            .Include(x => x.Role)
            .Include(x => x.Branch)
            .Include(x => x.Counter)
            .ToListAsync();
    }

    public async Task<User?> GetByIdAsync(Guid id)
    {
        return await _context.Users
            .Include(x => x.Role)
            .Include(x => x.Branch)
            .Include(x => x.Counter)
            .FirstOrDefaultAsync(x => x.Id == id);
    }
}
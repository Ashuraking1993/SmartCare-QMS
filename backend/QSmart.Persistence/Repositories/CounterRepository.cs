using Microsoft.EntityFrameworkCore;
using QSmart.Application.Interfaces;
using QSmart.Domain.Entities;
using QSmart.Persistence.Context;

namespace QSmart.Infrastructure.Repositories;

public class CounterRepository : ICounterRepository
{
    private readonly AppDbContext _context;

    public CounterRepository(
        AppDbContext context)
    {
        _context = context;
    }

   public async Task<List<Counter>> GetAllAsync()
        {
            return await _context.Counters
                .Include(x => x.Branch)
                .ToListAsync();
        }

        public async Task<Counter?> GetByIdAsync(Guid id)
        {
            return await _context.Counters
                .Include(x => x.Branch)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

    public async Task AddAsync(
        Counter counter)
    {
        await _context.Counters.AddAsync(counter);
    }

    public async Task DeleteAsync(
        Counter counter)
    {
        _context.Counters.Remove(counter);
        await _context.SaveChangesAsync();
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }

    
}
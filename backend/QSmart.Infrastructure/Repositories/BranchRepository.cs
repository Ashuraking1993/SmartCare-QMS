using Microsoft.EntityFrameworkCore;
using QSmart.Application.Interfaces;
using QSmart.Domain.Entities;
using QSmart.Persistence.Context;

namespace QSmart.Infrastructure.Repositories;

public class BranchRepository : IBranchRepository
{
    private readonly AppDbContext _context;

    public BranchRepository(
        AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Branch>> GetAllAsync()
    {
        return await _context.Branches
            .ToListAsync();
    }

    public async Task AddAsync(
    Branch branch)
    {
        await _context.Branches.AddAsync(branch);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }

        public async Task<Branch?> GetByIdAsync(Guid id)
    {
        return await _context.Branches
            .FirstOrDefaultAsync(x => x.Id == id);
    }

        public async Task DeleteAsync(Branch branch)
    {
        _context.Branches.Remove(branch);
        await _context.SaveChangesAsync();
    }
}
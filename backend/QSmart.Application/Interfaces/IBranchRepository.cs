using QSmart.Domain.Entities;

namespace QSmart.Application.Interfaces;

public interface IBranchRepository
{
    Task<List<Branch>> GetAllAsync();
    Task AddAsync(Branch branch);
    Task SaveChangesAsync();
    Task<Branch?> GetByIdAsync(Guid id);
    Task DeleteAsync(Branch branch);
}
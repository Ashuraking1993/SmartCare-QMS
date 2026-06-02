using QSmart.Domain.Entities;

namespace QSmart.Application.Interfaces;

public interface ICounterRepository
{
    Task<List<Counter>> GetAllAsync();

    Task<Counter?> GetByIdAsync(Guid id);

    Task AddAsync(Counter counter);

    Task DeleteAsync(Counter counter);

    Task SaveChangesAsync();
}
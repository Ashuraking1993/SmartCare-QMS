using QSmart.Domain.Entities;

namespace QSmart.Application.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByEmailAsync(string email);

    Task AddAsync(User user);

    Task SaveChangesAsync();

   Task<List<User>> GetAllAsync();

   Task<User?> GetByIdAsync(Guid id);
}
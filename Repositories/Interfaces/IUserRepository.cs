using project_basic.Models;

namespace project_basic.Repositories.Interfaces;

public interface IUserRepository
{
    IQueryable<User> GetQueryable();
    Task<User?> GetByIdAsync(Guid id, CancellationToken ct);
    Task<User?> GetByEmailAsync(string email, CancellationToken ct);
    Task AddAsync(User user, CancellationToken ct);
    Task UpdateAsync(User user);
    Task DeleteAsync(User user);
}

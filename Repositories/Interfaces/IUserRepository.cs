using project_basic.Entities;

namespace project_basic.Repositories.Interfaces;

public interface IUserRepository
{
    IQueryable<User> GetQueryable();
    Task<User?> GetByIdAsync(Guid id, CancellationToken ct);
    Task<User?> GetByEmailAsync(string email, CancellationToken ct);
    Task AddAsync(User user, CancellationToken ct);
    void Update(User user);
    void Delete(User user);
}

namespace project_basic.Repositories.Interfaces;

public interface IUnitOfWork
{
    Task<bool> SaveChangesAsync(CancellationToken ct);
}
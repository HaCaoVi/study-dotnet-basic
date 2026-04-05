namespace project_basic.Repositories.Interfaces;

public interface IGenericRepository
{
    Task<bool> SaveChangesAsync(CancellationToken ct);
}
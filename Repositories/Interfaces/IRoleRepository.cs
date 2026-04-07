namespace project_basic.Repositories.Interfaces;

public interface IRoleRepository
{
    Task<bool> ExistRoleId(Guid id, CancellationToken ct);
}
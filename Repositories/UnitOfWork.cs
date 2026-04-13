using project_basic.Database;
using project_basic.Repositories.Interfaces;

namespace project_basic.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> SaveChangesAsync(CancellationToken ct)
    {
        return (await _context.SaveChangesAsync(ct)) > 0;
    }
}
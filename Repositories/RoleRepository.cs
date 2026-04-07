using Microsoft.EntityFrameworkCore;
using project_basic.Database;
using project_basic.Repositories.Interfaces;

namespace project_basic.Repositories;

public class RoleRepository: IRoleRepository
{
    private readonly ApplicationDbContext _context;
    public RoleRepository(ApplicationDbContext context)
    {
        _context = context;
    }
    
    public async Task<bool> ExistRoleId(Guid id, CancellationToken ct)
    {
        return await  _context.Roles.AnyAsync(r => r.Id == id, ct);
    }
}
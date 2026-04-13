using Microsoft.EntityFrameworkCore;
using project_basic.Database;
using project_basic.Entities;
using project_basic.Repositories.Interfaces;

namespace project_basic.Repositories;

public class CompanyRepository : ICompanyRepository
{
    private readonly ApplicationDbContext _context;

    public CompanyRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public IQueryable<Company> GetQueryable()
    {
        return _context.Companies.AsNoTracking();
    }

    public async Task<Company?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        return await _context.Companies
            .Include(c => c.User)
            .FirstOrDefaultAsync(c => c.Id == id, ct);
    }

    public async Task AddAsync(Company company, CancellationToken ct)
    {
        await _context.Companies.AddAsync(company, ct);
    }

    public void Update(Company company)
    {
        _context.Companies.Update(company);
    }

    public void Delete(Company company)
    {
        _context.Companies.Remove(company);
    }
}
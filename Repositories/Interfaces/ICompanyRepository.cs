using project_basic.Models;

namespace project_basic.Repositories.Interfaces;

public interface ICompanyRepository
{
    IQueryable<Company> GetQueryable();
    Task<Company?> GetByIdAsync(Guid id, CancellationToken ct);
    Task AddAsync(Company company, CancellationToken ct);
    Task UpdateAsync(Company company);
    Task DeleteAsync(Company company);
}
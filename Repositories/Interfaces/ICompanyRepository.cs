using project_basic.Entities;

namespace project_basic.Repositories.Interfaces;

public interface ICompanyRepository
{
    IQueryable<Company> GetQueryable();
    Task<Company?> GetByIdAsync(Guid id, CancellationToken ct);
    Task AddAsync(Company company, CancellationToken ct);
    void Update(Company company);
    void Delete(Company company);
}
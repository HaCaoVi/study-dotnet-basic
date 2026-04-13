using project_basic.Common.Responses;
using project_basic.Dtos.CompanyDto;

namespace project_basic.Services.Interfaces;

public interface ICompanyService
{
    Task<PagedResult<CompanyDto>> GetListCompanyAsync(QueryCompanyDto queryCompanyDto, CancellationToken ct);
    Task<CompanyDto> GetCompanyByIdAsync(Guid id, CancellationToken ct);
    Task<CompanyDto> AddCompanyAsync(CreateCompanyDto createCompanyDto, CancellationToken ct);
    Task UpdateCompanyAsync(Guid id, UpdateCompanyDto updateCompanyDto, CancellationToken ct);
    Task DeleteCompanyAsync(Guid id, CancellationToken ct);
}
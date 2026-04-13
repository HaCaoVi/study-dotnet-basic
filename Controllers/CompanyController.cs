using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using project_basic.Common;
using project_basic.Common.Responses;
using project_basic.Dtos.CompanyDto;
using project_basic.Services.Interfaces;

namespace project_basic.Controllers;

[ApiController]
[Route("api/company")]
public class CompanyController : ControllerBase
{
    private readonly ICompanyService _companyService;

    public CompanyController(ICompanyService companyService)
    {
        _companyService = companyService;
    }

    [AllowAnonymous]
    [HttpGet]
    public async Task<ActionResult<PagedResult<CompanyDto>>> GetListCompany(
        [FromQuery] QueryCompanyDto queryCompanyDto, CancellationToken ct)
    {
        var result = await _companyService.GetListCompanyAsync(queryCompanyDto, ct);
        return Ok(ApiResponse<PagedResult<CompanyDto>>.Success(result, "List company"));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CompanyDto>> GetCompanyById([FromRoute] Guid id, CancellationToken ct)
    {
        var company = await _companyService.GetCompanyByIdAsync(id, ct);
        return Ok(ApiResponse<CompanyDto>.Success(company, "Company found"));
    }

    [HttpPost]
    public async Task<ActionResult<CompanyDto>> CreateCompany(
        [FromBody] CreateCompanyDto createCompanyDto, CancellationToken ct)
    {
        var company = await _companyService.AddCompanyAsync(createCompanyDto, ct);
        return CreatedAtAction(
            nameof(GetCompanyById),
            new { id = company.Id },
            ApiResponse<CompanyDto>.Success(company, "Company added", 201));
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<CompanyDto>> UpdateCompany(
        [FromRoute] Guid id, [FromBody] UpdateCompanyDto updateCompanyDto, CancellationToken ct)
    {
        await _companyService.UpdateCompanyAsync(id, updateCompanyDto, ct);
        return Ok(ApiResponse<object>.Success(null, "Company updated"));
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteCompany([FromRoute] Guid id, CancellationToken ct)
    {
        await _companyService.DeleteCompanyAsync(id, ct);
        return NoContent();
    }
}
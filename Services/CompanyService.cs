using AutoMapper;
using Microsoft.EntityFrameworkCore;
using project_basic.Common.Exceptions;
using project_basic.Common.Responses;
using project_basic.Dtos.CompanyDto;
using project_basic.Entities;
using project_basic.Repositories.Interfaces;
using project_basic.Services.Interfaces;

namespace project_basic.Services;

public class CompanyService : ICompanyService
{
    private readonly ICompanyRepository _companyRepository;
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;

    public CompanyService(
        ICompanyRepository companyRepository,
        IMapper mapper,
        IUnitOfWork unitOfWork,
        IUserRepository userRepository)
    {
        _companyRepository = companyRepository;
        _userRepository = userRepository;
        _mapper = mapper;
        _unitOfWork = unitOfWork;
    }

    public async Task<PagedResult<CompanyDto>> GetListCompanyAsync(QueryCompanyDto queryCompanyDto, CancellationToken ct)
    {
        var queryable = _companyRepository.GetQueryable();
        var totalCount = await queryable.CountAsync(ct);
        var companies = await queryable
            .Include(c => c.User)
            .Skip(queryCompanyDto.PageSize * (queryCompanyDto.PageNumber - 1))
            .Take(queryCompanyDto.PageSize)
            .ToListAsync(ct);
        var result = companies.Select(c => _mapper.Map<CompanyDto>(c)).ToList();
        return new PagedResult<CompanyDto>
        {
            Meta = new Meta
            {
                PageNumber = queryCompanyDto.PageNumber,
                PageSize = queryCompanyDto.PageSize,
                TotalCount = totalCount
            },
            Result = result
        };
    }

    public async Task<CompanyDto> GetCompanyByIdAsync(Guid id, CancellationToken ct)
    {
        var company = await _companyRepository.GetByIdAsync(id, ct);
        if (company == null) throw new NotFoundException($"Company with id {id} not found");
        return _mapper.Map<CompanyDto>(company);
    }

    public async Task<CompanyDto> AddCompanyAsync(CreateCompanyDto createCompanyDto, CancellationToken ct)
    {
        var existingUser = await _userRepository.GetByIdAsync(createCompanyDto.UserId, ct);
        if (existingUser == null) throw new NotFoundException($"User with id {createCompanyDto.UserId} not found");

        var company = new Company
        {
            Name = createCompanyDto.Name,
            Address = createCompanyDto.Address,
            UserId = createCompanyDto.UserId,
        };
        await _companyRepository.AddAsync(company, ct);
        await _unitOfWork.SaveChangesAsync(ct);
        return _mapper.Map<CompanyDto>(company);
    }

    public async Task UpdateCompanyAsync(Guid id, UpdateCompanyDto updateCompanyDto, CancellationToken ct)
    {
        var company = await _companyRepository.GetByIdAsync(id, ct);
        if (company == null) throw new NotFoundException($"Company with id {id} not found");

        company.Name = updateCompanyDto.Name;
        company.Address = updateCompanyDto.Address;
        company.UserId = updateCompanyDto.UserId;

        _companyRepository.Update(company);
        await _unitOfWork.SaveChangesAsync(ct);
    }

    public async Task DeleteCompanyAsync(Guid id, CancellationToken ct)
    {
        var company = await _companyRepository.GetByIdAsync(id, ct);
        if (company == null) throw new NotFoundException($"Company with id {id} not found");

        _companyRepository.Delete(company);
        await _unitOfWork.SaveChangesAsync(ct);
    }
}
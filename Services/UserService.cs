using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using project_basic.Common.Exceptions;
using project_basic.Common.Responses;
using project_basic.Dtos.UserDtos;
using project_basic.Models;
using project_basic.Repositories.Interfaces;
using project_basic.Services.Interfaces;

namespace project_basic.Services;

public class UserService : IUserService
{
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    public UserService(IUserRepository userRepository, IMapper mapper,  IPasswordHasher<User> passwordHasher)
    {
        _userRepository = userRepository;
        _mapper = mapper;
        _passwordHasher = passwordHasher;
    }

    public async Task<PagedResult<UserDto>> GetAllUsersAsync(QueryUserDto queryUserDto, CancellationToken ct)
    {
        var queryable = _userRepository.GetQueryable();
        var totalCount = await queryable.CountAsync();
        var users = await queryable
            .Skip((queryUserDto.PageNumber - 1) * queryUserDto.PageSize)
            .Take(queryUserDto.PageSize)
            .ToListAsync(ct);
        var result = users.Select(u => _mapper.Map<UserDto>(u)).ToList();
        return new PagedResult<UserDto>
        {
            Meta = new Meta
            {
                PageNumber = queryUserDto.PageNumber,
                PageSize = queryUserDto.PageSize,
                TotalCount = totalCount
            },
            Result = result
        };
    }

    public async Task<UserDto> GetUserByIdAsync(Guid id, CancellationToken ct)
    {
        var user = await _userRepository.GetByIdAsync(id, ct);
        if (user == null)
        {
            throw new NotFoundException($"User with ID {id} was not found.");
        }
        return _mapper.Map<UserDto>(user);
    }

    public async Task<UserDto> CreateUserAsync(CreateUserDto createUserDto, CancellationToken ct)
    {
        var existingUser = await _userRepository.GetByEmailAsync(createUserDto.Email, ct);
        if (existingUser != null)
        {
            throw new ConflictException($"User with email {createUserDto.Email} already exists.");
        }
        
        var user = new User
        {
            Email = createUserDto.Email,
            Name = createUserDto.Name,
            Age = createUserDto.Age,
            Address = createUserDto.Address
        };
        user.Password = _passwordHasher.HashPassword(user, createUserDto.Password);
        await _userRepository.AddAsync(user, ct);
        await _userRepository.SaveChangesAsync(ct);

        return _mapper.Map<UserDto>(user);
    }

    public async Task UpdateUserAsync(Guid id, UpdateUserDto updateUserDto, CancellationToken ct)
    {
        var user = await _userRepository.GetByIdAsync(id, ct);
        if (user == null)
        {
            throw new NotFoundException($"User with ID {id} was not found.");
        }

        user.Email = updateUserDto.Email;
        user.Name = updateUserDto.Name;
        user.Age = updateUserDto.Age;
        user.Address = updateUserDto.Address;

        await _userRepository.UpdateAsync(user);
        await _userRepository.SaveChangesAsync(ct);
    }

    public async Task DeleteUserAsync(Guid id, CancellationToken ct)
    {
        var user = await _userRepository.GetByIdAsync(id, ct);
        if (user == null)
        {
            throw new NotFoundException($"User with ID {id} was not found.");
        }

        await _userRepository.DeleteAsync(user);
        await _userRepository.SaveChangesAsync(ct);
    }
}

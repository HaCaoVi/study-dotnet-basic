using project_basic.Common.Exceptions;
using project_basic.Dtos.UserDtos;
using project_basic.Models;
using project_basic.Repositories.Interfaces;
using project_basic.Services.Interfaces;

namespace project_basic.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<IEnumerable<User>> GetAllUsersAsync()
    {
        return await _userRepository.GetAllAsync();
    }

    public async Task<User> GetUserByIdAsync(Guid id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null)
        {
            throw new NotFoundException($"User with ID {id} was not found.");
        }
        return user;
    }

    public async Task<User> CreateUserAsync(CreateUserDto createUserDto)
    {
        var existingUser = await _userRepository.GetByEmailAsync(createUserDto.Email);
        if (existingUser != null)
        {
            throw new ConflictException($"User with email {createUserDto.Email} already exists.");
        }

        var user = new User
        {
            Email = createUserDto.Email,
            Name = createUserDto.Name,
            Password = createUserDto.Password, // In real prod, this should be hashed
            Age = createUserDto.Age,
            Address = createUserDto.Address
        };

        await _userRepository.AddAsync(user);
        await _userRepository.SaveChangesAsync();

        return user;
    }

    public async Task UpdateUserAsync(Guid id, CreateUserDto updateUserDto)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null)
        {
            throw new NotFoundException($"User with ID {id} was not found.");
        }

        user.Email = updateUserDto.Email;
        user.Name = updateUserDto.Name;
        user.Password = updateUserDto.Password;
        user.Age = updateUserDto.Age;
        user.Address = updateUserDto.Address;

        await _userRepository.UpdateAsync(user);
        await _userRepository.SaveChangesAsync();
    }

    public async Task DeleteUserAsync(Guid id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null)
        {
            throw new NotFoundException($"User with ID {id} was not found.");
        }

        await _userRepository.DeleteAsync(user);
        await _userRepository.SaveChangesAsync();
    }
}

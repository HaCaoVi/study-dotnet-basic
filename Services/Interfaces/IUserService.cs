using project_basic.Dtos.UserDtos;
using project_basic.Models;

namespace project_basic.Services.Interfaces;

public interface IUserService
{
    Task<IEnumerable<User>> GetAllUsersAsync();
    Task<User> GetUserByIdAsync(Guid id);
    Task<User> CreateUserAsync(CreateUserDto createUserDto);
    Task UpdateUserAsync(Guid id, CreateUserDto updateUserDto);
    Task DeleteUserAsync(Guid id);
}

using project_basic.Common.Responses;
using project_basic.Dtos.UserDtos;
using project_basic.Models;

namespace project_basic.Services.Interfaces;

public interface IUserService
{
    Task<PagedResult<UserDto>> GetAllUsersAsync(QueryUserDto  queryUserDto);
    Task<UserDto> GetUserByIdAsync(Guid id);
    Task<UserDto> CreateUserAsync(CreateUserDto createUserDto);
    Task UpdateUserAsync(Guid id, UpdateUserDto updateUserDto);
    Task DeleteUserAsync(Guid id);
}

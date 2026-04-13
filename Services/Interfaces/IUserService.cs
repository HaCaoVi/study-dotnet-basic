using project_basic.Common.Responses;
using project_basic.Dtos.UserDtos;

namespace project_basic.Services.Interfaces;

public interface IUserService
{
    Task<PagedResult<UserDto>> GetAllUsersAsync(QueryUserDto  queryUserDto, CancellationToken ct);
    Task<UserDto> GetUserByIdAsync(Guid id, CancellationToken ct);
    Task<UserDto> CreateUserAsync(CreateUserDto createUserDto, CancellationToken ct);
    Task UpdateUserAsync(Guid id, UpdateUserDto updateUserDto, CancellationToken ct);
    Task DeleteUserAsync(Guid id, CancellationToken ct);
}

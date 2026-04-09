using project_basic.Dtos.AuthDtos;
using project_basic.Dtos.UserDtos;

namespace project_basic.Services.Interfaces;

public interface IAuthService
{
    Task<AuthDto> LoginAsync(LoginDto loginDto, CancellationToken ct);
    Task<UserDto> GetAccountAsync(CancellationToken ct); 
    Task<AuthDto> RefreshTokenAsync(string? refreshToken, CancellationToken ct);
    Task LogoutAsync(CancellationToken ct);
}
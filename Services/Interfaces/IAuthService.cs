using project_basic.Dtos.AuthDtos;

namespace project_basic.Services.Interfaces;

public interface IAuthService
{
    Task<AuthDto> LoginAsync(LoginDto loginDto, CancellationToken ct);
}
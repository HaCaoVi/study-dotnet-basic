using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using project_basic.Common.Configuration;
using project_basic.Common.Exceptions;
using project_basic.Dtos.AuthDtos;
using project_basic.Dtos.UserDtos;
using project_basic.Entities;
using project_basic.Repositories.Interfaces;
using project_basic.Services.Interfaces;

namespace project_basic.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly ITokenService _tokenService;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IMapper _mapper;
    private readonly JwtSettings _jwtSettings;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        IPasswordHasher<User> passwordHasher,
        ITokenService tokenService,
        IHttpContextAccessor httpContextAccessor,
        IMapper mapper,
        IOptions<JwtSettings> jwtSettings,
        ILogger<AuthService> logger)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
        _httpContextAccessor = httpContextAccessor;
        _mapper = mapper;
        _jwtSettings = jwtSettings.Value;
        _logger = logger;
    }

    public async Task<AuthDto> LoginAsync(LoginDto loginDto, CancellationToken ct)
    {
        var user = await _userRepository.GetByEmailAsync(loginDto.Email, ct);
        if (user == null)
        {
            throw new NotFoundException("Account not found");
        }

        var checkPass = _passwordHasher.VerifyHashedPassword(user, user.Password, loginDto.Password);
        if (checkPass == PasswordVerificationResult.Failed)
        {
            throw new AuthenticationException("Email or password incorrect");
        }

        var accessToken = _tokenService.GenerateToken(user, _jwtSettings.AccessTokenExpirationMinutes);
        var refreshToken = _tokenService.GenerateToken(user, _jwtSettings.RefreshTokenExpirationMinutes);

        user.RefreshToken = _tokenService.HashToken(refreshToken);
        await _unitOfWork.SaveChangesAsync(ct);

        _logger.LogInformation("User {UserId} logged in successfully", user.Id);

        return new AuthDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            Name = user.Name
        };
    }

    public async Task<UserDto> GetAccountAsync(CancellationToken ct)
    {
        var userId = GetCurrentUserId();
        var user = await _userRepository.GetByIdAsync(userId, ct);
        if (user == null)
        {
            throw new NotFoundException("Account not found");
        }

        return _mapper.Map<UserDto>(user);
    }

    public async Task<AuthDto> RefreshTokenAsync(string? refreshToken, CancellationToken ct)
    {
        if (string.IsNullOrEmpty(refreshToken))
        {
            throw new BadRequestException("RefreshToken not exist");
        }

        var principal = _tokenService.ValidateRefreshToken(refreshToken);
        var userIdStr = principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userIdStr) || !Guid.TryParse(userIdStr, out var userId))
        {
            throw new BadRequestException("Invalid token");
        }

        var user = await _userRepository.GetByIdAsync(userId, ct);
        if (user == null || user.RefreshToken != _tokenService.HashToken(refreshToken))
        {
            throw new BadRequestException("Token invalid");
        }

        var accessToken = _tokenService.GenerateToken(user, _jwtSettings.AccessTokenExpirationMinutes);
        var newRefreshToken = _tokenService.GenerateToken(user, _jwtSettings.RefreshTokenExpirationMinutes);
        user.RefreshToken = _tokenService.HashToken(newRefreshToken);
        await _unitOfWork.SaveChangesAsync(ct);

        _logger.LogInformation("User {UserId} refreshed token", user.Id);

        return new AuthDto
        {
            AccessToken = accessToken,
            RefreshToken = newRefreshToken,
            Name = user.Name
        };
    }

    public async Task LogoutAsync(CancellationToken ct)
    {
        var userId = GetCurrentUserId();
        var user = await _userRepository.GetByIdAsync(userId, ct);
        if (user == null)
        {
            throw new NotFoundException("Account not found");
        }

        user.RefreshToken = string.Empty;
        await _unitOfWork.SaveChangesAsync(ct);

        _logger.LogInformation("User {UserId} logged out", userId);
    }

    /// <summary>
    /// Extracts the current user ID from the JWT claims with proper null-safety.
    /// </summary>
    private Guid GetCurrentUserId()
    {
        var userIdStr = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userIdStr) || !Guid.TryParse(userIdStr, out var userId))
        {
            throw new AuthenticationException("User is not authenticated");
        }

        return userId;
    }
}
using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using project_basic.Common.Exceptions;
using project_basic.Dtos.AuthDtos;
using project_basic.Dtos.UserDtos;
using project_basic.Models;
using project_basic.Repositories.Interfaces;
using project_basic.Services.Interfaces;
using AuthenticationException = project_basic.Common.Exceptions.AuthenticationException;

namespace project_basic.Services;

public class AuthService: IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IGenericRepository _genericRepository;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly ITokenService _tokenService;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IMapper _mapper;
    
    public AuthService(IUserRepository userRepository, IGenericRepository genericRepository, IPasswordHasher<User> passwordHasher,  ITokenService tokenService, IHttpContextAccessor httpContextAccessor, IMapper mapper)
    {
        _userRepository = userRepository;
        _genericRepository = genericRepository;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
        _httpContextAccessor = httpContextAccessor;
        _mapper = mapper;
    }
    
    public async Task<AuthDto> LoginAsync(LoginDto loginDto, CancellationToken ct)
    {
        var checkUser = await _userRepository.GetByEmailAsync(loginDto.Email, ct);
        if (checkUser == null)
        {
            throw new NotFoundException("Account not found");
        }

        var checkPass =  _passwordHasher.VerifyHashedPassword(checkUser, checkUser.Password,loginDto.Password);

        if (checkPass == PasswordVerificationResult.Failed)
        {
            throw new AuthenticationException("Email or password incorrect");
        }

        var accessToken = _tokenService.GenerateToken(checkUser, 30);
        var refreshToken = _tokenService.GenerateToken(checkUser, 30 * 24 * 60);
        
        
        
        checkUser.RefreshToken = _tokenService.HashToken(refreshToken);
        await _genericRepository.SaveChangesAsync(ct);
        return new AuthDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            Name = checkUser.Name
        };
    }

    public async Task<UserDto> GetAccountAsync(CancellationToken ct)
    {
        var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
        var user = await _userRepository.GetByIdAsync(Guid.Parse(userId), ct);
        return _mapper.Map<UserDto>(user);
    }

    public async Task<AuthDto> RefreshTokenAsync(string? refreshToken, CancellationToken ct)
    {
        if (refreshToken == null)
        {
            throw new BadRequestException("RefreshToken not exist");
        }

        var principal = _tokenService.ValidateRefreshToken(refreshToken);
        var userId = principal.FindFirst(ClaimTypes.NameIdentifier).Value;
        var user = await _userRepository.GetByIdAsync(Guid.Parse(userId), ct);
        if (user == null || user.RefreshToken != _tokenService.HashToken(refreshToken)) throw new BadRequestException("Token invalid");
        
        var accessToken = _tokenService.GenerateToken(user, 30);
        var newRefreshToken = _tokenService.GenerateToken(user, 30 * 24 * 60);

        return new AuthDto
        {
            AccessToken = accessToken,
            RefreshToken = newRefreshToken,
            Name = user.Name
        };
    }
}
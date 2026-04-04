using Microsoft.AspNetCore.Identity;
using project_basic.Common.Exceptions;
using project_basic.Dtos.AuthDtos;
using project_basic.Models;
using project_basic.Repositories.Interfaces;
using project_basic.Services.Interfaces;
using AuthenticationException = project_basic.Common.Exceptions.AuthenticationException;

namespace project_basic.Services;

public class AuthService: IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly ITokenService _tokenService;
    
    public AuthService(IUserRepository userRepository, IPasswordHasher<User> passwordHasher,  ITokenService tokenService)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
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
        var refreshToken = _tokenService.GenerateToken(checkUser, 30 * 24 * 60 * 60);
        
        
        
        checkUser.RefreshToken = _tokenService.HashToken(refreshToken);
        await _userRepository.SaveChangesAsync(ct);
        return new AuthDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            Name = checkUser.Name
        };
    }
}
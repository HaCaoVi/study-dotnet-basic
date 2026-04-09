using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using project_basic.Common;
using project_basic.Dtos.AuthDtos;
using project_basic.Dtos.UserDtos;
using project_basic.Services.Interfaces;

namespace project_basic.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController: ControllerBase
{
    private readonly IAuthService _authService;
    
    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }
    
    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<ActionResult<ApiResponse<AuthDto>>> Login(LoginDto loginDto, CancellationToken ct)
    {
        var result = await _authService.LoginAsync(loginDto, ct);
            Response.Cookies.Append("refreshToken", result.RefreshToken, new CookieOptions
            {
                HttpOnly =  true,
                Secure = false,
                SameSite = SameSiteMode.None,
                Expires = DateTimeOffset.Now.AddDays(30)
            });
        return Ok(ApiResponse<AuthDto>.Success(result, "Login successfully"));
    }

    [HttpGet("account")]
    public async Task<ActionResult<ApiResponse<UserDto>>> GetAccount(CancellationToken ct)
    {
        var user = await _authService.GetAccountAsync(ct);
        return Ok(ApiResponse<UserDto>.Success(user));
    }

    [AllowAnonymous]
    [HttpPost("refresh")]
    public async Task<ActionResult<ApiResponse<AuthDto>>> Refresh(CancellationToken ct)
    {
        var refreshToken = Request.Cookies["refreshToken"];
        var result = await _authService.RefreshTokenAsync(refreshToken, ct);
        Response.Cookies.Append("refreshToken", result.RefreshToken, new CookieOptions
        {
            HttpOnly =  true,
            Secure = false,
            SameSite = SameSiteMode.None,
            Expires = DateTimeOffset.Now.AddDays(30)
        });
        return Ok(ApiResponse<AuthDto>.Success(result, "Refresh successfully"));
    }
}
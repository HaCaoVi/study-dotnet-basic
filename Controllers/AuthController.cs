using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using project_basic.Common;
using project_basic.Dtos.AuthDtos;
using project_basic.Dtos.UserDtos;
using project_basic.Services.Interfaces;

namespace project_basic.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
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
        SetRefreshTokenCookie(result.RefreshToken);
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
        SetRefreshTokenCookie(result.RefreshToken);
        return Ok(ApiResponse<AuthDto>.Success(result, "Refresh successfully"));
    }

    [HttpPost("logout")]
    public async Task<ActionResult> Logout(CancellationToken ct)
    {
        await _authService.LogoutAsync(ct);
        Response.Cookies.Delete("refreshToken");
        return NoContent();
    }

    /// <summary>
    /// Sets the refresh token as an HTTP-only secure cookie.
    /// Extracted to avoid duplication between Login and Refresh endpoints.
    /// </summary>
    private void SetRefreshTokenCookie(string refreshToken)
    {
        Response.Cookies.Append("refreshToken", refreshToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.None,
            Expires = DateTimeOffset.UtcNow.AddDays(30)
        });
    }
}